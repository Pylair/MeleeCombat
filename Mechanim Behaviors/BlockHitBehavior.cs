/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/14/2017
 * Time: 5:52 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;


namespace MeleeCombat.Mechanim_Behaviors
{
	/// <summary>
	/// Description of BlockHitBehavior.
	/// </summary>
	public class BlockHitBehavior : BaseMechanimBehavior
	{
		public override  void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   	 	{
			base.OnStateEnter(animator,stateInfo,layerIndex);
			controller.receivingAttackInput = false;
			controller.acceptingMovementInput = false;
		}
		
		void Awake () {
			OnExitTransitionBegin += exit;
		}
		
		
		void exit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			//animator.applyRootMotion = false;
			controller.acceptingMovementInput = true;
			controller.receivingAttackInput = true;
		}
	}
}
