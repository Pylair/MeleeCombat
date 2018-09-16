/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 7/13/2017
 * Time: 11:46 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using UnityEngine;
using System;
using Random = System.Random;
using MeleeCombat.InventoryClasses;
using UnityEngine.AI;
using MeleeCombat.AI;


namespace MeleeCombat
{
	public enum CombatMoves : int{IDLE = 0,ADVANCE = 1, RETREAT = 2, BLOCK = 3, ATTACK = 4};
	//Modification of Melee Controller w/o user input
	
	public class AIController : MeleeController
	{
	
		[Range(0,1)]
		public float aggression;
		public string stateName = "";
		public float distanceFromTarget;
			
		MeleeCombatState currentState;
		TacticalControl tacticalControl;
		
		public TacticalControl TacticalControl {
			get{return tacticalControl;}
		}
		
		public override void Awake()
		{
			receivingTeamInstructions = true;
			setUp();
			tacticalControl = new TacticalControl(gameObject);
			//handleTransition(new Surround(this));
			handleTransition(new Idle(this));
		}
		
		public void handleTransition ( MeleeCombatState nextState) {
			if (currentState != null){
				currentState.OnTransition -= handleTransition;
				Debug.Log("current: " + currentState.GetType().Name);
			}
			currentState = nextState;
			currentState.OnTransition += handleTransition;
			stateName = currentState.GetType().Name;			
		}
		
		public bool equipped {
			get {
				return skeletonMap[Bodypart.LEFTHAND].GetComponent<HandManager>().Occupied() ||
					skeletonMap[Bodypart.RIGHTHAND].GetComponent<HandManager>().Occupied();
			}
		}
		
		public override void drawEquipment (){
			var inventory = gameObject.GetComponent<Inventory>();
			var count = inventory.itemPaths.Count;
			while (inventory.itemPaths.Count > 0){
				drawEquipment(inventory.loadItem(0));
			}
		}
		
		public override void handleBodyDirection()
		{
			animator.SetFloat("y",Vector3.Angle(Vector3.up,gameObject.transform.forward));
		}
		
		public override void setMovementInput(bool value)
		{
			tacticalControl.agent.enabled = value;
			base.setMovementInput(value);
		}
		
		public override void movementInput()
		{
			if (acceptingMovementInput) {
				if (! tacticalControl.agent.enabled) tacticalControl.agent.enabled = true;
				var forward = gameObject.transform.forward;
				var right = gameObject.transform.right;
			
				var direction = tacticalControl.agent.steeringTarget - gameObject.transform.position;
				direction = direction.normalized;
				var angle = Vector3.Angle(gameObject.transform.forward,direction);
				var distance = Vector3.Distance(tacticalControl.agent.destination,gameObject.transform.position);
			
				if (dodging){
					u = 0;
					v = 0;
				} else {
					if (tacticalControl.agent.remainingDistance > .025){
						if (angle > 90){
							u -= acceleration;
						} else {
							u += acceleration;
						}
					}			
				}
				friction();
			}
			handleMovement();
			
		}
		
		internal override void capSpeed () {
			var mag = tacticalControl.agent.velocity.magnitude;
			if (mag < 3 && mag > 1){
				mag = 2;
			}
			u = Math.Min(mag,u);
			u = Math.Max(u,-mag);
			if (dodging){
				v = Math.Max(-mag,v);
				v = Math.Min(mag,v);
			} else {
				v = Math.Max(-mag * .75f,v);
				v = Math.Min(mag * .75f,v);
			}
		}
		
		internal override void adjustVelocity(){
			if (! animator.applyRootMotion && acceptingMovementInput){
				tacticalControl.agent.speed = MeleeController.runSpeed * speedMultiplier;
			} else if (animator.applyRootMotion){
				tacticalControl.agent.velocity =  (animator.deltaPosition / 2 * Time.deltaTime);
				tacticalControl.agent.speed = (animator.deltaPosition / 2 * Time.deltaTime).magnitude;
			}
		}

		public override void die () {
			handleTransition(new Death(this));
			base.die();
		}

		public void Update()
		{	
			if (alive){
				movementInput();
				handleBodyDirection();
				tacticalControl.Update();
				
				currentState.executeStateOrders();
				currentState.evaluateNextState();
				distanceFromTarget = tacticalControl.distanceFromTarget();
			}	
		}
	}
}
