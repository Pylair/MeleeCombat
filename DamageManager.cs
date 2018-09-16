/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 7/9/2017
 * Time: 10:26 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using MeleeCombat.UI;

namespace MeleeCombat
{
	
	public enum Attributes : int {HEALTH = 0, STAMINA = 1}
	
	
	public delegate void StatEventHandler (EventArgs e);
	[RequireComponent(typeof(MeleeController))]
	
	public class DamageManager : MonoBehaviour
	{
		
		public const string attackSpeedName = "attackSpeed";
		public float AttackSpeed {
			get {return controller.animator.GetFloat ( attackSpeedName);}
			set {controller.animator.SetFloat ( attackSpeedName, value);}
		}
		
		public float maxHealth;
		public float health;
		
		public float maxStamina;
		public float stamina;
		
		public static float lowStaminaThreshHold = 5;
		public Faction faction;
		
		public float timeSincePhysicalExertion;
	
		public ProgressBar healthBar;
		public ProgressBar staminaBar;
		public static float minimumTimeForStaminaRecovery = 5;
		public static float staminaRecoveredPerSecond = 1f;

		public MeleeController controller;
		AudioSource source;
		
		public event StatEventHandler OnLowStamina;
		public event StatEventHandler OnNotLowStamina;
		
		public bool isLowStamina {
			get {
				return stamina <= lowStaminaThreshHold;
			}
		}

		public void ApplyDamage (Weapon w, Faction weaponOwnerFaction){
			if (weaponOwnerFaction != null && faction != null && weaponOwnerFaction.faction == faction.faction) return;
			
			controller.playDamageAnim();
			controller.playDamageAudio();
			controller.OnDamageApplied(this);
	
			changeHealth(-w.damage);
			changeStamina(-w.staminaDamage);

		}
		
		public float capAttribute (float attribute, float min, float max){
			attribute = Math.Max(min,attribute );
			attribute = Math.Min(max,attribute );
			return attribute;
		}
		
		public void changeHealth (float dH){
			if (dH < 0){
				interruptStaminaRecovery();
			}
			
			health = capAttribute(health + dH,0,maxHealth);
			if (healthBar != null){
				healthBar.setValue(health / maxHealth );
			}
		}

		
		public void interruptStaminaRecovery () {
			timeSincePhysicalExertion = 0;
		}
		
		public void OnLowStaminaEnter () {
		
			if (OnLowStamina != null){
				OnLowStamina(new EventArgs());
			}	
		}
		
		public void OnLowStaminaExit () {
			if (OnNotLowStamina != null){
				OnNotLowStamina(new EventArgs());
			}
		}
		const float lowStaminaAttackSpeedPenalty = .1f;
		
		public void RemoveLowStaminaPenalties (EventArgs e){
			AttackSpeed += lowStaminaAttackSpeedPenalty;
		}
		
		public void ApplyLowStaminaPenalties (EventArgs e){
			AttackSpeed -= lowStaminaAttackSpeedPenalty;
		}
		
		public void changeStamina (float ds) {
			if (ds < 0){
				interruptStaminaRecovery();
			}
			var previous = stamina;
			stamina = capAttribute(stamina + ds,0,maxStamina);
			
			if (isLowStamina && previous > lowStaminaThreshHold){
				OnLowStaminaEnter();
			} else if (previous <= lowStaminaThreshHold && stamina > lowStaminaThreshHold){
				OnLowStaminaExit();
			}
			
			if (staminaBar != null){
				staminaBar.setValue(stamina/maxStamina);
			}
		}
		
		
		public void Update () {
			if (controller != null && controller.enabled){
				timeSincePhysicalExertion += Time.deltaTime;		
				regenStamina();
				if (health == 0){
					controller.die();
				}
			}
		}
		
		public void regenStamina () {
			if (timeSincePhysicalExertion > minimumTimeForStaminaRecovery){
				changeStamina( Time.deltaTime * staminaRecoveredPerSecond);
			}
		}
		
		public void Awake () {
			controller = gameObject.GetComponent<MeleeController>();
			source = gameObject.GetComponent<AudioSource>();
			OnLowStamina += ApplyLowStaminaPenalties;
			OnNotLowStamina += RemoveLowStaminaPenalties;
		}

		
		
	}
}
