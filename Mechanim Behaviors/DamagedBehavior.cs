/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/7/2017
 * Time: 4:01 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;

namespace MeleeCombat
{
	/// <summary>
	/// Description of AnimationCoordinater.
	/// </summary>
	public class DamagedBehavior : BaseMechanimBehavior
	{
		public static string isDefaultStateBool = "isInDefaultState";

		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   	 	{
			base.OnStateEnter(animator,stateInfo,layerIndex);
			animator.SetBool("dodging",false);
			animator.SetBool("flinching",true);
			controller.setMovementInput(false);
			
			controller.OnBlockEnd(new EventArgs());
			disableWeapons(animator);
			animator.applyRootMotion = false;
		}
		
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.SetBool("flinching",false);
		}
	}
}
