/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 7/11/2017
 * Time: 11:10 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MeleeCombat
{
	
	
	public class DefaultBehavior : BaseMechanimBehavior
	{
		public static string isDefaultStateBool = "isInDefaultState";
		
		public void Awake (){ 
			OnEnterTransitionEnd += resetAttackBehavior;
		}
		
		 public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   	 	{
		 	base.OnStateEnter(animator,stateInfo,layerIndex);
		 	controller.setMovementInput(true);
			animator.SetBool(isDefaultStateBool,true);
			animator.SetBool("dodging",false);
			base.disableWeapons(animator);
		}

		void resetAttackBehavior (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			controller.resetAttackInput();
			
			controller.feinting = false;
		}
		
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.SetBool(isDefaultStateBool,false);
		}
		
	}
}
