
/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 7/20/2017
 * Time: 2:37 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MeleeCombat.AI;


namespace MeleeCombat
{
	
	
	public class TacticalControl 
	{
		Texture2D threatMap;
		
		public const float blockRadius = 1f;
		public const int audioDetectionRange = 5;
		public const int numberOfSubIntervals = 2;
		
		List<MeleeController> targetList;
		SceneControl sceneControl;
		AIController controller;
		GameObject character;
		float stopRadius;
		
		MeleeController enemyController;
		GameObject target;
		Faction faction;
		
		public AttackAnalyzer analyzer;
		internal NavMeshAgent agent;
		
		public TacticalControl(GameObject cha){
			this.character = cha;
			threatMap = new Texture2D( numberOfSubIntervals * audioDetectionRange,numberOfSubIntervals * audioDetectionRange);
			targetList = new List<MeleeController>();
			controller = character.GetComponent<AIController>();
			agent = character.GetComponent<NavMeshAgent>();
			stopRadius = 3;
			analyzer = new AttackAnalyzer(controller);
			agent.updateRotation = false;
			agent.isStopped = false;
			faction = controller.faction();
		}
		
		public MeleeController getNearestTarget () {
			if (targetList.Count == 0) return null;
			targetList.Sort(new ProximityComparer(character));
			return targetList[0];
		}
		
		public static float destinationDistance = .1f;
		
		public void Update(){
			acquireTargets();
			analyzer.CollectInfomation();
			lookTo();
		}
		
		public MeleeController EnemyController {
			get { return enemyController;}
		}
		
		public GameObject Target {
			get{return target;}
		}
		
		public void maintainProximityWithoutLookRotation (float distance,bool copySpeed){
			if (target == null) return;
			moveTo(distance,0);
		}
		
		public void maintainProximity (float distance,bool copySpeed) {
			if (target == null) return;
			maintainProximityWithoutLookRotation(distance,copySpeed);
			lookTo(false);
		}
		
		
		Vector3 calculateApproachVector (float r){
			var targetPos = target.transform.position;
			var charPos = character.transform.position;
			var approachVector = targetPos - charPos;
			
			var distance = Vector3.Distance(targetPos,charPos);
			approachVector = approachVector.normalized;
			approachVector *= (distance - r);
			return approachVector;
		}
		
		
		public void moveTo (float r,float angle){
			var agentPos = character.transform.position;
			var approachVector = calculateApproachVector(r);
			move ( agentPos + approachVector);
		//	move(target.transform.position);
		}
		
		public void move (Vector3 pos){
			if ( ! controller.acceptingMovementInput) return;
			if (! agent.enabled) return;
			agent.destination = pos;
		}
			
		public void moveTo (){
			moveTo(stopRadius,0);
		}
		
		public float distanceFromTarget () {
			if (target == null) return float.PositiveInfinity;
			return Vector3.Distance(character.transform.position,target.transform.position);
		}
		
		public float smoothTime = 10f;
		
		public void lookTo (GameObject g){
			var v1 =character.transform.forward;
			var v2 = (g.transform.position - character.transform.position);
			var angle = Vector3.Angle(v1,Vector3.up);	
			v2 = new Vector3(v2.x,0,v2.z);
			lookTo(v2);	
		}
		
		Vector3 lookVector = Vector3.forward;
		
		public void lookTo ( Vector3 v2){
			lookVector = v2;
			lookTo();
		}
		
		public void lookTo () {
			var rot =Quaternion.LookRotation(lookVector,Vector3.up);
			var localRot = character.transform.localRotation;
			localRot = Quaternion.Slerp(localRot,rot, smoothTime * Time.deltaTime);	
			character.transform.localRotation = localRot;
		}
		
		public void lookTo(bool lookAtNextPointInPath){
			if (target == null) lookAtNextPointInPath = true;
			
			var v1 =character.transform.forward;
			
			v1 =  new Vector3(v1.x,0,v1.z);
			Vector3 v2;
			
			if (lookAtNextPointInPath && agent.enabled){
				v2 = agent.destination - character.transform.position;
			} else {
				v2 = (target.transform.position - character.transform.position);
			}

			v2 = new Vector3(v2.x,0,v2.z);
			lookTo(v2);
		}
		
		public void lookAtTracer (Vector3 axis, bool fallBack, AttackDirection direction){
			if (enemyController == null) return;
			var tracer = enemyController.weaponTracer;
			if (tracer == null){
				if (! fallBack) return;
				lookTo(target);
				return;
			}
			
			var y = controller.animator.GetFloat("y");
			var interval = 10;
			
			if (direction == AttackDirection.UPWARD){
				y+= 3 *interval;
			} else if (direction == AttackDirection.DOWN){
				y -= interval;
			} else {
				if (y > 90){
					y -= interval;
				} else {
					y += interval;
				}
			}
			y = Math.Max(y,60);
			y = Math.Min(y,120);
			controller.animator.SetFloat("y",y);
			
		}
		
		public void designateTarget (GameObject g){
			var cont = g.GetComponent<MeleeController>();
			if (! targetList.Contains(cont)){
				targetList.Add(cont);
			}
			target = g;
			enemyController = cont;
			enemyController.OnDeathEvent += unsetTargetOnDeath;
			
		}
		
		public void unsetTargetOnDeath (MeleeController targetController){	
			var g = targetController.gameObject;
			if (target == g){
				target = null;
				targetList.Remove(targetController);
				enemyController.OnDeathEvent -= unsetTargetOnDeath;
				enemyController = null;
			}
		}
		
		public float bufferZone = .1f;
		
		public bool isInStopRadius (){
			return isInRadius(stopRadius);
		}
		
		public bool isInRadius (float radius){
			var distance = distanceFromTarget();
			return distance < radius + bufferZone;
		}
		const float xFrustumDegrees = 60f;
		const float yFrustrumDegrees = 60f;
		const float maximumVisualRange = 20f;
	
		
		bool canSeeTarget (MeleeController possibleEnemy ) {
			float distance =Vector3.Distance(possibleEnemy.transform.position,controller.gameObject.transform.position) ;
			if (distance > maximumVisualRange) return false;
			var transform = controller.gameObject.transform;
			var forward = transform.forward;
			var right = transform.right;
			var up = transform.up;
			
			var vector =-(transform.position - possibleEnemy.gameObject.transform.position);
		
			var xPlaneProj = Vector3.ProjectOnPlane(vector,up);
			var yPlaneProj = Vector3.ProjectOnPlane(vector,right);
			var xAngle = Vector3.Angle(xPlaneProj,forward);
			var yAngle = Vector3.Angle(yPlaneProj,forward);
		
			if (xAngle < xFrustumDegrees && yAngle < yFrustrumDegrees){
				RaycastHit hit;
				Physics.Raycast(transform.position,vector,out hit , maximumVisualRange);
				var mc = hit.collider.gameObject.GetComponentInParent<MeleeController>();
		
				if (mc == null) return false;
				return mc.Equals(possibleEnemy);
			} 
			return false;
		}
		
		void debugVisionFrustrum (Vector3 origin, Vector3 vector,Transform transform){
			//Debug.DrawRay(origin,vector,Color.cyan,.1f);
			var forward = transform.forward;
			var right = transform.right;
			var up = transform.up;
			
			var v1 = Quaternion.AngleAxis(xFrustumDegrees,up) * forward;
			var v2 = Quaternion.AngleAxis(-xFrustumDegrees,up) * forward;
			var v3 = Quaternion.AngleAxis(yFrustrumDegrees,right) * forward;
			var v4 = Quaternion.AngleAxis(-yFrustrumDegrees,right) * forward;
	

			Debug.DrawRay(origin,v1,Color.red,.1f);
			Debug.DrawRay(origin,v2,Color.yellow,.1f);
			Debug.DrawRay(origin,v3,Color.green,.1f);
			Debug.DrawRay(origin,v4,Color.blue,.1f);
		}
		
		public void acquireTargets (){
			if (sceneControl == null){
				sceneControl = SceneControl.sceneCont;
			}
			var count = sceneControl.characterList.Count;
			for (int i = 0; i < count;i++){
				
				var otherCharacter = sceneControl.characterList[i];
				var otherController = otherCharacter.GetComponent<MeleeController>();
				
				if (! otherController.alive ) continue;
				if (otherCharacter.Equals(character)) continue;			
				if (otherController.faction().Equals(faction)) continue;
				 
				if (Vector3.Distance(otherCharacter.transform.position,character.transform.position) < audioDetectionRange || canSeeTarget(otherController)){
					if (targetList.Contains(otherController)) continue;
					targetList.Add(otherController);
				} else {
					if (targetList.Contains(otherController)){
						targetList.Remove(otherController);
					}
				}
				
			}
		}
		
		public float evaluateRange (GameObject obj){
			var mC = obj.GetComponent<MeleeController>();
			var rightWeap = mC.GetRightWeapon();
			var leftWeap = mC.GetLeftWeapon();
			if (rightWeap == null && leftWeap == null) {
				return 0;
			} else if (rightWeap == null && leftWeap != null){
				return leftWeap.range;
			} else if (rightWeap != null && leftWeap == null){
				return rightWeap.range; 
			} else {
				return Math.Max(rightWeap.range,leftWeap.range);
			}
		}
		
		public bool isWithinRangeOf (float d, Vector3 des){
			return Vector3.Distance(controller.transform.position,des) < d;
		}

	}
	
	
	
}
