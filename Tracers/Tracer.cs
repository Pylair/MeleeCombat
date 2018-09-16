/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/3/2017
 * Time: 11:21 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;

namespace MeleeCombat.Tracers
{
	/// <summary>
	/// Description of Tracer.
	/// </summary>
	public class Tracer: MonoBehaviour
	{
		MeleeController controller;
		public bool Tracing {get{return tracing;}}
		bool tracing;
		
		public void OnEquip(MeleeController controller)
		{
			this.controller = controller;
			controller.SwingBegan += enableSwingRecording;
			controller.SwingFinished += disableSwingRecording;
		}
		
		public void OnUnequip () {
			controller.SwingBegan -= enableSwingRecording;
			controller.SwingFinished -= disableSwingRecording;
		}
		
		
		void enableSwingRecording (MeleeController sender) {
			tracing = true;
		}
		
		void disableSwingRecording (MeleeController sender ){
			tracing = false;
		}
	}
}
