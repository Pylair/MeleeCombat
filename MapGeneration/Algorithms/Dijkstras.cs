/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 10/25/2017
 * Time: 3:30 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Priority_Queue;



namespace MeleeCombat.MapGeneration.Algorithms
{
	
	public class EdgeNode : Priority_Queue.FastPriorityQueueNode {
		public Edge e;
		
		public EdgeNode (Edge e){
			this.e = e;
		}
	}
	
	public class VectorNode : Priority_Queue.FastPriorityQueueNode {
		public Vector3 v;
		
		public VectorNode (Vector3 v){
			this.v = v;
		}
	}
	
	public class Dijkstras : MonoBehaviour
	{
		public bool trigger = false;
	
		public List<Vector3> entrances;
		public Vector3 start;
		public Vector3 end;

		public Vector3 size;
		public Vector3 min;
		public int c;

		
		
		
		IEnumerator<WaitForSeconds> test() {
			var r = new System.Random();
			var set1 = new List<Vector3>();
			var black = new HashSet<Vector3>();
			var white = new List<Vector3>();
			Dictionary<Vector3,GameObject> map = new Dictionary<Vector3, GameObject>();
			for (int i = 0; i < size.x;i++){
				for (int j = 0; j < size.y;j++){
					for (int k = 0; k < size.z;k++){
						var v0 = new Vector3(i,j,k);
						set1.Add(v0);
						map[v0] = GameObject.CreatePrimitive(PrimitiveType.Cube);
						map[v0].transform.position = v0;
					}
				}
			}
			
			
			var set2 = new List<Vector3>(set1);	
			var edgemap = EdgeMap.constructEdgemapFromPoints(set1);
			var v = set1[r.Next(0,set1.Count)];
			set1.Remove(v);
			white.Add(v);
			
			while (set1.Count > 0){
				
				var v1 = set1[r.Next(0,set1.Count)];
				set1.Remove(v1);
				bool dontSearch = false;
				foreach (Vector2 x in VoxelIterators.VonNeumanNeighbors()){
					if (white.Contains(v1 + (Vector3)x)){
						dontSearch = true;
					}
				}
				if (dontSearch) continue;
				Vector3 v2 = white[r.Next(0,white.Count)];
				
		
				
				var path = Dijkstras.ASTAR(edgemap,v1,v2);
		
				for (int j = 0; j < path.Count -1;j++){
					var x1 = path[j];
					var x2 = path[j + 1];
					var e0 = edgemap.edgeMap[x1].Intersect(edgemap.edgeMap[x2]).ToList()[0];
					
					e0.weight = .1f;
					
					var v3 = path[j];
					set1.Remove(v3);
					white.Add(v3);
					black.Remove(v3);
					map[v3].GetComponent<MeshRenderer>().material = whiteMat;
					
					foreach (Vector3 v4 in VoxelIterators.VonNeumanNeighbors()){
						var v5 = v4 + v3;
						set1.Remove(v5);
						if (! white.Contains(v5) && set2.Contains(v5)){
							black.Add(v5);
							set1.Remove(v5);
							map[v5].GetComponent<MeshRenderer>().material = blackMat;
						}
						
						
					}
					yield return new WaitForSeconds(.05f);
				}
				
			}
		}
		
		
		
		public Material blackMat;
		public Material whiteMat;
		
			
		int count (Vector3 v, HashSet<Vector3> points){
			int i = 0;
			foreach (Vector3 v2 in VoxelIterators.VonNeumanNeighbors()){
				var v3 = v + v2;
				if (points.Contains(v3)) i++;
			}
			return i;
		}
		
		
		public void Update () {
			if (trigger){
				trigger = false;
				
				/*var rects = new List<Rect>();
				new Subdivison(3,3).subdivide(9,new Rect(0,0,60,60),rects);
				*/
				var vq = Vector3.right;
					for (int i = 0; i < 4;i++){
						vq = Quaternion.AngleAxis(90,Vector3.forward) * vq;
						Debug.Log(vq);
					}
				//StartCoroutine(test());
				
			}
		}
		
		public static float distance (float weight,Vector3 v1, Vector3 v2){
			return weight * (Math.Abs(v2.x - v1.x) + Math.Abs(v2.y - v1.y) + Math.Abs(v2.z - v1.z));
		}
		
		public static List<Vector3> dijkstras (EdgeMap edgeMap, Vector3 start, Vector3 end){
			var input = edgeMap.edgeMap.Keys.ToList();
			var queue = new SimplePriorityQueue<Vector3>();
			
			if (! edgeMap.edgeMap.ContainsKey(start) || ! edgeMap.edgeMap.ContainsKey(end)) return new List<Vector3>();
			
			Dictionary<Vector3,Vector3> prev = new Dictionary<Vector3, Vector3>();
			Dictionary<Vector3,float> vertexWeightMap = new Dictionary<Vector3, float>();
			
			
			foreach (Vector3 v in input){
				vertexWeightMap[v] = float.PositiveInfinity;
			}
			vertexWeightMap[start] = 0;
			
			Vector3 current = start;
			queue.Enqueue((current),0);
			
			var a = 0;
			var b = 5000;
			
			while (input.Any()){
				
				a++;
				if (a > b) {
					Debug.Log("NO PATH FOUND " + a);
					return new List<Vector3>();
				}
				
				
				var p = current;
				current = queue.Dequeue();
				var neighbors = edgeMap.edgeMap[current];
				input.Remove(current);
				
				
				if (current.Equals(end)) {
					break;
				}
				
				foreach (Edge e in neighbors){
					var v = e.v1;
					if (v.Equals(current)) v = e.v2;
					
					if (! input.Contains(v)){
						continue;
					}
					var d = (distance(e.weight,e.v1,e.v2) + vertexWeightMap[current]);
					
					if (d < vertexWeightMap[v]){
						
						vertexWeightMap[v] = d;
						queue.Enqueue(v,d);
						prev[v] = current;
					}
				}		
			}
			
			
			
			a = 0;
			List<Vector3> shortestPath = new List<Vector3>();
			Vector3 previous = end;
			shortestPath.Add(previous);
			while (previous != start){
				a++;
				if (b < a) {
					Debug.Log("NOO PATH FOUND");
					return new List<Vector3>();
				}
				
				previous = prev[previous];
				shortestPath.Add(previous);
		
			}
			
			return shortestPath;
			
		}
		
		public static List<Vector3> ASTAR_With_Weighted_Turns (EdgeMap edgeMap, Vector3 start, Vector3 end){
			var input = edgeMap.edgeMap.Keys.ToList();
			var queue = new SimplePriorityQueue<Vector3>();	
			
			Dictionary<Vector3,Vector3> prev = new Dictionary<Vector3, Vector3>();
			Dictionary<Vector3,float> vertexWeightMap = new Dictionary<Vector3, float>();
			
			if (! edgeMap.edgeMap.ContainsKey(start) || ! edgeMap.edgeMap.ContainsKey(end)) return new List<Vector3>();
			
			foreach (Vector3 v in input){
				vertexWeightMap[v] = float.PositiveInfinity;
			}
			vertexWeightMap[start] = 0;
			
			Vector3 current = start;
			queue.Enqueue((current),0);
			
			var a = 0;
			var b = 5000;
			Vector3 direction = Vector3.zero;
			
			while (input.Any()){
				
				a++;
				if (a > b) {
					Debug.Log("NO PATH FOUND " + a);
					return new List<Vector3>();
				}
				
				
				var p = current;
				current = queue.Dequeue();
				direction = current - p;
				var neighbors = edgeMap.edgeMap[current];
				input.Remove(current);
				
				
				if (current.Equals(end)) {
					break;
				}
				
				foreach (Edge e in neighbors){
					var v = e.v1;
					if (v.Equals(current)) v = e.v2;
					
					if (! input.Contains(v)){
						continue;
					}
					var angle = (Vector3.Angle(e.v2-e.v1, direction) / 90) + .5f;
					
					var d =  distance(angle* e.weight,end,v) - (distance(angle * 1/e.weight,e.v1,e.v2) + vertexWeightMap[current]);
					if (d < vertexWeightMap[v]){
						
						vertexWeightMap[v] = d;
						queue.Enqueue(v,d);
						prev[v] = current;
					}
				}		
			}
			
			
			
			a = 0;
			List<Vector3> shortestPath = new List<Vector3>();
			Vector3 previous = end;
			shortestPath.Add(previous);
			while (previous != start){
				a++;
				if (b < a) {
					Debug.Log("NOO PATH FOUND");
					return new List<Vector3>();
				}
				previous = prev[previous];
				shortestPath.Add(previous);
		
			}
			
			return shortestPath;
		}
		
		public static List<Vector3> ASTAR (EdgeMap edgeMap, Vector3 start, Vector3 end){
			var watch = System.Diagnostics.Stopwatch.StartNew();
			var input = edgeMap.edgeMap.Keys.ToList();
			var queue = new SimplePriorityQueue<Vector3>();	
			
			Dictionary<Vector3,Vector3> prev = new Dictionary<Vector3, Vector3>();
			Dictionary<Vector3,float> vertexWeightMap = new Dictionary<Vector3, float>();
			
			if (! edgeMap.edgeMap.ContainsKey(start) || ! edgeMap.edgeMap.ContainsKey(end)) return new List<Vector3>();
			
			foreach (Vector3 v in input){
				vertexWeightMap[v] = float.PositiveInfinity;
			}
			vertexWeightMap[start] = 0;
			
			Vector3 current = start;
			queue.Enqueue((current),0);
			
			var a = 0;
			var b = 5000;
			
			while (input.Any()){
				
				a++;
				if (a > b) {
					Debug.Log("NO PATH FOUND " + a);
					return new List<Vector3>();
				}
				
				
				var p = current;
				current = queue.Dequeue();
				var neighbors = edgeMap.edgeMap[current];
				input.Remove(current);
				
				
				if (current.Equals(end)) {
					break;
				}
				
				foreach (Edge e in neighbors){
					var v = e.v1;
					if (v.Equals(current)) v = e.v2;
					
					if (! input.Contains(v)){
						continue;
					}
					//var d = (distance(e.weight,e.v1,e.v2) + vertexWeightMap[current]);
					
					var d =  distance(e.weight,end,v) - (distance(1/e.weight,e.v1,e.v2) + vertexWeightMap[current]);
					if (d < vertexWeightMap[v]){
						
						vertexWeightMap[v] = d;
						queue.Enqueue(v,d);
						prev[v] = current;
					}
				}		
			}
			
			
			
			a = 0;
			List<Vector3> shortestPath = new List<Vector3>();
			Vector3 previous = end;
			shortestPath.Add(previous);
			while (previous != start){
				a++;
				if (b < a) {
					Debug.Log("NOO PATH FOUND");
					return new List<Vector3>();
				}
				
				previous = prev[previous];
				shortestPath.Add(previous);
		
			}
			
			return shortestPath;
			
		}
		
	/*	public static List<Vector3> run (IEnumerable<Vector3> points, Vector3 start, Vector3 end){
			

			if (! points.Contains(start) || ! points.Contains(end)){
				Debug.Log(start + " or " + end + " not present");
				return new List<Vector3>();
			}
			var edgeMap = EdgeMap.constructEdgemapFromPoints(points);
			return execute (edgeMap,start,end);

		}*/
		

	}
}
