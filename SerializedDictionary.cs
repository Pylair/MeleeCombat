/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 10/8/2017
 * Time: 7:35 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MeleeCombat
{
	 [Serializable]
	
 	public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
 	{
	     [SerializeField]
	    public List<TKey> keys = new List<TKey>();
	     
	     [SerializeField]
	     public List<TValue> values = new List<TValue>();
	     
	     // save the dictionary to lists
	     public void OnBeforeSerialize()
	     {
	         keys.Clear();
	         values.Clear();
	         foreach(KeyValuePair<TKey, TValue> pair in this)
	         {
	             keys.Add(pair.Key);
	             values.Add(pair.Value);
	         }
	     }
	     
	     // load dictionary from lists
	     public void OnAfterDeserialize()
	     {
	         this.Clear();
	 
	         if(keys.Count != values.Count)
	             throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
	 
	         for(int i = 0; i < keys.Count; i++)
	             this.Add(keys[i], values[i]);
	     }
 

	}
}
