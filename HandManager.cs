/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 7/3/2017
 * Time: 12:22 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;

namespace MeleeCombat
{
	/// <summary>
	/// Description of HandManager.
	/// </summary>
	public class HandManager : MonoBehaviour
	{
		public MeleeController controller;
		Vector3 poolLocation = new Vector3(-1000,-1000,-1000);
		public Bodypart part;
		bool occupied;
		public GameObject objectOccupyingBodyPart;

		public Equipment getOccupyingEquipment (){
			if (objectOccupyingBodyPart == null) return null;
			return objectOccupyingBodyPart.GetComponentInChildren<Equipment>();
		}
		
		public void freezeHandPos (){
			gameObject.AddComponent<Rigidbody>();
			var rb = gameObject.GetComponent<Rigidbody>();
			rb.constraints = RigidbodyConstraints.FreezeAll;
		}
		
		
		
		public bool sameObject (GameObject other){
			if (objectOccupyingBodyPart == null){
				if (other == null) return true;
				return false;
			}
			return objectOccupyingBodyPart.Equals(other);
		}
		
		public bool Occupied(){
			return occupied;
		}
		
		public void equipTwoHander (GameObject twoHander,HandManager offHand){
			if (sameObject(twoHander) && offHand.sameObject(twoHander)) {
				unequipTwoHander(offHand);
				return;
			}
			equip(twoHander,true);
			offHand.equip(twoHander,false);
		}
		
		public bool CanUnequipTwoHander (HandManager offHand){
			return offHand.occupied && occupied && offHand.sameObject(objectOccupyingBodyPart);
		}
		
		public GameObject unequipTwoHander (HandManager offHand){
			var g = unEquip(true);
			offHand.unEquip(false);
			return g;	
			
		}
		
		public GameObject swapTwoHander (GameObject twoHander,HandManager offHand){
			var g = unequipTwoHander(offHand);
			equipTwoHander(twoHander,offHand);
			return g;
		}
		
		public GameObject unEquip(bool transform){
			if (! occupied) return null;
			
			
			if (transform){
				objectOccupyingBodyPart.transform.SetParent(null);
				objectOccupyingBodyPart.transform.position = poolLocation;
			}
			var g = objectOccupyingBodyPart;
			objectOccupyingBodyPart = null;
			occupied = false;

			
			var weapon = g.GetComponentInChildren<Weapon>();
			var dm = g.GetComponentInChildren<DeflectionManager>();
			if (weapon != null) weapon.UnAssign();
			if (dm != null) dm.UnAssign();
			
			
			
			return g;
		}
		
		public GameObject equip(GameObject g,bool transform){
			if (occupied && !sameObject(g)) return swap(g,transform);
			if (sameObject(g)){
				return unEquip(transform);
			}

	
			occupied = true;
			objectOccupyingBodyPart = g;
			if (transform){
				g.transform.position = gameObject.transform.position;
				objectOccupyingBodyPart.transform.SetParent(gameObject.transform);
				objectOccupyingBodyPart.transform.localEulerAngles = Vector3.zero;
			}
			
			
			var weapon = g.GetComponentInChildren<Weapon>();
			var dm = g.GetComponentInChildren<DeflectionManager>();
			if (weapon != null) weapon.Assign(this);
			if (dm != null) dm.Assign(controller);;
			
			return objectOccupyingBodyPart;
		}
		
		public GameObject swap (GameObject g,bool transform){
			var unEquipped = unEquip(transform);
			equip(g,transform);
			return unEquipped;
		}
		
		public void Start () {
			controller = GetComponentInParent<MeleeController>();
		}
		
	}
}
