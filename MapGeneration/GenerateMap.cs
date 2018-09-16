/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/20/2017
 * Time: 3:41 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using UnityEngine;
using System.Collections.Generic;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System.Linq;
using NetTopologySuite.Triangulate;
using MeleeCombat.MapGeneration.Algorithms;
using System;



namespace MeleeCombat.MapGeneration
{
	using Random = System.Random;
	using Debug = UnityEngine.Debug;
	
	public class GenerateMap : MonoBehaviour
	{

		Random random = new Random();
		
		public void Update () {
			if (trigger){
				trigger = false;
				execute();	
			}
		}
		
		public bool trigger;
		public bool fill;

		
		public StairCaseBuilder scb;
		public int minRoomLength;
		public int maxRoomLength;
		public int roomHeight;
		
		public int minRoomChildCount;
		public int maxRoomChildCount;

		public int  floors;
		public int  width;
		public int  height;
		public int  numberOfRooms;
		
		public Vector3 finalScale;
		public Vector3 offsetRotation;
		public Vector3 offsetPosition;

	
		Dictionary<BoundedSpaceTags,ComponentDirectory> componentDirectory;
		public GameObject componentDirectoryFile;
		
		Dictionary<Vector3, Room> pointDict = new Dictionary<Vector3, Room>();
		HashSet<Vector3> totalPoints;
		EdgeMap graph;
		
		public List<Vector3> requiredSpaces;
		
		public Dictionary<Vector3,int> test;
		
		
		public void execute(){	
			placeMapPieces(roomGenerator());
		}
		
		public void placeMapPieces (IEnumerable<Room> rooms){

			GameObject physicalMap = new GameObject();
			physicalMap.name = "Completed Map";
			
			Dictionary<Bounds,int> boundsCount = new Dictionary<Bounds, int>();
			Dictionary<Bounds,BoundsData> maxCount = new Dictionary<Bounds,BoundsData>();
			Dictionary<Bounds,List<Vector3>> directions = new Dictionary<Bounds, List<Vector3>>();
			List<GameObject> markedForDeletion = new List<GameObject>();
			GameObject parent = new GameObject();
			
			var cellDict = new CellMap();
			foreach (Room r in rooms){
				propFill(physicalMap,r);
				foreach (Cell c in r.cells.Values) cellDict.addElement(c);
			
			}
			
				
			

			setComponentDirectory(componentDirectoryFile);
			

			var walls = cellDict.wallList;
			var edges = cellDict.edgeList;
			
			
			
			while (walls.Any()){
				var s = walls.Last();
				walls.Remove(s);

					Vector3 right;
					Vector3 center;
					Bounds bounds;
					
					right = s.groupNormal;
					center = s.groupCenter;
					bounds = s.groupBounds;
					

					var forward = Vector3.forward;
					var up = Vector3.Cross(forward,right);
					var dot = Vector3.Dot(Vector3.forward,right);
					if (Mathf.Approximately(Math.Abs(dot),1)){
						up = Vector3.up;
						forward = Vector3.right;
					}
					
					bounds.size += right * .1f;
					

					var tag = s.tag;
		
					var g = componentDirectory[tag].getComponent(true);
					g.transform.parent = physicalMap.transform;
					Aligner.AlignBoundingBoxFaceToBoundingBoxFace(forward,up,bounds,g);
				}
	
			
				foreach (Edge e in edges){
					var forward = e.slope;
					forward = new Vector3(Math.Abs(forward.x),Math.Abs(forward.y),Math.Abs(forward.z));
					var up = Vector3.up;
					
					if (Mathf.Approximately(Math.Abs(Vector3.Dot(up,forward)),1)){
						up = Vector3.forward;
						
					}
					var right = Vector3.Cross(forward,up);
					var tag = e.tag;
					
					var size = right * .1f + up * .1f + forward;
					var bounds = new Bounds(e.center(),size);

					var g = Instantiate(componentDirectory[tag].defaultComponent);
					g.transform.parent = physicalMap.transform;
					Aligner.AlignBoundingBoxFaceToBoundingBoxFace(forward,up,bounds,g);
				}
		
			
			physicalMap.transform.localEulerAngles += offsetRotation;
			physicalMap.transform.localScale = finalScale;
			physicalMap.transform.localPosition += offsetPosition;
			
		}
	
		public static Vector3[] directions = new Vector3[]{Vector3.right,Vector3.left,Vector3.up,Vector3.down};
		
		public bool generateHypotheticalChildRoom (out Rect current,Rect previousRect,List<Rect> rects){
			
			current = previousRect;
			var v = new Vector3(random.Next(minRoomLength,maxRoomLength),random.Next(minRoomLength,maxRoomLength),0);
			var i = random.Next(0,directions.Length);
			var direction = directions[i];
			
			float l = 0;
			if (i < 2){
				l = v.x/2.0f + previousRect.width/2.0f;
			} else {
				l = v.y/2.0f + previousRect.height/2.0f;
			}
			
			var projectedCenter = (Vector3)previousRect.center + l * direction;
			var rect = new Rect(projectedCenter - v/2,v);
			
			if (rects.Contains(rect)){
				return false;
			}
			foreach (Rect r2 in rects){
				if (r2.Overlaps(rect)) return false;
			}
			current = rect;
			return true;
		}
		
		bool generateRoomHelper(ref int roomCount, HashSet<Vector3> roomCenters, Rect rect, List<Rect> rects, Vector3 v){
			foreach (Rect r2 in rects){
				if (r2.Overlaps(rect)) return false;
			}
				
			rects.Add(rect);
			var room = new Room(rect,roomHeight,0);
			room.tag = new BoundedSpaceTags(FaceTypes.LIBRARY);
			assignPointToRoom(room,true);			
			roomCenters.Add(v);
			roomCount++;
			return true;
		}
			
		public HashSet<Room> roomGenerator  () {
			var watch = System.Diagnostics.Stopwatch.StartNew();
			var roomCenters = new HashSet<Vector3>();

			var rs = new List<Vector3>(requiredSpaces);
			
			
			totalPoints = new HashSet<Vector3>();
			for (int i = 0; i < width;i++){
				for (int j = 0; j < width;j++){
					for (int k = 0; k < height;k++){
						totalPoints.Add(new Vector3(i,j,k));
					}
				}
			}
			graph = EdgeMap.constructEdgemapFromPoints(totalPoints);
			var rects = new List<Rect>();
			
			int failsafe = 0;
			int roomCount = 0;
			while (roomCount< numberOfRooms || requiredSpaces.Count < 0){
				failsafe ++;
				if (failsafe > 100) {
					Debug.Log("overflow");
					break;
				}
				
				var v = new Vector3(random.Next(0,width),random.Next(0,width),0);
				Vector3 size = new Vector3(random.Next(minRoomLength,maxRoomLength),random.Next(minRoomLength,maxRoomLength),0);
				if (requiredSpaces.Count > 0){
					size = requiredSpaces[0];
				}
				
				
				
				var rect = new Rect(v,size);
				
				if (generateRoomHelper(ref roomCount,roomCenters,rect,rects,v)){
					requiredSpaces.Remove(size);

					int childCount = random.Next(minRoomChildCount,maxRoomChildCount);
					Rect current;
					for (int q = 0; q < childCount;q++){
						if (generateHypotheticalChildRoom(out current,rect,rects)){
							generateRoomHelper(ref roomCount,roomCenters,current,rects,rect.min);
						}
					}
				}				
			}
		

			var edges =(getDelaunayFromPoints(roomCenters)).ToList();
			var totalEdges = new HashSet<Edge>(edges);
			edges=  new HashSet<Edge>(new Prims().execute(edges)).ToList();
			foreach (Edge e in totalEdges){
				if (random.NextDouble() < .15 && ! edges.Contains(e)){
					edges.Add(e);
				}
			}
			var g = new EdgeMap(edges);

			foreach (Edge e in edges){	
				var points = paintRoomAroundLine(e,1,2);
				if (points.Count > 0){
					var r = new Room(points);
					r.tag = new BoundedSpaceTags(FaceTypes.HALLWAY);
					assignPointToRoom(r,false);
				}	
			}	
			
			var output = new HashSet<Room>(pointDict.Values);
			foreach (Room r in output){
				var r2 = Room.splitDisconnectedRoom(r);
				foreach (Room r3 in r2) assignPointToRoom(r3,true);
			}
			output.Clear();
			output.UnionWith(pointDict.Values);
			
			new RecursiveBackTrace().recursiveBackTrace(pointDict);

			return output;
		}
	
		bool assignPointToRoom (Room r, bool overwrite){
			var points = r.pointsInRoom.ToList();
			int count = 0;
			foreach (Vector3 v in points){
				if (totalPoints.Contains(v)){
					count++;
					if (pointDict.ContainsKey(v)){
						if (overwrite){
							pointDict[v].removeCell(v);
							pointDict[v] = r;
						} 	else {
							r.removeCell(v);
						}
					} else {
						pointDict[v] = r;
					}
				} 
				
			}
			return count == points.Count;
		}
	
		
		public static HashSet<Edge> getDelaunayFromPoints (IEnumerable<Vector3> points){
			HashSet<Edge> edges = new HashSet<Edge>();
			
			NetTopologySuite.Triangulate.DelaunayTriangulationBuilder builder = new DelaunayTriangulationBuilder();
			var coords = new List<Coordinate>();
			foreach (Vector3 v in points) coords.Add(new Coordinate((int)v.x,(int)v.y,0));
			builder.SetSites(coords);
			var lines = builder.GetEdges(new GeometryFactory());
			foreach (LineString l in lines.Geometries){
				edges.Add(new Edge(new Vector3((int)l.Coordinates[0].X,(int)l.Coordinates[0].Y,0),new Vector3((int)l.Coordinates[1].X,(int)l.Coordinates[1].Y,0)));
			}
			
			/*foreach (Vector3 v1 in points){
				foreach (Vector3 v2 in points){
					if (v1.Equals(v2)) continue;
					var e = new Edge(v1,v2);
					
					edges.Add(e);
					;
				}
			}*/
			return edges;
		}
	
		public HashSet<Vector3> paintRoomAroundLine (Edge e0, int w, int h){
			var edges = (Edge.straightenEdge(e0));
			var hallwayPoints = new HashSet<Vector3>();
			Debug.DrawLine(e0.v1,e0.v2,Color.green,float.PositiveInfinity,false);
			foreach (Edge e in edges){
				var angle = Vector3.Angle(e.slope,Vector3.forward);
				if ( Mathf.Approximately(90,angle)) {
					var path = Dijkstras.ASTAR(graph,e.v1,e.v2);
					hallwayPoints.UnionWith(path);
					Debug.DrawLine(e.v1,e.v2,Color.blue,float.PositiveInfinity,false);
				
				} else {
					Debug.DrawLine(e.v1,e.v2,Color.red,float.PositiveInfinity,false);
				
					
				}
			}
			return hallwayPoints;
		}
		
		
		public void propFill (GameObject parent,Room r){
			if (r.tag.tags.Contains(FaceTypes.LIBRARY)){
				var points = r.getFloorPoints();
				var propFill = GetComponent<PropFill>();
				propFill.run(parent,r,points);
			}
		}
		
		
		public void setComponentDirectory (GameObject cdf) {
			componentDirectory = new Dictionary<BoundedSpaceTags, ComponentDirectory>();
			var componentDirectories = cdf.GetComponents<ComponentDirectory>();
			foreach (ComponentDirectory c in componentDirectories){
				componentDirectory[new BoundedSpaceTags(c.faceTagList)] = c;
			}
		}
		
		
		
	}
}
