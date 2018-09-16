/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 7/9/2017
 * Time: 10:31 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MeleeCombat
{
	/// <summary>
	/// Description of Weapon.
	/// </summary>
	public class Weapon : Equipment
	{
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			Weapon other = obj as Weapon;
				if (other == null)
					return false;
						return gameObject.Equals(other.gameObject);
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * gameObject.GetHashCode();
			}
			return hashCode;
		}

		public static bool operator ==(Weapon lhs, Weapon rhs) {
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Weapon lhs, Weapon rhs) {
			return !(lhs == rhs);
		}

		#endregion
		
		internal bool isSwingingWeapon = false;
		public bool bypassDeflection = false;
		public bool damageEnabled = false;
		public float damage;
		public float staminaDamage;
		public Faction faction;
		public float range;
		public float staminaDrainOnBeingDeflected;
		public float costToSwingWeapon;
		AudioSource source;
		
		bool canPerformAction;
		int count = 0;
		
		HashSet<DamageManager> recentlyDamaged = new HashSet<DamageManager>();
		
		bool isDeflecting (GameObject obj){
			bool output = false;
			var deflectionManager = obj.GetComponent<DeflectionManager>();
			if (deflectionManager == null) return false;
			if (bypassDeflection) return false;
			output = deflectionManager.canDeflect();
			if (output){
				deflectionManager.OnDeflection();
				controller.damageManager.changeStamina(-staminaDrainOnBeingDeflected);
				controller.attackInterrupted();
			}
			return output;
		}
		
		bool isEnemy (GameObject obj) {
			var cont = obj.GetComponentInParent<MeleeController>();
			if (cont != null){
				if (cont.faction() != faction) return true;
			}
			return false;
		}
		
		bool applyDamage (GameObject obj){
			if (count == 1) return false;
			var damageManager = obj.GetComponentInParent<DamageManager>();
			if (damageManager == null || recentlyDamaged.Contains(damageManager)) return false;	
			damageManager.ApplyDamage(this,controller.gameObject.GetComponent<Faction>());
			recentlyDamaged.Add(damageManager);
			source.Play();
			count = 1;		
			return true;
		}

		public void OnTriggerEnter(Collider col){
			var obj = col.gameObject;
			if (damageEnabled){	
				if (! isEnemy(obj)) return;
				if (isDeflecting(obj)) return;
				applyDamage(obj);
			}
		}
		

		public override void Assign (HandManager manager){
			base.Assign(manager);
			faction = controller.faction();
			source = GetComponent<AudioSource>();
			controller.SwingBegan += new WeaponSwingEventHandler(enableSwingDamage);
			controller.SwingFinished += new WeaponSwingEventHandler(disableSwingDamage);
			controller.EnableWeapon += activate;
		}
		
		public void activate (Weapon weapon) {
			canPerformAction = weapon.Equals(this);
		}

		public void UnAssign (){
			faction = null;
			controller.SwingBegan -= enableSwingDamage;
			controller.SwingFinished -= disableSwingDamage;
			controller = null;
		}
		
		public void enableSwingDamage (MeleeController sender){
			if (canPerformAction){
				damageEnabled = true;
				recentlyDamaged.Clear();
				count = 0;
				controller.damageManager.changeStamina(-costToSwingWeapon);
			}
		}
		
		public void disableSwingDamage (MeleeController sender){
			damageEnabled = false;
			isSwingingWeapon = false;
			recentlyDamaged.Clear();
			canPerformAction = false;
			//
		}
	}
}
