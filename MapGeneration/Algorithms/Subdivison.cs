/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 6/17/2018
 * Time: 12:40 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace MeleeCombat.MapGeneration.Algorithms
{

	
	public class Subdivison
	{
		public Subdivison(int minX, int minY)
		{
			this.minY = minY;
			this.minX = minX;
			minVolume = (2* minX - 1) * (2 *minY -1);
		}
		
		public System.Random r = new System.Random();
		public int minX ;
		public int minY ;
	
		public int minVolume;
		public int maxVolume = 36;
		
		public float ratio = 3;
		
		Rect[] divideHorizontal (Rect rect){
			var w = (int)rect.width;
			var h = (int)rect.height;
			var x0 = (int)rect.min.x;
			var y0 = (int)rect.min.y;
			var x1 = (int)rect.max.x;
			var y1 = (int)rect.max.y;
			var w1 =  r.Next( minX, (w - minX));
			var w2 = w - w1;
			var r1 = new Rect(x0,y0,w1,h);
			var r2 = new Rect(x0 + w1,y0,w2,h);
			return new Rect[]{r1,r2};
		}
		
		Rect[] divideVertical (Rect rect){
			var w = (int)rect.width;
			var h = (int)rect.height;
			var x0 = (int)rect.min.x;
			var y0 = (int)rect.min.y;
			var x1 = (int)rect.max.x;
			var y1 = (int)rect.max.y;
			var h1 = r.Next(minY, (h - minY));
			var h2 = h - h1;
			var r1 = new Rect(x0,y0,w,h1);
			var r2 = new Rect(x0,y0 + h1,w,h2);
			return new Rect[]{r1,r2};
		}
		
		public void subdivide (int iter,Rect rect, List<Rect> list){
	
			if (r.NextDouble() < .55f && rect.width * rect.height < maxVolume && rect.width/rect.height < ratio && rect.width/rect.height > 1/ratio){
				list.Add(rect);
				//drawRect(rect,Color.red);
				return;
			}

			var w = (int)rect.width;
			var h = (int)rect.height;

			
			Rect r1;
			Rect r2;

				
			if ( h > ratio * w){
				var rects = divideVertical(rect);
				r1 = rects[0];
				r2 = rects[1];
			} else if (w > ratio * h){
				var rects = divideHorizontal(rect);
				r1 = rects[0];
				r2 = rects[1];
			} else if (h > 2 * minY  && w > 2 * minX ){
				if (r.NextDouble() < .5){
					var rects = divideVertical(rect);
					r1 = rects[0];
					r2 = rects[1];
				} else {
					var rects = divideHorizontal(rect);
					r1 = rects[0];
					r2 = rects[1];
				}
			} else if (h > 2 * minY ){
				var rects = divideVertical(rect);
				r1 = rects[0];
				r2 = rects[1];
			} else if (w > 2 * minX ){
				var rects = divideHorizontal(rect);
				r1 = rects[0];
				r2 = rects[1];
			} else {
				//drawRect(rect,Color.red);
				
				list.Add(rect);
				return;
			}

			subdivide(iter - 1,r1,list);
			subdivide(iter - 1,r2,list);
			
			
		}
		
		void drawRect (Rect rect, Color color){
			Debug.DrawLine(rect.min,rect.min + new Vector2(rect.width,0),color,float.PositiveInfinity,false);
			Debug.DrawLine(rect.min,rect.min + new Vector2(0,rect.height),color,float.PositiveInfinity,false);
			Debug.DrawLine(rect.max,rect.max - new Vector2(rect.width,0),color,float.PositiveInfinity,false);
			Debug.DrawLine(rect.max,rect.max - new Vector2(0,rect.height),color,float.PositiveInfinity,false);
		}
		
		
	}
}
