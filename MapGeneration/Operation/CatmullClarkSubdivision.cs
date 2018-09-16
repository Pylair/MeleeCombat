/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 9/10/2017
 * Time: 6:53 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/*namespace MeleeCombat.MapGeneration.Operation
{

	public static class CatmullClarkSubdivision
	{
	
		
		
		public static List<Shape> catmullClarkSubdivision (Shape shape) {

			Vertex facePoint = new Vertex( shape.center);
			facePoint.uv = shape.uvCenter();
			
			List<Vertex> list = new List<Vertex>();
			List<Edge> oldEdges = new List<Edge>();
			for (int i = 0; i <  shape.points.Count - 1;i++){
				var v =  shape.points[i];
				var n = v.connectedFaces.Count * 1.0f;

			
				var m1 = (n-3f)/n;
				var m2 = 1f/n;
				var m3 = 2f/n;
			
				var v2 = m1 * v.v +
					m2 * v.averageFacePoints() +
					m3 * v.averageCenterOfConnectedEdges();
				var distance = Vector3.Distance(v2,v.v);
	
				var vertex = new Vertex(v2);
				vertex.uv = v.uv;
				list.Add(vertex);
				
			}
			list.Add(list[0]);

			List<Edge> newEdges = new List<Edge>();
			for (int i = 0; i < list.Count - 1;i++){
				var edgePoint = createEdgePoints(shape,new Edge( shape.points[i], shape.points[i+1]));
				newEdges.Add(new Edge(list[i],edgePoint));
				newEdges.Add(new Edge(edgePoint,list[i+1]));
				
				oldEdges.Add(new Edge(shape.points[i],shape.points[i+1]));
			}
			return splice(facePoint,newEdges,oldEdges);
			
		}

		static List<Shape> splice (Vertex facePoint,List<Edge> newEdges,List<Edge> oldEdges) {
			List<Shape> shapes = new List<Shape>();
			for (int i = 0; i < newEdges.Count;i++){
				var e = newEdges[i];
				var t = new Triangle(facePoint,e.v2,e.v1);
				if (t.points.Count != 4) continue;
				Shape.unwrapTriangle(t);
				shapes.Add(t);
			}
			return shapes;
		}

		public static Vertex createEdgePoints (Shape shape,Edge edge) {
			var average = Vector3.zero;
			var count = 0;
		
			foreach (Shape s in shape.neighbors){
				if (s.edges.Contains(edge)){
					average += s.center;
					count++;
				}
			}

			average += shape.center;
			average/= (1 + count);
			average += edge.center();
			average/=2;
			
			var output = new Vertex(average);
			output.uv =(edge.v1.uv + edge.v2.uv)/2;
			return output;
		}
		
	}
}
*/