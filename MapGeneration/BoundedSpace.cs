/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/27/2017
 * Time: 4:55 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MeleeCombat.MapGeneration
{
	public enum NeighborDirection : int {RIGHT = 0,LEFT = 1, UP = 2, DOWN = 3, FORWARD= 4, BACKWARDS = 5}
	
	public class BoundedSpace {
		
		public static Vector3 roundVector (Vector3 v, int places){
		
			var x = (float)Math.Round(v.x,places);
			var y = (float)Math.Round(v.y,places);
			var z = (float)Math.Round(v.z,places);
			return new Vector3(x,y,z);
		}
	
		public int diagCount;
		public int horizCount;
		
		public GameObject assignedComponent;
		public Bounds bounds;
		
		public BoundedSpaceTags tags {get;set;}
		
		
		
		public Dictionary<Vector3,BoundedSpace> linkedBoundedSpaces {
			get { return neighbors;}
		}
		
		//public Transform transform;
		
		Vector3 forward = Vector3.forward;
		Vector3 up = Vector3.up;
		Vector3 right = Vector3.right;
		
		public Vector3 Right {
			get {return right;}
		}
		
		public Vector3 Forward {
			get {return forward;}
		}
		
		public Vector3 Up {
			get {return up;}
		}
		
		public Vector3 Down {
			get { return -up;}
		}
		
		public Vector3 Back {
			get { return -forward;}
		}
		
		public Vector3 Left {
			get {return -right;}
		}
		
		
		
		public Dictionary<Vector3,BoundedSpace> neighbors;
		
		public BoundedSpace getNeighbor (NeighborDirection dir){
			switch (dir) {
				case (NeighborDirection.RIGHT):
					return getNeighbor(Right);
				case (NeighborDirection.LEFT):
					return getNeighbor(Left);
				case (NeighborDirection.UP):
					return getNeighbor(Up);
				case (NeighborDirection.DOWN):
					return getNeighbor(Down);
				case (NeighborDirection.FORWARD):
					return getNeighbor(Forward);
				case (NeighborDirection.BACKWARDS):
					return getNeighbor(Back);
					
			}
			return null;
		}
		
		public BoundedSpace getNeighbor (Vector3 v){
			var dir = bounds.center + v;
			if (neighbors.ContainsKey(dir)) return neighbors[dir];
			return null;
		}
		
		public void setForwardAndUp (Vector3 f,Vector3 u){
			forward =roundVector(f.normalized,1);
			up= roundVector(u.normalized,1);
			right = roundVector(Vector3.Cross(forward,up).normalized,1);
		}
	

		public void addNeighbor (BoundedSpace space){
			var v = space.bounds.center - bounds.center;
			addNeighbor(v,space);
		}
	
		public void addNeighbor (Vector3 v , BoundedSpace space){
			neighbors[v] = space;
		}
		
		public void setNeighbors (List<BoundedSpace> list){
			foreach (BoundedSpace s in list){
				neighbors[s.bounds.center - bounds.center] = s;
			}	
		}
		
		public void addNeighbors (List<BoundedSpace> additions){
			foreach (BoundedSpace space in additions){
				addNeighbor(space);
			}
		}
		
		public BoundedSpace (FaceTypes tag, Bounds b,Vector3 forward){
			
			tags = new BoundedSpaceTags(tag);
			this.bounds = b;
			neighbors = new Dictionary<Vector3, BoundedSpace>();
			setForwardAndUp(forward, Vector3.forward);
		}

		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			BoundedSpace other = obj as BoundedSpace;
				if (other == null)
					return false;
						return this.bounds == other.bounds;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * bounds.center.GetHashCode();
				hashCode += 324235 * bounds.size.GetHashCode();
			}
			return hashCode;
		}

		public static bool operator ==(BoundedSpace lhs, BoundedSpace rhs) {
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(BoundedSpace lhs, BoundedSpace rhs) {
			return !(lhs == rhs);
		}

		#endregion
		
		
	
		
	}
}
