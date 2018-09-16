/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 7/24/2017
 * Time: 2:46 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;

namespace MeleeCombat
{
	
	//public delegate void OnDeflectionEventHandler (DeflectionManager deflectionManager);
	
	public class DeflectionManager : EquipmentPropertyManager
	{
		
		public static float StaminaDrainOnDeflection = .5f;
		public bool alwaysDeflecting;
		public bool deflecting;
		
		//public event OnDeflectionEventHandler DeflectionRegistered;
		
		public bool canDeflect () {
			if (controller.damageManager.isLowStamina) return false;
			return deflecting;
		}
		
		public void OnDeflection () {
			source.Play();
			controller.damageManager.changeStamina(-StaminaDrainOnDeflection);
			controller.animator.CrossFade("Block Hit",.05f,MeleeController.blockLayer);
		}
		
		public void enableDeflection (object sender, EventArgs e){
			deflecting = true;
		}
		
		public void disableDeflection (object sender, EventArgs e){
			if (alwaysDeflecting) return;
			deflecting = false;
		}
		
		public override void Assign (MeleeController controller){
			base.Assign(controller);
			controller.BlockBegin += enableDeflection;
			controller.BlockEnd += disableDeflection;
		}
		
		public void UnAssign (){
			controller.BlockBegin -= enableDeflection;
			controller.BlockEnd -= disableDeflection;
			controller = null;
		}
	}
}
