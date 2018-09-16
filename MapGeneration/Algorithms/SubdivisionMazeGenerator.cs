/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 9/17/2017
 * Time: 6:16 PM
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
	using Random = System.Random;
	public class SubdivisionMazeGenerator
	{
		
		int x;
		int y;
		Random random = new Random();
		public HashSet<Edge> openings = new HashSet<Edge>();
		Dictionary<Vector3,BoundedSpace> abstractMap = new Dictionary<Vector3, BoundedSpace>();
		public HashSet<Bounds> outputBounds = new HashSet<Bounds>();
	
		
		public static void drawEdge (Edge e,Color c){
			var v1 = e.v1; var v2 = e.v2;
			var p0 = new Vector3(v1.x,v1.y,v1.z);
			var p1 = new Vector3(v2.x,v2.y,v2.z);
			Debug.DrawLine(p0,p1,c,float.PositiveInfinity,false);
		}
		
		Polygon envelopeToGeometry (Envelope e){
			var c0 = new Coordinate(e.MinX,e.MinY,0);
			var c1 = new Coordinate(e.MaxX,e.MinY,0);
			var c2 = new Coordinate(e.MaxX,e.MaxY,0);
			var c3 = new Coordinate(e.MinX,e.MaxY,0);
			return new Polygon(new LinearRing(new Coordinate[]{c0,c1,c2,c3,c0}));
		}
		
		
		public List<Edge> divide (int x, int y, int w,int h, int iterations){
			var b = new Bounds(new Vector3(x + w/2, y + h/2,0), new Vector3(w,h,0));
			var edges = new Quad(new Vector3(x,y,0), new Vector3(x + w,y,0), new Vector3(x + w, y + h,0), new Vector3(x,y + h,0)).edges;
			divide(edges,b,iterations);
			return edges;
		}
	
		public void divide (List<Edge> edges, Bounds e, int iterations){
			if (iterations <= 0 || e.size.x <= 3 || e.size.y <= 3) {
				outputBounds.Add(e);
				return;
			}
			
			var bisectingEdge = getBisectingEdge(e);
			var e1 = new Bounds(e.min,Vector3.zero);
			var e2 = new Bounds(e.max,Vector3.zero);
			e1.Encapsulate(bisectingEdge.v1);
			e1.Encapsulate(bisectingEdge.v2);
			e2.Encapsulate(bisectingEdge.v1);
			e2.Encapsulate(bisectingEdge.v2);
			
			
			if (e1.size.x < 3 || e2.size.y < 3 || e2.size.x < 3 || e2.size.y < 3) {
				outputBounds.Add(e);
				return;
				
			}
			
			
			edges.Add(bisectingEdge);
			
			cutOpening(bisectingEdge);
			
			divide(edges,e1,iterations -1);
			divide(edges,e2,iterations -1);
			
		}
		
		
		
		
		List<Edge> cutOpening(Edge e){
			List<Edge> list = new List<Edge>();
			if (e.length < 2) {
				
				return list;
			}
			var slope = e.slope;
			var p0 = e.v1;
			
			var length = random.Next((int)(e.length * .25), (int)(e.length * .75));
			var p1 = p0 + (length - 1) * slope;
			var p2 = p0 + (length + 1) * slope;
			var p3 = e.v2;
			list.Add(new Edge(p0,p1));
			list.Add(new Edge(p2,p3));
			openings.Add(new Edge(p1, p2));
			
			return list;
		}
		
		Vector3[] directions = new Vector3[]{Vector3.right,Vector3.left,Vector3.up,Vector3.down};

	
		Edge getBisectingEdge (Bounds b){

			
			var xDivision = (int)(random.Next((int)(.4f *b.size.x),(int)(.6f *b.size.x)) + b.min.x);
			var yDivision = (int)(random.Next((int)(.4f *b.size.y),(int)(.6f *b.size.y)) + b.min.y);

			Edge edge;
			
			if (b.size.x > 2 * b.size.y){
				edge = new Edge(new Vector3(xDivision,b.min.y,0), new Vector3(xDivision,b.max.y,0));
			
			} else if (b.size.y > 2 * b.size.x){
				edge = new Edge(new Vector3(b.min.x,yDivision,0), new Vector3(b.max.x,yDivision,0));
			
			} else if (random.NextDouble() < .5f ){
				edge = new Edge(new Vector3(xDivision,b.min.y,0), new Vector3(xDivision,b.max.y,0));
			} else {
				edge = new Edge(new Vector3(b.min.x,yDivision,0),  
				                new Vector3(b.max.x,yDivision,0));
			}

			return edge;
		}

		
		int neighborCount (Vector3 v,HashSet<Vector3> points){
			int count = 0;
			for (int i = -1; i < 2;i++){
				for (int j = -1; j < 2;j++){
					var sum = Math.Abs(j) + Math.Abs(i);
					if (sum != 1) continue;
					if ( points.Contains(new Vector3(i,j,0) + v)){
						count++;
					}
				}
			}
			return count;
		}

	}

}
