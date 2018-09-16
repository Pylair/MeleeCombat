/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 7/21/2017
 * Time: 12:37 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MeleeCombat.AI;


namespace MeleeCombat
{
	/// <summary>
	/// Description of SceneControl.
	/// </summary>
	public class SceneControl : MonoBehaviour{
		
		public static SceneControl sceneCont {
			get{
				return GameObject.Find("Main").GetComponent<SceneControl>();
			}
		}
		
		Dictionary<Faction,TeamControl> teamDict;
		public List<GameObject> characterList;
		
		public void Awake(){
			characterList = new List<GameObject>();
			teamDict = new Dictionary<Faction, TeamControl>();
		}
		
		public void addCharacter (GameObject character){
			var controller = character.GetComponent<MeleeController>();
			var fact = controller.faction();
			TeamControl team;
			if (teamDict.ContainsKey(fact)){
				team = teamDict[fact];
			} else {
				team = new TeamControl(fact);
			}
			teamDict[fact] = team;
			team.addTeamMember(controller);
			characterList.Add(character);
	
		}
		
		public bool isWithinRangeOfCharacter (GameObject character, float radius,int index){
			if (index > characterList.Count - 1) return false;
			return Vector3.Distance(characterList[index].transform.position,character.transform.position) < radius;
		}
	}
}
