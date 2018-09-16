/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 12/20/2017
 * Time: 9:31 PM
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

namespace MeleeCombat.MapGeneration
{
		
	using Random = System.Random;

	
	public class Cell {
		
		public bool visited;
		public Vector3 center;
		public List<Shape> walls;
		public HashSet<Edge>edges;
		public List<Vector3> neighbors;
		public bool occupied = false;
		public Bounds bounds;
		
		public static Random random = new Random();
		
		public List<Shape> findWallWithAttribute (FaceTypes t){
			return walls.FindAll(x => x.tag.tags.Contains(t));
			
		}
		
		#region Equals and GetHashCode implementation
		public override int GetHashCode()
		{
			int hashCode = 0;
				unchecked {
					hashCode += 1000000009 * center.GetHashCode();
				}
					return hashCode;
		}

		public override bool Equals(object obj)
		{
			Cell other = obj as Cell;
			if (other == null)
				return false;
			return this.center == other.center;
		}

		public static bool operator ==(Cell lhs, Cell rhs) {
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Cell lhs, Cell rhs) {
			return !(lhs == rhs);
		}

		#endregion
		
		public bool cellContainsAttribute (FaceTypes tag){
			return findWallWithAttribute(tag).Count > 0;
		}
		
		public Cell randomNeighbor (Dictionary<Vector3,Cell> cellMap){
			if (neighbors.Count == 0) return null;
			return cellMap[neighbors[random.Next(0,neighbors.Count)]];
		}
		
		public Cell (Vector3 v, HashSet<Vector3> pointsInRoom ) : this(v){
			for (int i = -1; i < 2;i++){
				for (int j = -1; j < 2;j++){
					var sum = Math.Abs(i) + Math.Abs(j);
					if (sum != 1) continue;
					var v2 = new Vector3(i,j,0) + v;
					if (pointsInRoom.Contains(v2)){
						neighbors.Add(v2);
					}
				}
			}
		}
		
		public void restoreFloor () {
			var v = center;
			var v0 = v + new Vector3(-.5f,-.5f,-.5f);
			var v1 = v0 + Vector3.right;
			var v2 = v0 + Vector3.up + Vector3.right;
			var v3 = v0 + Vector3.up;
			var quad = new Quad(v0,v1,v2,v3,FaceTypes.FLOOR);
			if (! walls.Contains(quad)){
				walls.Add(quad);
			}
			
		}
		
		public Cell (Vector3 v){
			visited = false;
			center = v;
			
			walls = new List<Shape>();
			edges = new HashSet<Edge>();
	
			var v0 = v + new Vector3(-.5f,-.5f,-.5f);
			var v1 = v0 + Vector3.right;
			var v2 = v0 + Vector3.up + Vector3.right;
			var v3 = v0 + Vector3.up;
			var v4 = v0 + Vector3.forward;
			var v5 = v1 + Vector3.forward;
			var v6 = v2 + Vector3.forward;
			var v7 = v3 + Vector3.forward;
			walls.Add(new Quad(v0,v1,v2,v3,FaceTypes.FLOOR));
			walls.Add(new Quad(v4,v5,v6,v7,FaceTypes.CEILING));
			
			
			walls.Add(new Quad(v0,v1,v5,v4,FaceTypes.WALL));
			walls.Add(new Quad(v1,v2,v6,v5,FaceTypes.WALL));
			walls.Add(new Quad(v2,v3,v7,v6,FaceTypes.WALL));
			walls.Add(new Quad(v3,v0,v4,v7,FaceTypes.WALL));
			
			
			edges.Add(new Edge(v0,v4,FaceTypes.PILLAR));
			edges.Add(new Edge(v1,v5,FaceTypes.PILLAR));
			edges.Add(new Edge(v2,v6,FaceTypes.PILLAR));
			edges.Add(new Edge(v3,v7,FaceTypes.PILLAR));
			
			neighbors  = new List<Vector3>();
			bounds = new Bounds(v,Vector3.one);
		}

		public void removeWall (Shape q){
			walls.Remove(q);
			foreach (Edge e in q.edges){
				if (walls.Find(x => x.edges.Contains(e)) == null){
					edges.Remove(e);
				}
				
			}
		}
		
		public List<Shape> removeWall (Cell other){
			var wallIntersection = other.walls.Intersect(walls).ToList();

			foreach (Shape q in wallIntersection){
				removeWall(q);
				other.removeWall(q);
			}

			return wallIntersection.ToList();
		}
		
	}
	
}
