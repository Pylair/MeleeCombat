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
	public class Triangle : Shape{

		
		public Triangle (Vector3 v1, Vector3 v2, Vector3 v3) : base (new Vector3[]{v1,v2,v3}.ToList()){
			//setUV(0,Vector2.zero);
			//setUV(1,new Vector2(1,0));
			//setUV(2,Vector2.one);
		}
	}
	
	
}