/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 1/29/2018
 * Time: 2:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MeleeCombat.MapGeneration.Algorithms;

namespace MeleeCombat.MapGeneration
{

	public class PropFill : MonoBehaviour
	{

		public Material mA;
		public List<PropOverlayGroup> propOverlays;
		public List<BoundedSpaceTags> tags;
		public GameObject g1;
		public GameObject g2;
		public GameObject g3;
		public bool s = false;
	
		System.Random r = new System.Random();	
		
		public Dictionary<Vector3, GridUnit> grid = new Dictionary<Vector3, GridUnit>();
		
		
		public void run (GameObject parent,Room room,HashSet<Vector3> availablePoints)  {
			grid.Clear();
			colorDict.Clear(); 
			commitColorChangeToGrid(availablePoints,Color.grey);
			
			var tag = room.tag;
			var index = -1;
			PropOverlayGroup overlayGroup = null;
			
			if (tags.Contains(tag)) {
				index = tags.IndexOf(tag);
				overlayGroup = propOverlays[index];
			}
	
			if (index < 0) return;
			
			var white = new List<Vector3>();
			drawEntrancePath(room,white);
			commitColorChangeToGrid(white,Color.white);

			var propUnitCollection = partitionSpace(room);
			foreach (PropUnitCollection list in propUnitCollection){
				fill(room,list,parent);
			}
			
		}
		
		
		
		void commitColorChangeToGrid (IEnumerable<Vector3> list, Color c){
			if (! colorDict.ContainsKey(c)) colorDict[c] = new List<GridUnit>();
				
			foreach (Vector3 v in list) {
		
				if (grid.ContainsKey(v)) {
					colorDict[c].Remove(grid[v]);
					grid[v].changeColor(c);
				}
				else {
					grid[v] = new GridUnit(v,c);
				}
				
				colorDict[c].Add(grid[v]);
				
				if (c == Color.white){
					foreach (Vector3 v0 in VoxelIterators.FullNeighbors()){
						var v2 = v0 + v;
						if (! list.Contains(v2) && grid.ContainsKey(v2) && grid[v2].C != Color.white){
							grid[v2].value += 1;
						}
					}
				}
				
			}
		}
		
		List<GridUnit> getListOfPointsOfColor (Color c){
			if (! colorDict.ContainsKey(c)){
				colorDict[c] = new List<GridUnit>();
			}
			return colorDict[c];
		}
		
		void drawEntrancePath (Room room, List<Vector3> white) {
			var entrances = room.getCellsByAttribute(FaceTypes.ENTRANCE);
			
			foreach (Cell c in entrances) white.Add(c.center);
			var edgeMap = EdgeMap.constructEdgemapFromPoints(room.pointsInRoom);
			var edges = GenerateMap.getDelaunayFromPoints(white).ToList();
			edges = new Prims().execute(edges).ToList();
			foreach (Edge e in edges){
				var path = Dijkstras.ASTAR(edgeMap,e.v1,e.v2);
				for (int i = 0; i < path.Count -1;i++){
					Debug.DrawLine(path[i],path[i+1],Color.grey,float.PositiveInfinity,false);
				}
				white.AddRange(path);
			}
		}

		//arbitrary number that tells partitionSpace the number of cycles to run before giving up
		int cutoff = 100;
		Dictionary<Color,List<GridUnit>> colorDict = new Dictionary<Color, List<GridUnit>>();
		
		public List<PropUnitCollection> partitionSpace (Room room){
			var rects = new Dictionary<Rect,PropOverlay>();
			var POG = propOverlays[0].propOverlays;	
			var propQueue = new Queue<PropOverlay>(POG);
			var output = new List<PropUnitCollection>();
			
			var list = getListOfPointsOfColor(Color.grey);
			list = list.OrderByDescending(x => x.value).ToList();	
			var queue = new Queue<GridUnit>(list);
			
			for (int q = 0; q < cutoff;q++){
				
				
				
				if (list.Count == 0) break;
				var gU = queue.Dequeue();
				var p = gU.V;
				queue.Enqueue(gU);
				
				
				PropOverlay overlay = propQueue.Peek();
				
				if (! overlay.highPriority){
					propQueue.Dequeue();
					propQueue.Enqueue(overlay);
				}
				
				int iter = 1;
				if (overlay.extendable) iter = r.Next(3,6) ;
				
				List<PropUnitCollection> possibilities = new List<PropUnitCollection>();
				foreach (Vector3 direction in VoxelIterators.VonNeumanNeighbors()){
					
					var d2 = new Vector3(direction.x * overlay.size.x, direction.y * overlay.size.y, direction.z * overlay.size.z);
					var rot = Quaternion.identity;
					var black = new PropUnitCollection();
					if (recurse(iter,d2,p,rot,overlay,room,black) && black.list.Count > 0){
						possibilities.Add(black);
					}

					
				}
				possibilities = possibilities.OrderByDescending(x=> x.list.Count).ToList();
				if (possibilities.Count > 0){
					var first = possibilities.First();
					var black = first.getListOfPoints();
					if (black.Count > 0){
						output.Add(first);
						commitColorChangeToGrid(black,Color.black);
						var whitespace = new List<Vector3>();
						allocateWhiteSpace(black, whitespace);
						commitColorChangeToGrid(whitespace,Color.white);
						
						list = getListOfPointsOfColor(Color.grey);
						list = list.OrderByDescending(x => x.value).ToList();	
						queue = new Queue<GridUnit>(list);
					}
					
					if (overlay.highPriority){
						propQueue.Dequeue();
					}					
				}
			}
			//foreach (GridUnit v in getListOfPointsOfColor(Color.white)) GameObject.Instantiate(g1).transform.position = v.V;
			//foreach (GridUnit v in getListOfPointsOfColor(Color.black)) GameObject.Instantiate(g2).transform.position = v.V;
			
		
			return output;

		}
		
		
		bool recurse (int iter, Vector3 direction, Vector3 p, Quaternion rotation,PropOverlay overlay,Room room, PropUnitCollection propUnitCollection) {
			if (iter == 0 ) return true;
			
			
			
			var size = rotation * overlay.size ;
			var points = new List<Vector3>();
			var allocateBlackSpaceSucceeded = allocateBlackSpace(room,overlay,size,p,points) ;
			
			
			var totalblack = getListOfPointsOfColor(Color.black).ConvertAll(x => x.V).ToList();
			
			
			totalblack.AddRange(propUnitCollection.getListOfPoints());
			var totalWhite = grid.Keys.Except(totalblack).ToList();
			var islegal = check(grid.Keys.ToList(),totalWhite,totalblack);
			
			if (! islegal) {
				GameObject.Instantiate(g3).transform.position = p;
				grid[p].changeColor(Color.white);
				return false;
			}
			
			if (! allocateBlackSpaceSucceeded || points.Count == 0){
				return true;
			}
			
			var propUnit = new PropUnit(points,overlay);
			propUnitCollection.list.Add(propUnit);
			
			
			recurse(iter - 1,direction,p + direction,rotation,overlay,room,propUnitCollection);
					
			return true;

		}

		public bool positionSuitableForProp (PropOverlay overlay, Vector3 v, Room room) {
			var cell = room.cells[v];
			var walls= cell.findWallWithAttribute(FaceTypes.WALL);
			
			bool containsRightAmountOfWalls = overlay.numberOfWalls.Contains(walls.Count);
			bool containsNoExcludedTags = true;
		
			foreach (FaceTypes excludedTag in overlay.excludedTags){
				var count = cell.findWallWithAttribute(excludedTag).Count;
				if (count > 0) {
					containsNoExcludedTags = false;
				}
			}
						
			return containsNoExcludedTags && containsRightAmountOfWalls ;
			
		}
		
		bool allocateBlackSpace (Room room, PropOverlay overlay,Vector3 size, Vector3 p, List<Vector3> tempBlack) {
			var i0 = Math.Sign(size.x);
			var j0 = Math.Sign(size.y);
			var k0 = Math.Sign(size.z);
			
			for (int i = 0; i < Math.Abs(size.x);i++){
				for (int j = 0; j < Math.Abs(size.y);j++){
					for (int k = 0; k < Math.Abs(size.z);k++){
						var p2 = (p + new Vector3(i0 * i,j0 * j,k0 * k));
						
							if (grid.ContainsKey(p2) && grid[p2].C== Color.grey && (positionSuitableForProp(overlay,p2,room))){
								tempBlack.Add(p2);
							} else {
								return false;
							}
					}
					
				}
			}
			
			return true;
		}
		
		void allocateWhiteSpace (List<Vector3> black,  List<Vector3> white) {
			foreach (Vector3 v in black){
				foreach (Vector2 v0 in VoxelIterators.FullNeighbors()){
					var v2 = v + (Vector3)v0;
					if (grid.ContainsKey(v2) && grid[v2].C!= Color.black){
						white.Add(v2);
					}
				}
			}
		
			
		}

		void fill (Room room, PropUnitCollection propUnitCollection, GameObject parent) {
			var points = propUnitCollection.getListOfPoints();
			var neighbors = new List<Vector3>();
				
			
			foreach (Vector3 v in points){
				
				var cell = room.cells[v];
				var walls = cell.findWallWithAttribute(FaceTypes.WALL);
				foreach (Shape s in walls) {
					neighbors.Add(s.normal.normalized);
				}
				
				foreach (Vector3 v0 in VoxelIterators.VonNeumanNeighbors()){
					var v2 = v + v0;
					if (grid.ContainsKey(v2) && grid[v2].C== Color.white){
						neighbors.Add(v0);
					}	
				}
			}
			
			Vector3 horizontal;
			if (neighbors.Count == 0) {
				horizontal = Vector3.right;
			} else {
				var grouping = neighbors.GroupBy(a => a.GetHashCode());
				var groupList = grouping.OrderByDescending(b => b.Count());
				horizontal = groupList.First().First();
			}
			
			foreach (PropUnit propUnit in propUnitCollection.list){
				var center = propUnit.b.center;
				var vertical = Vector3.forward;
				var prop = GameObject.Instantiate(propUnit.overlay.prop);
				Aligner.AlignBoundingBoxFaceToBoundingBoxFace(horizontal,vertical,propUnit.b,prop);
				prop.transform.parent = parent.transform;
				
			}
		}
		
		
		
		/*IEnumerable<Vector3> iterateWithinRect (Vector3 min, Vector3 size){
			for (int u = 0; u < size.x;u++){
				for (int v = 0; v < size.y;v++){
					yield return min + new Vector3( u,v,0);
					
				}
			}
		}*/
		
		
		bool check (List<Vector3> list, List<Vector3> white, List<Vector3> black){
			var temp = list.ToList();
			temp.AddRange(white);
			temp.RemoveAll(black.Contains);
			return check (new HashSet<Vector3>(temp));
		}
		
		bool check (HashSet<Vector3> group){
			var l2 = new HashSet<Vector3>();
			int count = 0;
			while (group.Count > 0){
				var v = group.First();
				recurse(v,group);
				count++;
				
			}
			return count == 1;
		}
		
		void recurse (Vector3 v, HashSet<Vector3> group){
			if (! group.Contains(v)) return;
			group.Remove(v);
			foreach (Vector3 v0 in VoxelIterators.VonNeumanNeighbors()){
				recurse (v0 + v,group);
			}
		}
		
		
			
	}
	
	public class PropUnitCollection {
	
		public List<PropUnit> list = new List<PropUnit>();
	
	
		public List<Vector3> getListOfPoints (){
			var points = new HashSet<Vector3>();
			foreach (PropUnit u in list) points.UnionWith(u.points);
			return points.ToList();
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return (obj is PropUnitCollection) && Equals((PropUnitCollection)obj);
		}
	
		public bool Equals(PropUnitCollection other)
		{
			return object.Equals(this.list, other.list);
		}
	
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (list != null)
					hashCode += 1000000007 * list.GetHashCode();
			}
			return hashCode;
		}
	
		public static bool operator ==(PropUnitCollection lhs, PropUnitCollection rhs) {
			return lhs.Equals(rhs);
		}
	
		public static bool operator !=(PropUnitCollection lhs, PropUnitCollection rhs) {
			return !(lhs == rhs);
		}
	
		#endregion
	}
	
	public class GridUnit {
		public Vector3 V {
			get {return v;}
		}
		Vector3 v;
		public Color C {get { return c;}}
		Color c;
		public float value;
		
		
		public GridUnit(Vector3 v0, Color c0){
			this.v= v0;
			this.c = c0;
			if (C == Color.grey || C == Color.black) value = 1;
			else value = 0;
		}
		
		public void changeColor (Color c){
			this.c = c;
			
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			GridUnit other = obj as GridUnit;
				if (other == null)
					return false;
						return this.v == other.v && this.c== other.c;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * v.GetHashCode();
				hashCode += 1000000009 * c.GetHashCode();
			}
			return hashCode;
		}

		public static bool operator ==(GridUnit lhs, GridUnit rhs) {
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(GridUnit lhs, GridUnit rhs) {
			return !(lhs == rhs);
		}

		#endregion
	}

	public class PropUnit {
		
		public List<Vector3> points;
		public Bounds b;
		public PropOverlay overlay;
		
		public PropUnit(List<Vector3> p, PropOverlay overlay){
			this.points = p.ToList();
			this.overlay = overlay;
			b = new Bounds(points[0], Vector3.zero);
			foreach (Vector3 v in points){
				b.Encapsulate(v + Vector3.one/2);
				b.Encapsulate(v - Vector3.one/2);
			}
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return (obj is PropUnit) && Equals((PropUnit)obj);
		}

		public bool Equals(PropUnit other)
		{
			return this.b == other.b && object.Equals(this.overlay, other.overlay);
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * b.GetHashCode();
				if (overlay != null)
					hashCode += 1000000009 * overlay.GetHashCode();
			}
			return hashCode;
		}

		public static bool operator ==(PropUnit lhs, PropUnit rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator !=(PropUnit lhs, PropUnit rhs) {
			return !(lhs == rhs);
		}

		#endregion
	}
}




	