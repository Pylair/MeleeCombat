/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/4/2017
 * Time: 12:03 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MeleeCombat.UI
{
	/// <summary>
	/// Description of ProgressBar.
	/// </summary>
	public class ProgressBar : MonoBehaviour
	{
		public Image background;
		public Image foreground;
		
		Vector2 originalOffset;
		
		[Range(0,1)]
		public float value;
		
		float previousValue;
		
		public void Start () { 
			originalOffset = foreground.rectTransform.offsetMax;
		}
		
		public void Update () {
			if (previousValue != value){
				setValue(value);
			}
			previousValue = value;
		}
		
		public void setValue (float percentage) {
			percentage = Math.Min(percentage,1);
			percentage = Math.Max(percentage,0);
			foreground.fillAmount = percentage;
		}
	}
}
