/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 11/7/2017
 * Time: 5:18 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MeleeCombat.MapGeneration
{
	/// <summary>
	/// Description of VoxelIterators.
	/// </summary>
	public static class VoxelIterators
	{
		
		public static IEnumerable<Vector2> VonNeumanNeighbors (){
			for (int i = -1 ; i < 2; i++){
				for (int j = -1; j < 2;j++){
					if (Math.Abs(i) + Math.Abs(j) == 1){
						yield return new Vector2(i,j);
					}
				}
			}
		}
		
		public static IEnumerable<Vector2> FullNeighbors (){
			for (int i = -1 ; i < 2; i++){
				for (int j = -1; j < 2;j++){
					if (! (Math.Abs(i) == 0 & Math.Abs(j) == 0)){
						yield return new Vector2(i,j);
					}
				}
			}
		}
		
		public static IEnumerable<Vector3> VonNeumanNeighbors3D (){
			for (int i = -1 ; i < 2; i++){
				for (int j = -1; j < 2;j++){
					for (int k = -1; k < 2;k++){
						if (Math.Abs(i) + Math.Abs(j) + Math.Abs(k) == 1){
							yield return new Vector3(i,j,k);
						}
					}
					
				}
			}
		}
		
		public static IEnumerable<Vector3> FullNeighbors3D (Vector3 extents){
			for (int i = (int)-extents.x; i < extents.x;i++){
				for (int j = -(int)extents.y;j < extents.y;j++){
					for (int k = -(int)extents.z; k < extents.z;k++){
						if (! (i == 0 && j == 0 && k == 0)){
							yield return new Vector3(i,j,k);
						}
						
					}
				}
			}
		}
		
	}
}
