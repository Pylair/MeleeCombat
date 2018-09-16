/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/10/2017
 * Time: 12:26 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using UnityEngine.AI;

namespace MeleeCombat
{
	/// <summary>
	/// Description of test.
	/// </summary>
	public class test : MonoBehaviour
	{
		
		Animator animator;
		NavMeshAgent agent;
		public GameObject target;
		public float distance;
		public float dodgeRadius;
		
		public void Awake () {
			animator = GetComponent<Animator>();
			agent= GetComponent<NavMeshAgent>();
		}
		
		
		public void Update()
		{	
			
			distance = Vector3.Distance(target.transform.position,agent.transform.position);
			if (Input.GetKeyDown(KeyCode.Alpha1) || distance < dodgeRadius){
				testDodge(-3,0);
			}
			
		}
		
		Vector3 dodgeVector;
		
		void testDodge (float u1, float v1){
			agent.updateRotation = false;
			var forward = gameObject.transform.forward.normalized * u1 ;
			var right = gameObject.transform.right.normalized * v1 ;
			dodgeVector = forward  + right;
			
			animator.Play("Dodge",3);
		}
	}
}
