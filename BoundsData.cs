/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 11/7/2017
 * Time: 11:46 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;

namespace MeleeCombat.MapGeneration
{
	
	public class BoundsData : MonoBehaviour
	{
		public int max;
		public int min;
		public Vector3 up {
			get { return gameObject.transform.up;}
		}
		public Vector3 right {
			get { return gameObject.transform.right;}
		}
		
		
		public new BoundedSpaceTags tag;
	}
}
