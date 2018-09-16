/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/19/2017
 * Time: 4:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using UnityEngine.AI;

namespace MeleeCombat
{
	/// <summary>
	/// Description of swordIK.
	/// </summary>
	public class swordIK : MonoBehaviour
	{
		/*public GameObject rightHandle;
		public GameObject leftHandle;
		public bool iKEnabled;*/
		Animator animator;
		public bool switc;
		
		SkinnedMeshRenderer smr;
		
		public void Awake () {
			animator = gameObject.GetComponent<Animator>();
			smr  = GetComponent<SkinnedMeshRenderer>();
		}
		
		public void Update () {
			if (switc){
				switc = false;
				smr.BakeMesh(smr.sharedMesh);
			}
		}
		
		/*public void OnAnimatorIK(){

			if (iKEnabled){
				Debug.Log("ik");
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);
				animator.SetIKPosition(AvatarIKGoal.RightHand,rightHandle.transform.position);
				//animator.SetIKRotation(AvatarIKGoal.RightHand,rightHandle.transform.rotation);
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1);
				animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,1);
				animator.SetIKPosition(AvatarIKGoal.LeftHand,leftHandle.transform.position);
				//animator.SetIKRotation(AvatarIKGoal.LeftHand,leftHandle.transform.rotation);
			} else {
				 animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,0); 

			}
			
			
		}*/
	}
}
