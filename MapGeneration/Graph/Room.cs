/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 9/18/2017
 * Time: 8:48 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using GeoAPI.Geometries;
using System.Linq;


namespace MeleeCombat.MapGeneration
{

	
	public class Room
	{
		
		public static System.Random random = new System.Random();
		
		//returns the keys of the internal cell map. contains all points in room
		public HashSet<Vector3> pointsInRoom {
			get{
				return new HashSet<Vector3>(cells.Keys);
			}	
		}
		
		public Bounds bounds{
			get;private set;
		}
		
		public int Count {
			get {return pointsInRoom.Count;}
		}
			
		public HashSet<Room> neighbors = new HashSet<Room>();
	
		public BoundedSpaceTags tag = new BoundedSpaceTags(FaceTypes.IGNORE);
		
		public CellMap getCellMap () {
			var graph = new CellMap();
			foreach (Cell c in cells.Values){
				graph.addElement(c);
			}
			return graph;
		}
		
	
		static bool carveEntrance (Cell cell1, Cell cell2,HashSet<Shape> finalEntranceQuads){
			var intersection = cell1.walls.Intersect(cell2.walls).ToList();
		//	Debug.DrawLine(cell1.center,cell2.center,Color.green,float.PositiveInfinity,false);
		
			if (intersection.Count == 0) {
			//	Debug.Log(cell1.walls.Count + " "+ cell2.walls.Count);
			//	Debug.DrawLine(cell1.center,cell2.center,Color.yellow,float.PositiveInfinity,false);
		
				return false;
			}
			
			foreach (Shape s in intersection){
				if (s.tag.tags.Contains(FaceTypes.ENTRANCE)) return false;
				s.tag.tags.Add(FaceTypes.ENTRANCE);
			}
			finalEntranceQuads.UnionWith(intersection);

			cell1.walls.RemoveAll(intersection.Contains);
			cell2.walls.RemoveAll(intersection.Contains);
			
			
							
			cell1.walls.AddRange(intersection);
			cell2.walls.AddRange(intersection);
			
			finalEntranceQuads.UnionWith(intersection);
			
			
			
			
			return true;
		}
		

		public static bool carveBetweenRooms (Room r1, Room r2){
			var touchingPoints = Room.touchingCells(r1,r2);
			if (touchingPoints.Count == 0) {
				//Debug.DrawLine(r1.closestPointToCenter(),r2.closestPointToCenter(),Color.green,float.PositiveInfinity,false);
				return false;
			}
			
			var keys = touchingPoints.Keys.ToList();
			var values = touchingPoints.Values.ToList();
			while (keys.Count > 0){
				var count = random.Next(0,keys.Count);
				var cell1 = keys[count];
				var cell2 = touchingPoints[cell1];
				keys.RemoveAt(count);
	
				if (Mathf.Approximately(cell1.center.z , cell2.center.z)){
					
					if (! cell1.cellContainsAttribute(FaceTypes.FLOOR) ){
					//	Debug.DrawLine(cell1.center,cell1.center + Vector3.forward/2,Color.blue,float.PositiveInfinity,true);
						continue;
				    }
				
					if  (! cell2.cellContainsAttribute(FaceTypes.FLOOR)) {
					//	Debug.DrawLine(cell2.center,cell2.center + Vector3.forward/2,Color.blue,float.PositiveInfinity,true);
						continue;
					}
				
				
				
				//	Debug.DrawLine(cell1.center,cell2.center,Color.red,float.PositiveInfinity,true);
			
				//	if (cell1.occupied || cell2.occupied) continue;
					//Debug.DrawLine(cell1.center + new Vector3(.1f,.1f,.1f),cell2.center+ new Vector3(.1f,.1f,.1f),Color.magenta,float.PositiveInfinity,true);
			
					var finalEntranceQuads = new HashSet<Shape>();
					if (carveEntrance(cell1,cell2,finalEntranceQuads)){
					//	Debug.DrawLine(cell1.center,cell2.center,Color.cyan,float.PositiveInfinity,true);
					//	GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = cell1.center;
					//	GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = cell2.center;
					
						return true;
					} else {
						
					}
				}
			}
			return false;
		}

		public bool removeCell (Vector3 v){
			if (pointsInRoom.Contains(v)){
				pointsInRoom.Remove(v);
				cells.Remove(v);
				
				return true;
			}
			return false;
		}
		
		public bool removeCell (Cell c){
			if (pointsInRoom.Contains(c.center)){
				pointsInRoom.Remove(c.center);
				cells.Remove(c.center);
				
				return true;
			}
			return false;
		}
		
		public Vector3 closestPointToCenter () {
			var l = pointsInRoom.OrderBy(x => (x - bounds.center).magnitude).ToList();
			return l[0];
		}
		
		public Room (Bounds bounds) : this(bounds, new HashSet<Vector3>()){
		
		}
		
		public Room (Rect rect, int height, int z){
			bounds = new Bounds( new Vector3(rect.xMin,rect.yMin,z),Vector3.zero);
			bounds.Encapsulate(new Vector3(rect.max.x,rect.max.y,z + height));
			cells = new Dictionary<Vector3, Cell>();
			int l = (int)rect.width;
			int w = (int)rect.height;
			int h = (int)height;
			var v0 =  BoundedSpace.roundVector(new Vector3(rect.xMin, rect.yMin,z),0);
			for (int i = 0;i <l;i++){
				for (int j = 0; j < w;j++){
					for (int k = 0; k < h;k++){
						var v1 = new Vector3(i,j,k) + v0;
						cells[v1] =new Cell(v1);
					}
					
				}
			}
			createBorderGeometry();
		}

		public Room (Bounds bounds,HashSet<Vector3> occupiedPoints){
			this.bounds = bounds;
			int l = (int)bounds.size.x;
			int w = (int)bounds.size.y;
			int h = (int)bounds.size.z;
			var v0 = BoundedSpace.roundVector(bounds.min,0);
			cells = new Dictionary<Vector3, Cell>();
			for (int i = 0;i <l;i++){
				for (int j = 0; j < w;j++){
					for (int k = 0; k < h;k++){
						var v1 = new Vector3(i,j,k) + v0;
						if (! occupiedPoints.Contains(v1))
							cells[v1] =new Cell(v1);
							occupiedPoints.Add(v1);
						
					}
					
				}
			}
			createBorderGeometry();
			
		}	
		
		public Room (IEnumerable<Vector3> otherPoints){
			cells = new Dictionary<Vector3, Cell>();
			var l = new HashSet<Vector3>(otherPoints).ToList();
		
			var b = new Bounds(l[0],Vector3.zero);
			
			for (int i = 0; i < l.Count;i++){
				var v = l[i];
				b.Encapsulate(v);
				cells[v] = new Cell(v);
			}
			bounds = b;
			createBorderGeometry();
			
		}

		
		public HashSet<Cell> getCellsByAttribute (FaceTypes tag){
			var output = new HashSet<Cell>();
			output.UnionWith(cells.Values.ToList().FindAll(x => x.cellContainsAttribute(tag)));
			return output;
		}
	
		public HashSet<Vector3> getFloorPoints () {
			var floorPoints = new HashSet<Vector3>();
			foreach (Vector3 v in pointsInRoom){
				if (! pointsInRoom.Contains(v + Vector3.back)) floorPoints.Add(v);
			}
			return floorPoints;
		}
		
	
		
		public static List<Shape> WallUnion (Room a, Room b){
			var walls = new List<Shape>();
			foreach (Vector3 v in a.pointsInRoom){
				foreach (Vector3 v2 in VoxelIterators.VonNeumanNeighbors3D()){
					var v3 = v + v2;
					if (b.pointsInRoom.Contains(v3)){
						var c = a.cells[v].walls;
						var d = b.cells[v3].walls;
						walls.AddRange(c.Intersect(d));
					}
				}
			}
			return walls;
		}
		
		
		
		public static Dictionary<Cell,Cell> touchingCells (Room a, Room b){
			Dictionary<Cell,Cell> d = new Dictionary<Cell, Cell>();
			foreach (Vector3 v in a.pointsInRoom){
				foreach (Vector3 v2 in VoxelIterators.VonNeumanNeighbors3D()){
					var v3 = v + v2;
					if (b.pointsInRoom.Contains(v3)){
						var c1 = a.cells[v];
						var c2 = b.cells[v3];
						d[a.cells[v]] = b.cells[v3];
					}
					
				}
			}
			return d;
		}
		
		static bool roomInTolerance (Bounds b, int minVolume){
			return  b.size.x / b.size.y < 4 && b.size.x /b.size.y > 1/4f && b.size.x * b.size.y * b.size.z > minVolume;
		}
		

		public static void divideRoom (IEnumerable<Vector3> points, int divisions, bool vertical, int minVolume,List<Room> output){
		
			var pool = points.ToList();
			var pointsInRoom = points.ToList();
	
			if ( divisions <= 0 || pointsInRoom.Count <= minVolume){
				output.Add(new Room(points));
				return;
			}

			while (pool.Count > 0){
				Vector3 normal;
				if (vertical){
					if (random.NextDouble() < .5){
						normal = Vector3.right;
					} else {
						normal = Vector3.up;
					}
				} else {
					normal = Vector3.forward;
				}
				var index =random.Next(0,pool.Count);
				var center = pool[index];
				pool.RemoveAt(index);
				

				var r1 = new HashSet<Vector3>();
				var r2 = new HashSet<Vector3>();
				
				
				bool s1 = true;
				bool s2 = true;
				Bounds b1 = default(Bounds);
				Bounds b2 = default(Bounds);
				foreach (Vector3 v in pointsInRoom){
					var dir = v - center;
					if (Vector3.Dot(dir,normal) >= 0){
						r1.Add(v);
						if (s1){
							s1 = false;
							b1 = new Bounds(v,Vector3.zero);
						} else {
							b1.Encapsulate(v + Vector3.one/2);
							b1.Encapsulate(v - Vector3.one/2);
						}
						
						
						
					} else {
						r2.Add(v);
						if (s2){
							s2 = false;
							b2 = new Bounds(v,Vector3.zero);
						} else {
							b2.Encapsulate(v+ Vector3.one/2);
							b2.Encapsulate(v- Vector3.one/2);
						}
					}
				}
			

				if (r1.Count == 0 || r2.Count == 0) continue;
						
	
				if (roomInTolerance(b1,minVolume) && roomInTolerance(b2,minVolume)){
					divideRoom(r1,divisions -1,vertical,minVolume,output);
					divideRoom(r2,divisions -1, vertical,minVolume,output);
					return;
				}
			}
			output.Add(new Room(pointsInRoom));
		}
		
		public List<Room> createSubRooms (int roomDivisionPerFloor, int numberOfFloors, int minVolume){
			var roomList = new List<Room>();
			divideRoom(this.pointsInRoom,numberOfFloors,false,minVolume,roomList);
			var floors = new List<Room>();
			foreach (Room r in roomList){
				divideRoom(r.pointsInRoom,roomDivisionPerFloor,true,minVolume,floors);
			}
			foreach (Room r in floors){
				r.tag = tag;
			}
			return floors;
		}

		public Color color;

		HashSet<Vector3> filled = new HashSet<Vector3>();
		
		
		public Dictionary<Vector3,Cell> cells;
		public HashSet<Vector3> borderVertices;
		
		public void refresh () {
			var keys = cells.Keys.ToList();
			foreach (Vector3 v in keys){
				cells[v] = new Cell(v);
			}
		}
		
		public static List<Room> splitDisconnectedRoom (Room r){
			List<Room> output = new List<Room>();
			var input = splitPointSet(r.pointsInRoom);
			for (int i = 0; i < input.Count;i++){
				var r2 = new Room(input[i]);
				r2.tag = r.tag;
				output.Add(r2);
			}
			
			return output;
		}
		
		public static List<List<Vector3>> splitPointSet (HashSet<Vector3> points){
			var input = new HashSet<Vector3>(points);
			List<List<Vector3>> output = new List<List<Vector3>>();
			while (input.Count > 0){
				var visitedPoints = new HashSet<Vector3>();
				recurse(input.First(),input,visitedPoints);
				output.Add(visitedPoints.ToList());
			}
			return output;
		}
		
		public static void recurse (Vector3 v, HashSet<Vector3> points, HashSet<Vector3> visitedPoints){
			if (visitedPoints.Contains(v)) return;
			visitedPoints.Add(v);
			points.Remove(v);
			foreach (Vector3 v2 in VoxelIterators.VonNeumanNeighbors3D()){
				var v3 = v2 + v;
				if (points.Contains(v3)){
					recurse(v3,points,visitedPoints);
				}
			}
		}
		
		public void createBorderGeometry () {
			foreach (Cell cell in cells.Values){
				var v = cell.center;
				foreach (Vector3 v2 in VoxelIterators.VonNeumanNeighbors3D()){
					if (cells.ContainsKey(v2 + v)){
						cell.neighbors.Add(v2 + v);
						cell.removeWall(cells[v2 + v]);
						cells[v2 + v].removeWall(cell);
					}
				}
			}
			
		}

		public static int horizontalCount (Vector3 v, HashSet<Vector3> pointsInRoom){
			int count = 0;
			for (int i = -1; i < 2;i++){
				for (int j = -1; j < 2;j++){
			
					var sum = Math.Abs(j) + Math.Abs(i);
					if (sum == 0) continue;
					if ( pointsInRoom.Contains(new Vector3(i,j,0) + v)){
						count++;
					}
				}
			}
			return count;
		}
	
		public IGeometry addTile (IGeometry quad, IGeometry main){
			return main.Union(quad);
		}
	
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			Room other = obj as Room;
				if (other == null)
					return false;
				return ! this.pointsInRoom.Except(other.pointsInRoom).Any();
				
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (pointsInRoom != null)
					foreach (Vector3 v in cells.Keys){
						hashCode += 100007 * v.GetHashCode();
					}
			}
			return hashCode;
		}

		public static bool operator ==(Room lhs, Room rhs) {
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Room lhs, Room rhs) {
			return !(lhs == rhs);
		}

		#endregion
		
	}
}
	