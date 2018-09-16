/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 9/4/2017
 * Time: 1:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace MeleeCombat.MapGeneration
{
	

	


	[Serializable] 
	public class Edge : BaseShape
	{
		public static System.Random random = new System.Random();
		public static List<Edge> straightenEdge (Edge e){
			var edges = new List<Edge>();
		
			/*var v2 = new Vector3(e.v2.x,e.v1.y);
			if (random.NextDouble() < .5){
				v2 = new Vector3(e.v1.x,e.v2.y);
			}
			var e1 = new Edge(e.v1,v2);
			var e2 = new Edge(v2,e.v2);	
			edges.Add(e1);edges.Add(e2);
			return edges;*/
			
			edges.Add(new Edge(e.v1,new Vector3(e.v1.x,e.v1.y,e.v2.z)));
			edges.Add(new Edge(new Vector3(e.v1.x,e.v1.y,e.v2.z),new Vector3(e.v1.x,e.v2.y,e.v2.z)));
			edges.Add(new Edge(new Vector3(e.v1.x,e.v2.y,e.v2.z), e.v2));

			edges = edges.FindAll(x => x.length > 0).ToList();
			return edges;
			
			
		}
		
		public Vector3 v1 {
			get {
				return vertices[0];
			}
		}
		public Vector3 v2 {
			get {
				return vertices[1];
			}
		}
		
		public float length {
			get {
				return (v2 - v1).magnitude;
			}
		}
		public Vector3 slope {
			get {
				return (v2 - v1).normalized;
			}
		}
		
		public float weight {
			get;set;
		}

		public override Vector3 center () {
			return (v1 + v2)/2;
		}
		
		public Edge (Vector3 p1, Vector3 p2) : base(){
		
			vertices.Add(p1);
			vertices.Add(p2);
			init();
			weight = 1;
		}
		
		public Edge (Vector3 p1, Vector3 p2,FaceTypes t) : this(p1,p2){
			tag = new BoundedSpaceTags(t);

		}
		

		public override string ToString()
		{
			return string.Format("[Edge V1={0}, V2={1}]", v1, v2);
		}

		public bool pointLiesOnLine (Vector3 v){
			var d1 = (v2 - v1);
			var d2 = (v - v1);
			if (Vector3.Cross(d1,d2) != Vector3.Cross(d2,d1)) return false;
			var bounds = new Bounds(v1,Vector3.zero);
			bounds.Encapsulate(v2);
			return bounds.Contains(v);
		}
		
		public bool intersect (Edge other, out Vector3 intersection){
			var slope1 = v2 - v1;
			var slope2 = other.v2 - other.v1;
			var p1 = v1;
			var p2 = other.v1;
			if (Math.Abs(Vector3.Dot(slope1,slope2)) > .90){
				intersection = Vector3.zero;
				return false;
				
			}
			
			
			var cross1 = Vector3.Cross((p2 - p1),slope2);
			var cross2 = Vector3.Cross(slope1,slope2);
			
			var a = cross1.magnitude/cross2.magnitude;
			if (a.Equals(float.NaN)){
				intersection = Vector3.zero;
				return false;
			}
			if (Math.Sign(Vector3.Dot(cross1,cross2)) == -1){
				a = -a;
			}
		
			intersection =  p1  + a * slope1;	
			
			
			if (!pointLiesOnLine(intersection) ||! other.pointLiesOnLine(intersection)) return false;
			
			return true;
				
				
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			Edge other = obj as Edge;
				if (other == null)
					return false;
				return (this.v1 == other.v1 && this.v2 == other.v2) || (this.v1 == other.v2 && this.v2 == other.v1) ;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 10000007  * v1.GetHashCode();
				hashCode += 10000007 * v2.GetHashCode();
			}
			return hashCode;
		}

		public static bool operator ==(Edge lhs, Edge rhs) {
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Edge lhs, Edge rhs) {
			return !(lhs == rhs);
		}

		#endregion
	}
	
	public class  Node : BaseShape{
		
		public new HashSet<Node> neighbors;
		
		public Node (Vector3 v) : base(){
			neighbors = new HashSet<Node>();
			vertices.Add(v);
		}
		
		public void setNeighbor(Node other)
		{
			neighbors.Add(other);
		}
		
		public override Vector3 center()
		{
			return vertices.First();
		}
		
		
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			Node other = obj as Node;
				if (other == null)
					return false;
				return vertices.First().Equals(other.vertices.First());
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (neighbors != null)
					hashCode += vertices.First().GetHashCode();
			}
			return hashCode;
		}

		public static bool operator ==(Node lhs, Node rhs) {
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Node lhs, Node rhs) {
			return !(lhs == rhs);
		}

		#endregion
		
	}
	
	public class BaseShape {
		
		public List<Vector3> vertices {get ;internal set;}
		public HashSet<BaseShape> neighbors {get ;internal set;}
		public BoundedSpaceTags tag {get ;internal  set;}
		public HashSet<BaseShape> groupMembers {get ;internal  set;}

		
		public BaseShape (){
			vertices = new List<Vector3>();
			neighbors = new HashSet<BaseShape>();
			tag = new BoundedSpaceTags(FaceTypes.IGNORE);
			groupMembers = new HashSet<BaseShape>();
			
		}
		
		public void init () {
			groupMembers.Add(this);
		}
		
		public virtual void setNeighbor (BaseShape other){
			neighbors.Add(other);
			other.neighbors.Add(this);
		}
		
		public virtual bool touches (BaseShape other){
			return false;
		}
		
		public virtual Vector3 center (){
			return Vector3.zero;
		}
		
	}
	
	/*public class NTSUtil {

		
		public static Shape convertPolygonToShape (Polygon polygon){
			var ring = polygon.ExteriorRing.Coordinates;
			var simplifier = new NetTopologySuite.Simplify.DouglasPeuckerLineSimplifier(ring);
			ring = simplifier.Simplify();
			
			List<Vector3> vertices = new List<Vector3>();
			for (int i = 0; i < ring.Count() -1;i++){
				vertices.Add(ctv(ring[i],false));
			}
			
			var shape = new Shape(vertices);
			return shape;
		}
		
		public static Polygon convertShapeToPolygon (Shape shape){
		
			
			List<Coordinate> coords = new List<Coordinate>();
			for (int i = 0; i < shape.vertices.Count();i++){
				coords.Add(vtc(shape.vertices[i].v,false));
			}
			
			return new Polygon(new LinearRing(coords.ToArray()));
		}
	
		public static Vector3 ctv (Coordinate c, bool round){
			if (round) return new Vector3((int)c.X,(int)c.Y,0);
			return new Vector3((float)c.X,(float)c.Y,(float)c.Z);
		}
		
		public static Coordinate vtc (Vector3 v,bool round){
			if (round) return new Coordinate((int)v.x,(int)v.y,(int)v.z);
			return new Coordinate((double)v.x,(double)v.y,(double)v.z);
		}
		
	}*/
	
}
	