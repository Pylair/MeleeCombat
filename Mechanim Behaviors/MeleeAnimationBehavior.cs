/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/4/2017
 * Time: 10:41 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MeleeCombat
{
	/// <summary>
	/// Description of MeleeAnimationBehavior.
	/// </summary>
	public class MeleeAnimationBehavior : BaseMechanimBehavior
	{
		
		bool checkingComboing = true;
		public int index;
		public List<int> legalTransitions;
		public float endComboInputPhase;
		public float windupPhaseLength;
		public bool shouldApplyRootMotion;
		
		public void Awake () {
			OnExitTransitionBegin += AttackEnd;
			OnEnterTransitionEnd += enableFeints;
		}
		
		public void enableFeints (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			controller.canFeint = true;
		}
		
		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   	 	{
			base.OnStateEnter(animator,stateInfo,layerIndex);
			animator.SetBool("attacking",true);
			checkingComboing = true;
			controller.receivingAttackInput = true;
			animator.applyRootMotion = shouldApplyRootMotion;
		}		
		
		public void AttackEnd (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			animator.SetBool("attacking",false);
			controller.animator.applyRootMotion = false;
		}
		

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator,stateInfo,layerIndex);
			
			if (stateInfo.normalizedTime > endComboInputPhase){
				checkingComboing = false;
				animator.SetBool("comboing",false);
			}
			
			if (checkingComboing && ! controller.receivingAttackInput){
				var next = animator.GetInteger(MeleeController.attackIndex);
			
				if ( legalTransitions.Contains(next) && next != (int)AttackDirection.NODIRECTION){
					animator.SetBool("comboing",true);
					checkingComboing = false;
				} else {
					animator.SetBool("comboing",false);
					animator.SetInteger(MeleeController.attackIndex,(int)AttackDirection.NODIRECTION);
				}
			}
			
			if (controller.getCurrentAttackDirection == AttackDirection.NODIRECTION){
				animator.SetBool("comboing",false);
			}
		}

	}
}
