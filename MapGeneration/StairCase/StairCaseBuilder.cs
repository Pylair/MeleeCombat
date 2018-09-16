/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 12/3/2017
 * Time: 11:16 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MeleeCombat.MapGeneration
{
	/// <summary>
	/// Description of StairCaseBuilder.
	/// </summary>
	public class StairCaseBuilder 
	{
		
		public GameObject railingStart;
		public GameObject stairCasePiece;
		public GameObject railingEnd;
		

		public bool checkFloorConnectivity (IEnumerable<Cell> cells, IEnumerable<Cell> obstructingCells, int count){
			var roomCells = new List<Cell>(cells);
			roomCells = roomCells.Except(obstructingCells).ToList();
			roomCells = roomCells.FindAll(x => x.cellContainsAttribute(FaceTypes.FLOOR)).ToList();
			int numberOfGroups = 0;
			while (roomCells.Count > 0){
				removeNeighbors(roomCells,roomCells.First());
				numberOfGroups++;
			}
			return numberOfGroups == count;
			
		}
		
		public void removeNeighbors (List<Cell> cells, Cell c){
			cells.Remove(c);
			foreach (Vector3 v in VoxelIterators.VonNeumanNeighbors()){
				var c2 = cells.Find(x => x.center == v +c.center);
				if (c2 != null && ! c2.occupied) removeNeighbors(cells,c2);
			}
		}
		
		public void allocateSpace (Room a, Room b, Vector3 brushSize, PropFill propFill){
			
			var points = new HashSet<Vector3>(a.pointsInRoom);
			while (points.Count > 0){
				var p = points.Last();
			
				points.Remove(points.Last());
				/*if (propFill.getHasRequiredSpace(p,brushSize,a.pointsInRoom)){
					//Debug.Log(p);
					//GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = p;
						
					var p2 = b.pointsInRoom.ToList().Find(x => x.x == p.x && x.y == p.y);
					if (p2 != null){
						GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = p2;
						break;
						
					}
					
					
					
					
				}
				*/

				
			}
			
			
			
		}
		
		/*public bool checkHorizontalDirection (List<Cell> output, List<Vector3> vertical, List<Vector3> graph, Vector3 direction, int height,Staircase stairCase){
			var crawler = new Crawler();
			var obstructingCells = new List<Cell>();
			for (int i = 0; i < vertical.Count;i++){
				
				var c = vertical[i];
				
				var p = c.center;
				
				var horizontalPoints = crawler.crawl(direction,p,graph,height + 2);
			
				
				if (horizontalPoints.Count != height + 2){
					return false;
				}
				output.AddRange(horizontalPoints);
				obstructingCells.AddRange(horizontalPoints);
			}
			
			stairCase.direction = direction;
			stairCase.minimum = vertical[0];
			stairCase.start = graph.cells[vertical[0].center + Vector3.forward * height];
			stairCase.end = graph.cells[vertical[0].center + direction * stairCase.distance];
			
			
			bool pass1 = false;
			bool pass2 = false;
			foreach (Vector3 v in VoxelIterators.VonNeumanNeighbors()){
				var v1 = v + stairCase.start.center;
				var v2 = v + stairCase.end.center;
				if (graph.cells.ContainsKey(v1) && ! graph.cells[v1].occupied){
					pass1 = true;
				}
				if (graph.cells.ContainsKey(v2) && ! graph.cells[v2].occupied){
					pass2 = true;
				}
			}
			if (! pass1 || !pass2) return false;

			stairCase.distance = height + 2;
			stairCase.height = height;
			return checkFloorConnectivity(graph.cells.Values,obstructingCells,2);
		}*/
	
	
		/*public Room constructStairCase (Staircase staircase, CellMap graph,List<Cell> output){
			var direction = staircase.direction;
			var min = staircase.minimum.center;
			var cellsToRemove = new List<Cell>();
			var height = staircase.height;
			
			Cell start = null;
			Cell end = null;
			Vector3 center;
			Bounds bounds;

			for (int h = 0; h < height;h++){
				for (int d = 0; d < staircase.distance;d++){
					
					center = min + h * Vector3.forward + d * staircase.direction;
					
					GameObject g = null;
					bounds = new Bounds(center,Vector3.one);
						
					if (d == 0){
						
						if (h == height - 1) start = graph.cells[center + Vector3.forward];
					
						g = GameObject.CreatePrimitive(PrimitiveType.Cube);

						
					} else if (d < staircase.distance - 1){
						if (d + h == height){
							g = GameObject.Instantiate(stairCasePiece);

						} else if (d + h < height){
							g = GameObject.CreatePrimitive(PrimitiveType.Cube);
							
						} 
						
						
						
					} else {
						if (h == 0) {
							end = graph.cells[center];
							g = GameObject.Instantiate(railingEnd);
						}
						
					}
					if (g != null) Aligner.AlignBoundingBoxFaceToBoundingBoxFace(direction,Vector3.forward,bounds,g);
					
					
					if (h >= height - 2 && h > 0 && d > 0){
						center += Vector3.forward;
						cellsToRemove.Add(graph.cells[center]);
					}
				}
			}
			
			var temp = GameObject.Instantiate(railingStart);
			center = min + height * Vector3.forward;
			bounds = new Bounds(center,Vector3.one);
			Aligner.AlignBoundingBoxFaceToBoundingBoxFace(direction,Vector3.forward,bounds,temp);
			
			cellsToRemove.Remove(start);
			cellsToRemove.Remove(end);

								
			return markCellsAsOccupied(output,previous,currentRoom,cellsToRemove,start,end);
		}*/
		
		
		/*Room markCellsAsOccupied (List<Cell> output,Room r1, Room r2,List<Cell> cellsToRemove, Cell start, Cell end) {
			var room = new Room(output);

			foreach (Vector3 v in room.pointsInRoom){
				r1.removeCell(v);
				r2.removeCell(v);
			}
			room.refresh();
			room.createBorderGeometry();
	
			foreach (Vector3 v in room.cells.Keys){
				var c = room.cells[v];
				if (c.center.Equals(start.center)){
					c.occupied = false;
					start= c;
				} else if (c.center.Equals(end.center)){
					end = c;
					c.occupied = false;
				 } else {	
					c.occupied = true;
				}
			}
			start.restoreFloor();
			end.restoreFloor();
			room.tag = new BoundedSpaceTags(FaceTypes.STAIRCASE);
			return room;
		
		}
		*/
		
		
		public Room findSuitableStaircaseLocation (int height, Vector3 previousLocation , HashSet<Vector3> graph){

			
			var crawler = new Crawler();
			var points = graph.OrderBy(x => (x - previousLocation).magnitude).ToList();
			//Debug.Log("Graph count: " + graph.Count);
			var c = 0;
			while (points.Count > 0){

				var p = points[0];
				points.RemoveAt(0);
				
				var vertPoints = crawler.crawl(Vector3.forward,p,graph,4);
				var downPoints = crawler.crawl(Vector3.back,p,graph,1);
				vertPoints = vertPoints.OrderBy ( x => x.z).ToList();
				if (vertPoints.Count == 4 && downPoints.Count == 1){

					foreach (Vector3 direction in VoxelIterators.VonNeumanNeighbors()){
						var horizontalPoints = new List<Vector3>();
						bool badLocation = false;
						foreach (Vector3 v in vertPoints){
							var line = crawler.crawl(direction,v,graph,4);
							if (line.Count != 4) {
								badLocation = true;
							} else {
								horizontalPoints.AddRange(line);
							}
						}
						
						if (! badLocation){
							return new Room(horizontalPoints);
						}
					}	
				
				}
				
				
			}
			return null;
		
		}
		
		
	}
}
