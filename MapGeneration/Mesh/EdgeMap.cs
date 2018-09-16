/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 9/22/2017
 * Time: 10:41 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MeleeCombat.MapGeneration
{
	 
	public class EdgeMap 
	{
		public Dictionary<Vector3,List<Edge>> edgeMap;

		public List<Edge> this [Vector3 index]{
			get {
				return edgeMap[index];
			} set {
				edgeMap[index] = value;
			}
		}
		
		public HashSet<Edge> edges {
			get {
				var output = new HashSet<Edge>();
				foreach (List<Edge> l in edgeMap.Values){
					output.UnionWith(l);
				}
				return output;
			}
		}
		
	/*	public IEnumerator<Edge> GetEnumerator (){
			yield return edgeMap.Values.GetEnumerator()
		}*/
		
		public EdgeMap (IEnumerable<Edge> edges) : this(){
			foreach (Edge e in edges){
				if (! edgeMap.ContainsKey(e.v1)) edgeMap[e.v1] = new List<Edge>();
				if (! edgeMap.ContainsKey(e.v2)) edgeMap[e.v2] = new List<Edge>();
				edgeMap[e.v1].Add(e);
				edgeMap[e.v2].Add(e);
			}
		}
		
		public EdgeMap (){
			edgeMap = new Dictionary<Vector3, List<Edge>>();
		}
		
		public void addElement (Edge e){
			addVertex(e.v1,e);
			addVertex(e.v2, e);
		}
		
		
		public void addVertex (Vector3 v,Edge e){
			if (! edgeMap.ContainsKey(v)) {
				edgeMap[v] = new List<Edge>();
			} 
			var list = edgeMap[v];
			if (list.Contains(e))return;
			list.Add(e);
	
		}
		
		

		public static EdgeMap constructEdgemapFromPoints (IEnumerable<Vector3> input){
			var edgeMap = new EdgeMap();
			var edges = new HashSet<Edge>();
			foreach (Vector3 v in input){
				foreach (Vector3 v2 in VoxelIterators.VonNeumanNeighbors3D()){
					var v3 = v + v2;
					if (input.Contains(v3)) {
						edges.Add(new Edge(v,v3));
					}
				}
			}
			
			foreach (Edge e in edges){
				edgeMap.addElement(e);
			}
			return edgeMap;
		}
		
	}
}
