/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 7/12/2017
 * Time: 7:37 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MeleeCombat.InventoryClasses
{

	public class Inventory : MonoBehaviour
	{
		
		const string pathHeader = "Items/";
		public List<string> itemPaths;
		
		public static GameObject loadItem (string s){
			return GameObject.Instantiate(Resources.Load(pathHeader + s) as GameObject);
		}
		
		public GameObject loadItem (int index){
			if (index > itemPaths.Count - 1) return null;
			var path = itemPaths[index];
			itemPaths.RemoveAt(index);
			return loadItem(path);
		}
		
		public void equipItem (int i,Bodypart part,MeleeController controller){
			var g = loadItem(i);
			var equipment = g.GetComponent<Equipment>();
			if (equipment == null) return;
			var slots = new List<Bodypart>(equipment.equippableSlots);
			if (! slots.Contains(part)){
				return;
			}
			controller.skeletonMap[part].GetComponent<HandManager>().equip(g,true);
		}
	}
}
