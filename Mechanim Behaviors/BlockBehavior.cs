/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/7/2017
 * Time: 2:41 PM
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
	public class BlockBehavior : StateMachineBehaviour			
	{
			
		MeleeController controller;
		DamageManager damageManager;
		public static float staminaDrainPerSecondWhileBlocking = .1f;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			controller = animator.gameObject.GetComponent<MeleeController>();
			damageManager = controller.damageManager;
			controller.OnBlockBegin(new EventArgs());
		}
		
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (animator.GetNextAnimatorStateInfo(layerIndex).IsName("Default")) {
				controller.OnBlockEnd(new EventArgs());
			}
			
			if (controller.blocking){
				controller.damageManager.changeStamina(- Time.deltaTime *( staminaDrainPerSecondWhileBlocking));
			}
			
			
		}
		
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			controller.OnBlockEnd(new EventArgs());
		}
	}
}
