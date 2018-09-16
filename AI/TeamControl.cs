/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/11/2017
 * Time: 6:16 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using MeleeCombat;
using UnityEngine;

namespace MeleeCombat.AI
{
	/// <summary>
	/// Description of TeamControl.
	/// </summary>
	public class TeamControl
	{
		List<AIController> npcs;
		List<MeleeController> teamMembers;
		Faction faction;
		
		GameObject target;
		
		
		public void addTeamMember (MeleeController controller){
			teamMembers.Add(controller);
			controller.Team = this;
			if (MeleeController.isAi(controller)){
				npcs.Add((AIController)controller);
			}
		}
		
		public TeamControl (Faction faction) {
			npcs = new List<AIController>();
			teamMembers = new List<MeleeController>();
			this.faction = faction;
			//target = GameObject.Find("TAG");
		}
		
		const float surroundingDistance = 1;
		
		
		public void surround (GameObject g){
			teamMembers.Sort(new ProximityComparer(g));
			teamMembers.Reverse();
			if (npcs.Count == 0) return;
			var dT = 360/npcs.Count;
			AIController leader = npcs[0];
			Vector3 firstSpoke = -(g.transform.position - npcs[0].gameObject.transform.position).normalized * surroundingDistance;
		
			
			
			for (int i = 0; i < npcs.Count ;i++){	
				var destination = Quaternion.AngleAxis(dT * i ,Vector3.up) * firstSpoke + g.transform.position;
				if (! npcs[i].TacticalControl.isWithinRangeOf(.01f,destination)) {
					npcs[i].TacticalControl.move(destination);
					npcs[i].TacticalControl.lookTo(true);
				} 	
			}
		}
		

	}
}
