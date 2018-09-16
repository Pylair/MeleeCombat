/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 7/22/2017
 * Time: 3:27 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using MeleeCombat;
using System.Collections.Generic;

namespace MeleeCombat.AI
{

	public class ProximityComparer : IComparer<MeleeController>{
		
		public GameObject center;
		
		public ProximityComparer (GameObject center){
			this.center = center;
		
		}
		
		public int Compare (MeleeController a, MeleeController b){
			var d1 = Vector3.Distance(a.gameObject.transform.position,center.gameObject.transform.position);
			var d2 = Vector3.Distance(b.gameObject.transform.position,center.gameObject.transform.position);
			return d1.CompareTo(d2);	
		}
		
		
	}
}
