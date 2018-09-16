/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 3/26/2018
 * Time: 5:58 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace MeleeCombat.Diagnostics
{
	/// <summary>
	/// Description of MethodProfiler.
	/// </summary>
	public class MethodProfiler
	{
		public MethodProfiler()
		{
		}
		
		public Stopwatch watch;
		public List<float> recordedTimes;
		public float previousTime;
		
		public void init (int count) {
			watch = Stopwatch.StartNew();
			recordedTimes = new List<float>();
			for (int i = 0; i < count;i++) recordedTimes.Add(0);
			previousTime = 0;
		}
		

		
		public void profile (int i){
			
	
			recordedTimes[i] += watch.ElapsedMilliseconds - previousTime;
			previousTime = watch.ElapsedMilliseconds;
			
		}
		
		public void output () {
			var s = "(";
			for (int i = 0; i < recordedTimes.Count -1;i++){
				var f = recordedTimes[i];
				s += f + ",";
			}
			s += (recordedTimes[recordedTimes.Count -1] + ")");
			UnityEngine.Debug.Log (s);
		}
		
	}
}
