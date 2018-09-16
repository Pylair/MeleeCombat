/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 5/28/2018
 * Time: 12:27 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MeleeCombat.MapGeneration
{
	/// <summary>
	/// Description of PropOverlayGroup.
	/// </summary>
	public class PropOverlayGroup : MonoBehaviour
	{
		public BoundedSpaceTags tag;
		public List<PropOverlay> propOverlays;
	}
}
