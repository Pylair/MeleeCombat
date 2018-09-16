/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 10/8/2017
 * Time: 11:33 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using MeleeCombat;
using UnityEngine;
using System.Collections.Generic;

namespace MeleeCombat.MapGeneration
{
	/// <summary>
	/// Description of PropOverlay.
	/// </summary>
	public class PropOverlay : MonoBehaviour
	{
		public List<FaceTypes> excludedTags;
		public GameObject prop;
		
		public List<int> numberOfWalls;
		public Vector3 size;
		public bool highPriority;
		public bool extendable;
		
		
	}
}
