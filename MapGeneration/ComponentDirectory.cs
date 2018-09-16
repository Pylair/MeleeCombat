/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 9/16/2017
 * Time: 10:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MeleeCombat.MapGeneration
{
	/// <summary>
	/// Description of ComponentDirectory.
	/// </summary>
	public class ComponentDirectory : MonoBehaviour
	{
		public List<GameObject> definitions;
		public GameObject defaultComponent;
		public List<FaceTypes> faceTagList;
		public List<Vector3> dimensions;
		
		public Vector3 scaleFactor = new Vector3(1,1,3);
	
		public static System.Random r = new System.Random();
		
		public GameObject getComponent(bool instantiate){
			GameObject component = definitions[r.Next(0,definitions.Count)];
			if (instantiate){
				component= Instantiate(component);
			}
			return component;
		}
		
		
		/*public GameObject searchForBestDimensionfit (Vector3 dimension){
			if (definitions.Count < 2) return defaultComponent;
			var list = new List<GameObject>(definitions);
			list = list.OrderBy(x => differenceInDimensions(dimension,dimensions[definitions.IndexOf(x)])).ToList();
			return list[0];
		}
		
		public float differenceInDimensions (Vector3 a, Vector3 b){
			a = new Vector3(Math.Abs(a.x),Math.Abs(a.y),Math.Abs(a.z));
			b = new Vector3(Math.Abs(b.x),Math.Abs(b.y),Math.Abs(b.z));
			var c = a - b;
			
			c = Vector3.Scale(c,scaleFactor);
			return c.magnitude;
		}*/
		
		
		
		
	}
}
