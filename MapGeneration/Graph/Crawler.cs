/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 12/2/2017
 * Time: 8:05 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MeleeCombat.MapGeneration
{

	public struct CrawlerOutput {
		public List<Vector3> output;
		
	}
	
	public class Crawler 
	{
		

		
		public List<Vector3> crawl (Vector3 direction, Vector3 position, HashSet<Vector3> list,int max) {
			List<Vector3> visitedCells = new List<Vector3>();
			var graph = new HashSet<Vector3>(list);
			int count = 0;
			var p = position;
			
			while (count < max){
				var clear = graph.Contains(p);
				if (! clear) break;
			
				visitedCells.Add(p);
				count++;
				p += direction;
				
				
			}
			
			return visitedCells;
		}
		
		
	}
}
