/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 11/4/2017
 * Time: 5:44 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;

using System.Collections.Generic;

namespace MeleeCombat.MapGeneration
{
	[Serializable]
	public class BoundedSpaceTags
	{
		public List<FaceTypes> tags;
		
		public BoundedSpaceTags (FaceTypes tag){
			tags = new List<FaceTypes>();
			tags.Add(tag);
		}
		
		public BoundedSpaceTags(IEnumerable<FaceTypes> input){
			tags = new List<FaceTypes>(input);
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			BoundedSpaceTags other = obj as BoundedSpaceTags;
				if (other == null)
					return false;
				return this.GetHashCode().Equals(other.GetHashCode());
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				foreach (FaceTypes tag in tags){
					hashCode += tag.ToString().GetHashCode();
				}
			}
			return hashCode;
		}

		public static bool operator ==(BoundedSpaceTags lhs, BoundedSpaceTags rhs) {
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(BoundedSpaceTags lhs, BoundedSpaceTags rhs) {
			return !(lhs == rhs);
		}

		#endregion
		
		public override string ToString()
		{
			var s = "";
			foreach (FaceTypes tag in tags){
				s += tag + ", ";
				
			}
			return s;
		}

	}
}
