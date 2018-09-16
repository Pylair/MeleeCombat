/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 7/9/2017
 * Time: 10:26 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;

namespace MeleeCombat
{
	/// <summary>
	/// Description of Faction.
	/// </summary>
	public class Faction : MonoBehaviour
	{
		public int faction;
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			Faction other = obj as Faction;
				if (other == null)
					return false;
						return this.faction == other.faction;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * faction.GetHashCode();
			}
			return hashCode;
		}

		public static bool operator ==(Faction lhs, Faction rhs) {
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Faction lhs, Faction rhs) {
			return !(lhs == rhs);
		}

		#endregion
	}
}
