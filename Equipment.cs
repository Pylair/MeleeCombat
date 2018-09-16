/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 7/3/2017
 * Time: 12:59 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;
using MeleeCombat.Tracers;

namespace MeleeCombat
{
	
	public enum EquipmentType : int{SHIELD = 0, MELEE = 1, RANGED = 2}
	
	public class Equipment : MonoBehaviour
	{
		
		public Bodypart[] equippableSlots;
		public WeightClass weight;
		public EquipmentType equipmentType;
		internal MeleeController controller;
		public HandManager manager;
		Tracer tracer;
		
		public virtual void Assign (HandManager manager){
			if (tracer != null){
				tracer.OnUnequip();
			}
			controller = manager.controller;
			this.manager = manager;
			tracer = GetComponentInChildren<Tracer>();
			if (tracer != null){
				tracer.OnEquip(controller);
			}
		}
	}
}
