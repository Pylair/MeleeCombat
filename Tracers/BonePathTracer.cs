/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/2/2017
 * Time: 9:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MeleeCombat.Tracers
{
	/// <summary>
	/// Description of BonePathTracer.
	/// </summary>
	public class BonePathTracer : MonoBehaviour
	{
		Animator animator;
		public GameObject parent;
		public List<Vector3> pointPaths;
		public string animStateName = "right";
		public int layerIndex = MeleeController.attackLayer;
		Vector3 origin;
		GameObject tracerPoint;
		public MeleeController controller;
		
		public void Start()
		{
			animator = parent.GetComponent<Animator>();
			pointPaths = new List<Vector3>();
			
			controller = parent.GetComponent<MeleeController>();
			controller.SwingBegan += enableSwingRecording;
			controller.SwingFinished += disableSwingRecording;
		}
		
		//float dT = 0;
		//float interval = .05f;
		bool recording = false;
		
		bool firstIteration = true;
		
		void enableSwingRecording (MeleeController sender) {
			recording = true;
			tracerPoint = GetComponentInChildren<Tracer>().gameObject;
			origin = gameObject.transform.position;
			index = 0;
		}
		
		void disableSwingRecording (MeleeController sender ){
			recording = false;
			if (firstIteration) firstIteration = false;
			
		}
		
		int index = 0;
		public void Update () {
			if (recording) {
				if (firstIteration){
					record();
				} else {
					play();
				}
				
			}
		}
		
		public void play(){
			if (index >= pointPaths.Count) return;
			var v = pointPaths[index];
			v = gameObject.transform.rotation * v;
			
			Debug.DrawLine(origin + v,tracerPoint.transform.position,Color.blue,float.PositiveInfinity);
			index++;
		}
		
		public void record () {
			var v = gameObject.transform.position - origin;
			pointPaths.Add( Quaternion.Inverse(gameObject.transform.rotation) * v);
			Debug.DrawLine(origin + v,tracerPoint.transform.position,Color.red,float.PositiveInfinity);
		}
	}
}
