/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/23/2017
 * Time: 12:30 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MeleeCombat
{
	/// <summary>
	/// Description of ApplyTransform.
	/// </summary>
	public class ApplyTransform : MonoBehaviour
	{
		
		public bool trigger;
		public void Update () {
			if (trigger){
				trigger = false;
				applyTransform(gameObject);
			}
		}
		
		public static void applyTransform (GameObject g){
			Transform parentTransform = g.transform;
			descaleChildren(g);
			var c = g.GetComponent<MeshCollider>();
			var size = g.GetComponent<MeshRenderer>().bounds;
			c.bounds.SetMinMax(size.min,size.max);
		}

		public static void descaleChildren (GameObject g){
			GameObject parent = null;
			if (g.transform.parent != null){
				parent = g.transform.parent.gameObject;
				parent.transform.DetachChildren();
			} 
			foreach (Transform t in g.transform){
				descaleChildren(t.gameObject);
			}
			fixTransform(g,g.transform);
			if (parent != null){
				var distance = parent.transform.position - g.transform.position;
				g.transform.parent = parent.transform;
				g.transform.localScale = new Vector3(1,1,1);
				g.transform.localEulerAngles = Vector3.zero;
				g.transform.localPosition = -distance;
			}
		}
		
		public static int[] flip (int[] triangles, Vector3 scale){
			int count = 0;
			if (scale.x < 0) count++;
			if (scale.y < 0) count++;
			if (scale.z < 0) count++;
			if (count %2 == 0) return triangles;
			for (int i = 0; i < triangles.Length-2;i+= 3){
				var x = triangles[i];
				triangles[i] = triangles[i + 2];
				triangles[i+2] = x;
			}
			return triangles;

		}
	
	
		public static void fixTransform (GameObject g,Transform t){
			
			var mf = g.GetComponent<MeshFilter>();
			var m = mf.sharedMesh;
			var sectors = m.uv2;
			var verts = m.vertices;
			var triangles = m.triangles;
			var matrix = t.localToWorldMatrix;
			var quat =t.rotation;
			var uvs = m.uv;
			var normals = new List<Vector3>(m.normals);
			triangles = flip(triangles,g.transform.localScale);

			for (int i = 0; i <verts.Length;i++ ){
				verts[i] = matrix.MultiplyPoint3x4(verts[i]);
				verts[i] -= t.position;
			}
			
			m = new Mesh();
			m.vertices = verts;
			m.normals = normals.ToArray();
			m.uv = uvs;
			m.uv2 = sectors;
			m.triangles = triangles;
			m.UploadMeshData(false);
			mf.mesh = m;
			g.transform.localScale = Vector3.one;
			g.transform.localEulerAngles = Vector3.zero;
		}
	}
}
