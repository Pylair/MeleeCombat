/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 7/22/2017
 * Time: 3:07 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using MeleeCombat;
using System.Collections.Generic;
using UnityEngine.AI;
using MeleeCombat.Mechanim_Behaviors;

namespace MeleeCombat.AI
{
	
	using Random = System.Random;
	public delegate void OnTransitionEventHandler (MeleeCombatState nextState);
	
	public interface MeleeCombatState 
	{
		AIController controller {get;set;}
		TacticalControl tacticalControl {get;set;}
		
		 void executeStateOrders ();
		 void evaluateNextState ();
		 event OnTransitionEventHandler OnTransition;	
	}
	
	public class Idle : MeleeCombatState {
		
		public AIController controller {get;set;}
		public TacticalControl tacticalControl {get;set;}
		
		public Idle (AIController controller) {
			this.controller = controller;
			tacticalControl = controller.TacticalControl;
		}
		
		public void executeStateOrders (){
			if (! controller.equipped){
				controller.drawEquipment();
			}
			
		}
		
		public void evaluateNextState (){
			var target = tacticalControl.getNearestTarget();
			if (target != null){
				tacticalControl.designateTarget(target.gameObject);
				OnTransition(new Pursuit(controller));
			}
		}
		
		public event OnTransitionEventHandler OnTransition;
		
	}
	
	public class Pursuit : MeleeCombatState {
		
		
		public AIController controller {get;set;}
		public TacticalControl tacticalControl {get;set;}
		
		
		public Pursuit (AIController controller) {
			this.controller = controller;
			tacticalControl = controller.TacticalControl;
		}
		
		public void executeStateOrders (){
			tacticalControl.maintainProximity(1,false);
		}
		
		public void evaluateNextState (){
	
			var target = tacticalControl.Target;
			if (target == null) {
				OnTransition(new Idle(controller));
				return;
			}
			var targetController = target.GetComponent<MeleeController>();
			if (tacticalControl.isInStopRadius()) {
				bool isTargetAttacking = targetController.attacking;
				bool isTargetBlocking = targetController.blocking;
				OnTransition(new Standoff(controller));
			}
		}
			
		public event OnTransitionEventHandler OnTransition;
	}
	
	public class Standoff : MeleeCombatState {
		
		Random r= new Random();
		public float dT;
		
		public float timeSinceStateEnter;
		
		
		public AIController controller {get;set;}
		public TacticalControl tacticalControl {get;set;}
		
		
		public Standoff (AIController controller) {
			this.controller = controller;
			tacticalControl = controller.TacticalControl;
		}

		public void evaluateNextState(){	
			timeSinceStateEnter += Time.deltaTime;
			if (! tacticalControl.isInStopRadius()){
				OnTransition(new Pursuit(controller));
			}/* else if (tacticalControl.analyzer.enemyIsAttacking){
				OnTransition(new Block(controller));
			} else {
				OnTransition(new Attack(controller));
			}*/
			else if (controller.aggression > .5){
				OnTransition(new Attack(controller));
			} else {
				OnTransition(new Block(controller));
			}
		}
		
		public void executeStateOrders (){	
			tacticalControl.maintainProximity(1.25f,true);
		}
		
		public event OnTransitionEventHandler OnTransition;
	}
	
	
	public class Attack : MeleeCombatState{
		
		
		public AIController controller {get;set;}
		public TacticalControl tacticalControl {get;set;}
		GameObject target;
		
		float dT = 0;
		float delayTime;
		float minRange = standOffMinRange;
		bool willFeint = false;
		bool acceptingInput = false;
		
		public static float comboProb = .25f;
		public static float delayMultiplier = 1;
		public static float feintProb = .5f;
		public static float standOffMinRange = 2.5f;
		public static float weaponMinRange = 1.25f;
		public static float absoluteMinimum = .5f;
		

		public Attack (AIController controller) {
			this.controller = controller;
			tacticalControl = controller.TacticalControl;
			target = tacticalControl.Target;
			controller.executeBlock(false);
			controller.EnableWeapon += setMinWeaponRange;
		}
		
		void setMinWeaponRange (Weapon weapon) {
			minRange = absoluteMinimum + weapon.range;
		}

		
		float calculateDelayTime (){
			comboing = MeleeController.r.NextDouble() < comboProb;
			if (comboing){
				return 0;
			}
			return (float)MeleeController.r.NextDouble() * delayMultiplier;
		}

		void decideFeint (AttackDirection dir) { 
			if (tacticalControl.EnemyController.flinching || dir == AttackDirection.LTR){
				willFeint = false;
			} else {
				willFeint = MeleeController.r.NextDouble()  < feintProb;
			}
			
		}

		void executeFeint () {	
			if (! willFeint ) return;
			if (controller.feint()){
				willFeint = false;
			}
		}
		
		void maintainProximity () {
			tacticalControl.maintainProximity((minRange + absoluteMinimum)/2f,false);
			controller.distanceFromTarget = tacticalControl.distanceFromTarget();
		}
		
		bool comboing;
		void executeOffensive () { 
			var response = tacticalControl.analyzer.FormulateResponse(comboing);
			Bodypart part = Bodypart.RIGHTHAND;	
			if (response != AttackDirection.NODIRECTION && controller.receivingAttackInput){
				if (dT > delayTime){
					decideFeint(response);
					if (response == AttackDirection.KICK){
						part = Bodypart.RIGHTFOOT;
					}
					
					controller.executeAttack(response,part);
					delayTime = calculateDelayTime();
				
				}
			}
			executeFeint();
			acceptingInput = controller.receivingAttackInput;
		}
		
		bool isInOptimalRange () {
			return tacticalControl.distanceFromTarget() > absoluteMinimum  && tacticalControl.distanceFromTarget() < minRange ;
		}
		
		public void executeStateOrders (){	
			dT += Time.deltaTime;
			maintainProximity();
			if (! isInOptimalRange()) return;
			executeOffensive();
		}
		
		public void evaluateNextState () {
			var enemyController = target.GetComponent<MeleeController>();
			var distance = tacticalControl.distanceFromTarget();
			if (! tacticalControl.isInStopRadius()){
				OnTransition(new Pursuit(controller));
				return;
			}
			if ( controller.flinching){
				OnTransition(new Block(controller));
				return;
			}
			
			if (tacticalControl.EnemyController.isAttacking && MeleeController.r.NextDouble() < .05f){
				OnTransition(new Block(controller));
				return;
			}
		}
		
		public event OnTransitionEventHandler OnTransition;
	}
	
	
	
	
	public class Block : MeleeCombatState {
		
		public AIController controller {get;set;}
		public TacticalControl tacticalControl {get;set;}
		GameObject target;
		public const float blockTransitionTime = .02f;
		MeleeController enemyController;
		float dT = 0;
		
		public Block (AIController controller) {
			this.controller = controller;
			tacticalControl = controller.TacticalControl;
			target = tacticalControl.Target;
			enemyController = tacticalControl.analyzer.enemy;
			enemyController.EnableWeapon += EnemyOffensiveActionStart;
			enemyController.SwingFinished += EnemyOffensiveActionEnd;
			timeToAttackTransition = (float)MeleeController.r.NextDouble() * maxTimeToAttackTransition;
		}
		
		public void EnemyOffensiveActionStart (Weapon weapon){
			acceptingResponses = true;
			//acceptingCounterAttackResponses = false;
		}
		
		public void EnemyOffensiveActionEnd (MeleeController controller){
			//acceptingCounterAttackResponses = true;
			//controller.executeBlock(false);
		}
		
		bool acceptingResponses = false;
		float blockDistance = 2f;
		const float closeRange = 1.5f;
		float timeToAttackTransition;
		const float maxTimeToAttackTransition = 1.5f;
		
		public void executeStateOrders (){
			dT += Time.deltaTime;
			tacticalControl.maintainProximity(blockDistance,false);
			if (controller.flinching && !acceptingResponses){
				acceptingResponses = true;
			}
			controller.executeBlock(true);
			if (acceptingResponses){
				acceptingResponses = false;

				if (controller.canDodge()){
					var current = tacticalControl.EnemyController.getCurrentAttackDirection;
					if (current == AttackDirection.KICK || current == AttackDirection.RTL){
						checkDodge();
						return;
					}
					controller.executeBlock(true);
				} 
				
			} 
		}
		
		void checkDodge () { 
			var enemyDirection = tacticalControl.analyzer.enemy.getCurrentAttackDirection;
			if (enemyDirection == AttackDirection.DOWN || enemyDirection == AttackDirection.UPWARD){
				if (MeleeController.r.NextDouble() < .5){
					controller.dodge(0,MeleeController.runSpeed);
				} else {
					controller.dodge(0,-MeleeController.runSpeed);
				}	
			} else {
				controller.dodge(-MeleeController.runSpeed,-MeleeController.runSpeed);
			}
		}
		
		public void evaluateNextState (){
			if (controller.flinching || enemyController.attacking) return;
			if (! tacticalControl.isInStopRadius()){
				controller.executeBlock(false);
				OnTransition(new Pursuit(controller));
				return;
			} 
			
			if (dT > timeToAttackTransition || enemyController.flinching || tacticalControl.distanceFromTarget() < closeRange){
				dT = 0;
				OnTransition(new Attack(controller));
			}	
		}
		
		public event OnTransitionEventHandler OnTransition;
	}

	public class RunTest : MeleeCombatState {
		
	
		Vector3 originalPosition;
		List<Vector3> destinations = new List<Vector3>();

		
		
		public AIController controller {get;set;}
		public TacticalControl tacticalControl {get;set;}
		
		public RunTest ( AIController controller){
			this.controller = controller;
			tacticalControl = controller.TacticalControl;
		}
		
		public void evaluateNextState (){
			
		}
		
		public void executeStateOrders (){
			controller.dodge(0,-MeleeController.runSpeed);
		}
		
		public event OnTransitionEventHandler OnTransition;
		
	}
	
	
	public class Death : MeleeCombatState {
		public AIController controller {get;set;}
		public TacticalControl tacticalControl {get;set;}
		
		public Death ( AIController controller){
			this.controller = controller;
			tacticalControl = controller.TacticalControl;
		}
		
		public void evaluateNextState (){
			
		}
		
		public void executeStateOrders (){
		
		}
		
		public event OnTransitionEventHandler OnTransition;
		
	}
	
	
	public class Surround : MeleeCombatState {
		public AIController controller {get;set;}
		public TacticalControl tacticalControl {get;set;}
		GameObject g;
	
		public Surround ( AIController controller){
			this.controller = controller;
			tacticalControl = controller.TacticalControl;
			g = GameObject.Find("TAG");
		}
		
		public void evaluateNextState (){
			
		}
		
		
		public void executeStateOrders (){
			//if (swi){
				controller.Team.surround(g);
			//	swi = false;
			//}
		}
		
		public event OnTransitionEventHandler OnTransition;
	}
	
}
