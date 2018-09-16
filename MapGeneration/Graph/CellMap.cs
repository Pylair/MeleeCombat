/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 12/20/2017
 * Time: 9:30 PM
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
	/// <summary>
	/// Description of CellMap.
	/// </summary>
	public class CellMap {
		
		public Dictionary<Vector3,Cell> cells;
		public Dictionary<Vector3,Shape> walls;
		public Dictionary<Vector3,List<Edge>> edges;
		
		public CellMap (){
			cells = new Dictionary<Vector3, Cell>();
			walls = new Dictionary<Vector3, Shape>();
			edges = new Dictionary<Vector3, List<Edge>>();
		}
		
		public List<Shape> wallList {
			get {return walls.Values.ToList();}
		}
		
		public void merge (CellMap other){
			other.cells.ToList().ForEach(x => cells[x.Key] = x.Value);
			other.walls.ToList().ForEach(x => walls[x.Key] = x.Value);
			other.edges.ToList().ForEach(x => edges[x.Key] = x.Value);
			
		}
		
		public List<Edge> edgeList {
			get {
				
				var output = new HashSet<Edge>();
				foreach (List<Edge> e in edges.Values){
					output.UnionWith(e);
				}
				
				return output.ToList();
				
			}
		}
		
		public void addElement (Cell cell){
			cells[cell.center] = cell;
			foreach (Shape s in cell.walls){
				if (walls.ContainsKey(s.center)){
					if (walls[s.center].tag.tags.Contains(FaceTypes.FLOOR)) continue;	
				}
				walls[s.center] = s;
				
			}
			foreach (Edge e in cell.edges){
				addElement(e);
			}
		}
		
		
		public void addElement (Edge e){
			addVertex(e.v1,e);
			addVertex(e.v2,e);
		}
		
		
		public void addVertex (Vector3 v,Edge e){
			if (! edges.ContainsKey(v)) {
				edges[v] = new List<Edge>();
			} 
			var list = edges[v];
			if (list.Contains(e))return;
			list.Add(e);
			
			
		}
		
		public void Except (CellMap other){
			cells = cells.Where(x => ! other.cells.ContainsKey(x.Key)).ToDictionary(x => x.Key,x => x.Value);
			edges = edges.Where(x => ! other.edges.ContainsKey(x.Key)).ToDictionary(x => x.Key,x => x.Value);
			walls = walls.Where(x => ! other.walls.ContainsKey(x.Key)).ToDictionary(x => x.Key,x => x.Value);
		}
		
		
		
	}
	
}
