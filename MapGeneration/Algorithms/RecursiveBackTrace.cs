/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 9/30/2017
 * Time: 11:20 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using NetTopologySuite.Geometries;
using GeoAPI.Geometries;
using NetTopologySuite.Operation;
using System.Collections.Generic;
using System.Linq;

namespace MeleeCombat.MapGeneration.Algorithms
{
	
	

	public class RecursiveBackTrace
	{

		
		void recurse (HashSet<Edge> visitedPairs,HashSet<Vector3> unvisitedCells, Dictionary<Vector3,Room> dict, Vector3 current, Vector3 previous, Dictionary<Vector3,Room> pairs){
			if (! unvisitedCells.Contains(current)) return;
			unvisitedCells.Remove(current);
				
			var r1 = dict[previous];
			var r2 = dict[current];
			if (! r1.Equals(r2)){		
				var e1 = new Edge(r1.bounds.center,r2.bounds.center);
				if (! visitedPairs.Contains(e1)){
					visitedPairs.Add(e1);
					pairs[e1.v1] = r1;
					pairs[e1.v2] = r2;	
				}
			}
			
			foreach (Vector3 v in VoxelIterators.VonNeumanNeighbors3D()){
				var v2 = current + v;
				recurse(visitedPairs,unvisitedCells,dict,v2, current,pairs);
			}
			
		}
		
		public void recursiveBackTrace (Dictionary<Vector3,Room> dict){
			
			var unvisitedCells = new HashSet<Vector3>(dict.Keys);
			
			Vector3 current = unvisitedCells.First();
			
			Dictionary<Vector3,Room> pairs = new Dictionary<Vector3,Room>();
			HashSet<Edge> visitedPairs = new HashSet<Edge>();
			recurse(visitedPairs,unvisitedCells,dict,current,current,pairs);
			var prims = new Prims().execute(visitedPairs);
			foreach (Edge e in prims){
				var r1 = pairs[e.v1];
				var r2 = pairs[e.v2];
				Room.carveBetweenRooms(r1,r2);
			}
			
		}

		
	}
}
