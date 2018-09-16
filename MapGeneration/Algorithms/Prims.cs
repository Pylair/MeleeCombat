/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 9/30/2017
 * Time: 6:10 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace MeleeCombat.MapGeneration.Algorithms
{

	
	public class Prims{
		
		
		Dictionary<Vector3,List<Edge>> dict;
		HashSet<Vector3> addedVertices;
		List<Edge> output;
		
		public IEnumerable<Edge> execute (IEnumerable<Edge> edges){
	
			if (! edges.Any()) return edges;
			
			EdgeMap map = new EdgeMap();
			foreach (Edge e in edges){
				map.addElement(e);
			}
			dict = map.edgeMap;
			var vertex = map.edgeMap.Keys.First();
			var neighbors = dict[vertex];
			
			List<Edge> currentNeighbors = new List<Edge>();
		 	addedVertices = new HashSet<Vector3>();
			output = new List<Edge>();
			
			addedVertices.Add(vertex);
			currentNeighbors.AddRange(neighbors);
			currentNeighbors = currentNeighbors.OrderBy(x => x.length).ToList();
			
			var first = currentNeighbors.First();
			output.Add(first);
			currentNeighbors.RemoveAt(0);
			currentNeighbors = updateNeighbors(first.v1,currentNeighbors);
			currentNeighbors =updateNeighbors(first.v2,currentNeighbors);
			output.Add(currentNeighbors.First());
		
			int stop = 3000000;
			int i = 0;
			while (addedVertices.Count < dict.Keys.Count){
				i++;
				if (i > stop) {
					Debug.Log("STOPPING");
					break;
				}
				first = currentNeighbors.First();
				currentNeighbors.RemoveAt(0);
				if (addedVertices.Contains(first.v1) && addedVertices.Contains(first.v2)) continue;
				output.Add(first);
				currentNeighbors =updateNeighbors(first.v1,currentNeighbors);
				currentNeighbors =updateNeighbors(first.v2,currentNeighbors);
			
			}
			
			return output;
		}
		
		List<Edge> updateNeighbors (Vector3 v, List<Edge> currentNeighbors){
			
			currentNeighbors.AddRange(dict[v]);
			currentNeighbors = new HashSet<Edge>(currentNeighbors).ToList();
	
			currentNeighbors = currentNeighbors.OrderBy(x => x.length).ToList();

			addedVertices.Add(v); 
			return currentNeighbors;
			
			
		}
		
	}
}
