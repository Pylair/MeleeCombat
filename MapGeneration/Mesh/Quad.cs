/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 6/17/2018
 * Time: 12:23 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace MeleeCombat.MapGeneration{
	/// <summary>
	/// Description of Quad.
	/// </summary>

		[Serializable]
	public class Quad: Shape{
		
		public Quad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,FaceTypes tag) : base (new Vector3[]{v1,v2,v3,v4}.ToList()){
			this.tag = new BoundedSpaceTags(tag);
		}
		public Quad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) : base (new Vector3[]{v1,v2,v3,v4}.ToList()){
			
		}
		
	}
}
