/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/13/2017
 * Time: 10:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MeleeCombat
{
	
	public delegate void OnTransition (Animator anim, AnimatorStateInfo stateInfo, int layerIndex);
	
	public class BaseMechanimBehavior : StateMachineBehaviour
	{
		
		public event OnTransition OnEnterTransitionEnd;
		public event OnTransition OnExitTransitionBegin;
		internal bool checkingForExitTransition;
		internal bool checkingForEnterTransition;
		internal MeleeController controller;
		
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			checkTransitions(animator,stateInfo,layerIndex);
		}
		
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			controller = animator.gameObject.GetComponent<MeleeController>();
			checkingForEnterTransition = true;
			checkingForExitTransition = true;
		}
		
		public void checkTransitions (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			if (! animator.IsInTransition(layerIndex) && checkingForEnterTransition ){
				if (OnEnterTransitionEnd != null){
					OnEnterTransitionEnd(animator,stateInfo,layerIndex);
				}
				checkingForEnterTransition = false;
				checkingForExitTransition = true;
			} else if (checkingForExitTransition && animator.IsInTransition(layerIndex)) {
				if (animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash) return;
			
				if (OnExitTransitionBegin != null){
					OnExitTransitionBegin(animator,stateInfo,layerIndex);
				}
				checkingForExitTransition = false;
			}
		}
		
		public void disableWeapons (Animator animator) {
			controller.OnEndWeaponSwing();
			animator.SetInteger(MeleeController.attackIndex,(int)AttackDirection.NODIRECTION);
			animator.SetBool("comboing",false);
			animator.SetBool("attacking",false);
			
		}
		
	}
}
