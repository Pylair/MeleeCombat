

using UnityEngine;
using System;

namespace MeleeCombat.Mechanim_Behaviors
{
	/// <summary>
	/// Description of DodgeBehavior.
	/// </summary>
	public class SpineBehavior : StateMachineBehaviour		
	{
		public const string yDistoriton = "y";
		public const float maximum = 90f;
		
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
		}
	}
}
