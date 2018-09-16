/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/6/2017
 * Time: 6:32 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;

namespace MeleeCombat.Mechanim_Behaviors
{
	/// <summary>
	/// Description of DodgeBehavior.
	/// </summary>
	public class DodgeBehavior : BaseMechanimBehavior
	{
	
		
		
		public override  void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   	 	{
			base.OnStateEnter(animator,stateInfo,layerIndex);
			enableDodge(animator,stateInfo,layerIndex);
		}
		
		public void Awake () {
			OnExitTransitionBegin += disableDodge;
		}
		
		void enableDodge (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			animator.SetBool("blocking",false);
			animator.SetBool("dodging",true);
			controller.setMovementInput(false);
			animator.applyRootMotion = true;
		}
		
		void disableDodge (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			
			animator.SetBool("dodging",false);
			animator.applyRootMotion = false;
			controller.setMovementInput(true);
			controller.u = 0;
			controller.v = 0;
		}		
	}
}
