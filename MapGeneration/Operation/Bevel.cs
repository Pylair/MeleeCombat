/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 9/10/2017
 * Time: 7:00 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using MeleeCombat.MapGeneration;
using UnityEngine;
using System.Collections.Generic;
using MIConvexHull;
/*
namespace MeleeCombat.MapGeneration.Operation
{
	

	
	public class Bevel
	{
		public static void addToShapes (List<Shape> shapes,Shape shape){
			if (shape.area() > .01){
				shapes.Add(shape);
			}
		}
		
		
		public static Shape construct (List<Vertex> input){
			List<Vertex> output = new List<Vertex>();
			output.Add(input[input.Count -1]);
			Vertex current = output[0];
			input.RemoveAt(input.Count -1);
			while (input.Count > 0){
				float minDistance = float.PositiveInfinity;
				Vertex closest = input[0];
				
				for (int i = 0; i < input.Count;i++){
					var v = input[i];

					var distance = Vector3.Distance(v.v,current.v);
					if (minDistance > distance){
						minDistance = distance;
						closest = v;
					}
					
				}
				
				input.RemoveAll(closest.Equals);
				current = closest;
				output.Add(closest);
			}
			
			return new Shape(output);
		}
		
	
		
		static Vector3 slideVertex (Vector3 center, Vector3 v, float distance){
			var direction = (v - center);
			var newDistance = direction.magnitude - distance;
			if (newDistance <= 0){
				direction *= 0;
			} else {
				direction *= newDistance/direction.magnitude;
			}
			return direction + center;
		}
		
		public static List<Vertex> getShrunkVertices (Shape shape, Vertex input, float distance){
			var center = shape.center;
			var uvcenter = shape.uvCenter();
			List<Vertex> vertices = new List<Vertex>();
			for (int i= 0; i < shape.points.Count - 1;i++){
				if (input.Equals(shape.points[i])){
					var v = shape.points[i].v;
					
					v = slideVertex (center,v,distance);
					var vertex = new Vertex(v);

					if (vertices.Contains(vertex)) continue;
					vertices.Add( vertex);
					
					
				}
			}
			return vertices;
		}
		
		public static List<Vertex> getShrunkEdge (Shape shape, Edge e, float distance){
			return getShrunkVertices(shape,new List<Vertex>(new Vertex[]{e.v1,e.v2}),distance);
		}
		
		public static List<Vertex> getShrunkVertices (Shape shape, List<Vertex> input, float distance){
			List<Vertex> vertices = new List<Vertex>();
			foreach (Vertex v in input){
				vertices.AddRange(getShrunkVertices(shape,v,distance));
			}
			return vertices;
		}
		
		
		public  Shape shrinkShape (Shape shape, float distance){
			List<Vertex> vertices = new List<Vertex>();
			for (int i= 0; i < shape.points.Count - 1;i++){
				vertices.AddRange(getShrunkVertices(shape,shape.points[i],distance));
			}
			//Debug.Log(vertices.Count);
			return new Shape(vertices);
	
		}
	}
}*/
