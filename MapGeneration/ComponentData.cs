/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/23/2017
 * Time: 2:02 PM
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
	/// Description of ComponentData.
	/// </summary>
	public class ComponentData : MonoBehaviour 
	{
		public Bounds bounds {
			get { 
				return correctBoundsTransform();
			}
		}
		public BoxCollider boundingCollider;
		public List<GameObject> propSlots;
		public List<Edge> connections;
		
		Bounds correctBoundsTransform () {
			return boundingCollider.bounds;				
		}
	
		public List<Edge> correctedConnections (Vector3 pos){
		
			var l = new List<Edge>();

			foreach (Edge e in connections){
				var v1 = e.v1;
				var v2 = e.v2;
				l.Add(new Edge(v1 + pos,v2 + pos));
			}

			return l;
			
		}
	}
}
