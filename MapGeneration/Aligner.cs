/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/20/2017
 * Time: 2:59 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Threading;

namespace MeleeCombat.MapGeneration
{
	/// <summary>
	/// Description of Aligner.
	/// </summary>
	public class Aligner: MonoBehaviour
	{
		
		public GameObject target;
		public bool trigger;
		public bool randomizeTarget;
		const float zeroThreshHold = .0001f;
		
		public void randomizeTargetParameters () {
			target.transform.localScale =  new Vector3(MeleeController.r.Next(1,5), MeleeController.r.Next(1,5), MeleeController.r.Next(1,5));
			target.transform.eulerAngles = new Vector3(MeleeController.r.Next(0,360), MeleeController.r.Next(0,360), MeleeController.r.Next(0,360));
			target.transform.position += new Vector3(MeleeController.r.Next(-10,10), MeleeController.r.Next(-10,10), MeleeController.r.Next(-10,10));
			
		}
		
		public static void AlignBoundingBoxFaceToBoundingBoxFace (Vector3 aForward, Vector3 aUp,Bounds a, GameObject b){
			
			b.transform.rotation = Quaternion.LookRotation(aForward,aUp);
			a = new Bounds(a.center,b.transform.rotation * a.size);
			Scale(a,b);
			var p1 = a.center;
			Vector3 p2 = Vector3.zero;
			var data = b.GetComponent<ComponentData>();
			if (data != null){
				p2 = data.bounds.center;
			} else {
				p2 = b.GetComponent<MeshRenderer>().bounds.center;
			}
			b.transform.position += p1 - p2;
		}
		
		public static void Align (GameObject a, GameObject b){
			Rotate(a,b);
			Scale (a,b);
			Translate(a,b);
			
		}
		
		public static void Translate (GameObject a, GameObject b){
			var p1 = a.transform.position;
			var p2 = b.transform.position;
			b.transform.position += p1 - p2;
		}

		public static void Rotate (GameObject a, GameObject b){
			b.transform.rotation = Quaternion.LookRotation( a.transform.forward,a.transform.up);	
		}
		
		public static void Scale (GameObject a, GameObject b){
			var e1 = a.transform.eulerAngles;
			var e2 = b.transform.eulerAngles;
			a.transform.eulerAngles = Vector3.zero;
			b.transform.eulerAngles = Vector3.zero;
			
			
			var b1 = a.GetComponent<ComponentData>().bounds.size;
			var b2 = b.GetComponent<ComponentData>().bounds.size;
			
			var x = b2.x;
			var y = b2.y;
			var z = b2.z;
			
			if (Math.Abs(x) < zeroThreshHold) x = b1.x;
			if (Math.Abs(y) < zeroThreshHold) y = b1.y;
			if (Math.Abs(z) < zeroThreshHold) z = b1.z;
			
			if (Math.Abs(b1.x) < zeroThreshHold) {
				b1.x = 1;
				x = 1;
			}
			if (Math.Abs(b1.y) < zeroThreshHold) {
				b1.y = 1;
				y = 1;
			}if (Math.Abs(b1.z) < zeroThreshHold) {
				b1.z = 1;
				z = 1;
			}

			var targetScale = b.transform.localScale;
			b.transform.localScale = new Vector3(Math.Abs(targetScale.x * b1.x/x),
			                                     Math.Abs(targetScale.y *  b1.y/y),
			                                      Math.Abs(targetScale.z *  b1.z/z));
		
			a.transform.eulerAngles = e1;
			b.transform.eulerAngles = e2;
		}
		
		public static void Scale (Bounds a, GameObject b){
			
			var e2 = b.transform.eulerAngles;
			b.transform.eulerAngles = Vector3.zero;
			var b1 = a.size;
			var data = b.GetComponent<ComponentData>();
			Vector3 b2 = Vector3.zero;
			if (data != null){
				b2 = data.bounds.size;
			} else {
				b2 = b.GetComponent<MeshRenderer>().bounds.size;
			}

			var x = b2.x;
			var y = b2.y;
			var z = b2.z;
			
			if (Math.Abs(x) < zeroThreshHold) x = b1.x;
			if (Math.Abs(y) < zeroThreshHold) y = b1.y;
			if (Math.Abs(z) < zeroThreshHold) z = b1.z;
			
			if (Math.Abs(b1.x) < zeroThreshHold) {
				b1.x = 1;
				x = 1;
			}
			if (Math.Abs(b1.y) < zeroThreshHold) {
				b1.y = 1;
				y = 1;
			}if (Math.Abs(b1.z) < zeroThreshHold) {
				b1.z = 1;
				z = 1;
			}

			var targetScale = b.transform.localScale;
			var s =  b.transform.rotation * new Vector3(targetScale.x * b1.x/x,targetScale.y *  b1.y/y,targetScale.z *  b1.z/z);
			s = new Vector3(Math.Abs(s.x),Math.Abs(s.y),Math.Abs(s.z));
			b.transform.localScale = s;
			b.transform.eulerAngles = e2;
		}
		
		public void Align () {
			Align(gameObject,target);
		}
		
		public void Update () {
			if (randomizeTarget){
				randomizeTarget = false;
				randomizeTargetParameters();
			}
			if (trigger){
				trigger = false;
				run();
			}
		}
		
		public GameObject targetSlot;
		public GameObject objectToFillSlot;
		
		public void run (){
			var boxCol = targetSlot.GetComponent<BoxCollider>();
			AlignBoundingBoxFaceToBoundingBoxFace(targetSlot.transform.forward,targetSlot.transform.up,boxCol.bounds,objectToFillSlot);
		}
		
		
		
		
		
		
	}
}
