/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/4/2017
 * Time: 7:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;

namespace MeleeCombat
{
	/// <summary>
	/// Description of EquipmentPropertyManager.
	/// </summary>
	public class EquipmentPropertyManager : MonoBehaviour
	{
		internal MeleeController controller;
		internal AudioSource source;

		public virtual void Assign (MeleeController controller) { 
			this.controller = controller;
			source = GetComponent<AudioSource>();	
		}
	}
}
