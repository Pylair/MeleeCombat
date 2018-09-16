/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/1/2017
 * Time: 7:13 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;

namespace MeleeCombat.AI
{
	/// <summary>
	/// Description of RotatingCube.
	/// </summary>
	public class RotatingCube : MonoBehaviour
	{
		GameObject centerCube;
		Vector3 center;
		float distance;
		
		public void Start(){
			centerCube = GameObject.Find("Center");
			center = GameObject.Find("Center").transform.position;
			distance= Vector3.Distance(center,gameObject.transform.position);
		}
		
		public void Update() {
			var v1 = gameObject.transform.forward;
			v1.Scale(new Vector3(1,0,1));
			var v2 = centerCube.transform.forward;
	
			var angle = Vector3.Angle(v1,v2);
			var rot = Quaternion.AngleAxis(angle,Vector3.up);
			
			if (Math.Abs(Vector3.Angle(v2,rot * v1)) > 1){
				rot = Quaternion.Inverse(rot);
			}
			
			
			var originalRot = transform.localRotation;
			var localRot = transform.localRotation;
			localRot = Quaternion.Slerp(localRot,localRot * rot, Time.deltaTime);	
			transform.localRotation = localRot;
			
		//	gameObject.transform.position = new Vector3((float)Math.Cos(a) * distance,gameObject.transform.position.y,(float)Math.Sin(a) * distance);
			
			
		}
	}
}
