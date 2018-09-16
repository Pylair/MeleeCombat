/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 7/30/2017
 * Time: 11:31 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;

namespace MeleeCombat.AI
{
	
	public enum AttackResponse : int {ATTACK = 0, BLOCK = 1, EVADE = 2, COUNTER = 3, GUARDBREAK = 4}
	
	public class AttackAnalyzer
	{

		float enemyAttackAnimationTime;
		AttackDirection enemyAttackDirection;
		AIController controller;

		public MeleeController enemy {
			get { return controller.TacticalControl.EnemyController;}
		}
		
		public bool enemyIsAttacking {
			get {
				if (enemy == null) return false;
				return enemy.isAttacking;
			}
		}
		
		public AttackAnalyzer (AIController con){
			this.controller = con;
		}
		
		public void CollectInfomation () {
			if (enemy == null) return;
			enemyAttackAnimationTime = enemy.animator.GetCurrentAnimatorStateInfo(MeleeController.attackLayer).normalizedTime;
			enemyAttackDirection = (AttackDirection)enemy.animator.GetInteger(MeleeController.attackIndex);
		}
		
		AttackDirection previous;
		
		public AttackDirection FormulateResponse (bool comboing) {
			if (enemy == null) {
				return AttackDirection.NODIRECTION;
			}
			var current = (AttackDirection)controller.animator.GetInteger(MeleeController.attackIndex);
			if (enemy.blocking){
				if (MeleeController.r.NextDouble() < .25f || enemy.damageManager.isLowStamina){
					return (AttackDirection)MeleeController.r.Next(0,5);
				} else {
					return AttackDirection.KICK;
				}
			} else {
				if (comboing){
					return getCombo(current);
				}
				return (AttackDirection)MeleeController.r.Next(0,5);
			}
		}
		
		AttackDirection getCombo(AttackDirection current){
			var dir = (AttackDirection)MeleeController.r.Next(0,5);
			if (dir == current) return getCombo(current);
			return dir;
		}
	}
}
