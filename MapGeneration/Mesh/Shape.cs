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
	public class Shape : BaseShape{
		
		
		
		
		[NonSerialized]
		public List<Edge> edges;
		HashSet<Vector3> vertexHash;
		
		public Vector2 groupUVSize () {
			Vector2 size = Vector2.zero;
			foreach (Shape s in groupMembers){
				size += s.getUVSize();
			}
			return size;
		}
		
		public Bounds bounds;
		
		public Bounds groupBounds {
			get {
				if (groupMembers.Count > 0){
					var b = bounds;
					foreach (Shape s in groupMembers){
						b.Encapsulate(s.bounds);
					}
					return b;
				} else {
					return bounds;
				}
				
			}
		}
		
		public Vector3 normal {
			get {
				
				
				return Vector3.Cross(vertices[2] -vertices[0],vertices[1] - vertices[0]);
			
			
			}
		}
		
		public Vector3 groupNormal {
			get {
				if (groupMembers.Count > 0){
					var n = Vector3.zero;
					foreach (Shape s in groupMembers){
						n += s.normal;
					}
					n = n.normalized;
					return n;
				} else {
					return normal;
				}
				
			}
		}
		
		new public Vector3 center {
			get {
				var average = Vector3.zero;
				for (int i = 0; i < vertices.Count -1;i++){
					average += vertices[i];
				}
				return average/(vertices.Count - 1);
			}
		}
		
		public Vector3 groupCenter {
			get {
				if (groupMembers.Count > 0){
					var c = Vector3.zero;
					foreach (Shape s in groupMembers){
						c += s.center;
					}
					c /= groupMembers.Count;
					return c;
				} else {
					return center;
				}
				
			}
		}
		
		public IEnumerable<Edge> getGroupEdges () {
			var groupEdges = new HashSet<Edge>();
			var nonUniques = new HashSet<Edge>();
			foreach (Shape s2 in groupMembers){
				foreach (Edge e in s2.edges){
					if (groupEdges.Contains(e)) nonUniques.Add(e);
					groupEdges.Add(e);
				}
			}
			return (groupEdges.Except(nonUniques));
		}
		
		
		public void getUVDirections (out Vector3 u, out Vector3 v) {
			var forward = normal;
			var right = Vector3.right;
			var up = Vector3.Cross(forward,right);
			
			var dot = Vector3.Dot(forward,Vector3.right);
			if (Mathf.Approximately(Math.Abs(dot),1)){
				up = Vector3.up;
				right = Vector3.forward;
			}
			u = right;
			v = up;
		}
		
		public Vector2 getUVSize () {

			Vector3 right = Vector3.zero;
			Vector3 up = Vector3.up;
			getUVDirections(out right,out up);
			return new Vector2(Vector3.Scale(bounds.size, right).magnitude, Vector3.Scale(bounds.size,up).magnitude);
			
		}

		public Shape (List<Edge> edgeInput) : base(){
			
			this.edges = edgeInput;
			vertexHash = new HashSet<Vector3>();
			bounds = new Bounds(edgeInput[0].v1,Vector3.zero);
			for (int i = 0; i < edgeInput.Count;i++){
				var v1 = edgeInput[i].v1;
				var v2 = edgeInput[i].v2;
				
				
				hash += calculateHash(v1);
				hash += calculateHash(v2);
				
				               
			}
			init();
		}
		
		
		int calculateHash (Vector3 v1){
			int i = 0;
			i+=(int) (494395*v1.x);
			i += (int)(8793000*v1.y);
			i += (int)(2000004*v1.z);
			bounds.Encapsulate(v1);
			vertexHash.Add(v1);
			return i;
		}
		
		public Shape(List<Vector3> input) : base(){
			this.vertices = new List<Vector3>(input);
			vertexHash = new HashSet<Vector3>();
			if (! input.Last().Equals(input.First())){
				this.vertices.Add(input[0]);
			}
	
			bounds = new Bounds(vertices[0],Vector3.zero);
			for (int i = 0; i < vertices.Count -1;i++){
				var v1 = vertices[i];
				hash += calculateHash(v1);
			}

			edges = new List<Edge>();
			for (int i= 0; i < vertices.Count - 1;i++){
				edges.Add(new Edge(vertices[i],vertices[1 + i]));
			}
			init();
		}
		
		public bool comparevertices (HashSet<Vector3> p1, HashSet<Vector3> p2){
			foreach (Vector3 v in p1){
				if (! p2.Contains(v)) return false;
			}
			return true;
		}

		
		public float area () {
			double ar = 0;
			for (int i = 0; i < vertices.Count -1;i++){
				var a = (vertices[1 + i] - vertices[i]).magnitude;
				var b = (vertices[i] - center).magnitude;
				var c = (center - vertices[1+i]).magnitude;
				
				var p = (a + b + c)/2;
				var x = p * (p -a) * (p - b) * (p - c);
				ar += Math.Sqrt((double)x);
			}
			return (float)ar;
		}

		public override bool Equals(object obj)
		{
			Shape other = obj as Shape;
				if (other == null)
					return false;
						if (this.vertices.Count != other.vertices.Count) return false;
						if (! this.bounds.Contains(other.bounds.center)) return false;
						return comparevertices(this.vertexHash, other.vertexHash);
		}
		
		readonly int hash = 0;

		public override int GetHashCode()
		{
			return hash;
		}

		public static bool operator ==(Shape lhs,Shape rhs) {
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Shape lhs, Shape rhs) {
			return !(lhs == rhs);
		}
		
		public override string ToString()
		{
			var s=  "Shape: ";
			foreach (Vector3 v in vertices){
				s += v;
			}
			return s;
		}

	}
	
}