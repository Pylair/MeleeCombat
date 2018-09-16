/*
 * Created by SharpDevelop.
 * User: erict
 * Date: 6/30/2018
 * Time: 8:57 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;


namespace MeleeCombat.MapGeneration
{
	
	public class RoomTemplateBuilder : MonoBehaviour
	{
		public List<PropOverlay> propOverlayList;
		public ListBox listBox;
		
		RectTransform rectTransform;
		Texture2D texture;
		public int width;
		public int height;
		
		public StandaloneInputModule inputModule;
		public EventSystem eventSystem;
		
		public Vector2 imageLowerCorner;
		
		float scaleFactorX;
		float scaleFactorY;
		
		Vector2 getXYFromPosition (int i){
			var y =  ((int)(i / height));
			var x =(i) % height;
			return new Vector2(x,y);
		}
		
		Vector2 getNormalizedPosition (Vector2 worldPosition){
			var normalizedPosition = (worldPosition - imageLowerCorner);
			normalizedPosition.x /= rectTransform.localScale.x;
			normalizedPosition.y /= rectTransform.localScale.y;
			return normalizedPosition;
		}
		
		public Dictionary<Bounds,PropOverlay> propDict;
		
		public void paintBounds (Bounds b, Color c){
			var min = b.min;
			
			for (int i = 0; i <= b.size.x ;i++){
				for (int j = 0; j <= b.size.y ;j++){
					var x = i + (int)min.x;
					var y = j + (int)min.y;
					texture.SetPixel(x,y,c);
				}
			}
			texture.Apply();
		}
		
		
		public void OnMouseClick () {
			if (listBox.GetSelectedIndex() < 0) return;
			
			var normalizedPosition = getNormalizedPosition(inputModule.input.mousePosition);
			var x = (int)(normalizedPosition.x / scaleFactorX);
			var y = (int)(normalizedPosition.y / scaleFactorY);
			var color = texture.GetPixel(x,y);
			
			var prop = propOverlayList[listBox.GetSelectedIndex()];
			var size = prop.size;
			
			
			var maxBounds = new Bounds(Vector3.zero,Vector3.zero);
			maxBounds.Encapsulate(new Vector3(width,height,0));
			
			
			if (color.Equals(Color.black)) {
				color = Color.white;
			} else {
				color = Color.black;
			}
			
			var v = new Vector3(x,y);
			
			
			var pointGroup = new List<Vector2>();
			Bounds bounds = new Bounds(v,Vector3.zero);
			bounds.Encapsulate(v);
		//	bounds.Encapsulate(v + Vector3.forward/2);
		//	bounds.Encapsulate(v - Vector3.forward/2);
		//	bounds.Encapsulate(v + Vector3.one/2);
		//	bounds.Encapsulate(v - Vector3.one/2);
			
			for (int i = 0; i < size.x;i++){
				for (int j = 0; j < size.y;j++){
					if (i + x >= width || j + y >= height) return;
					var v0 = new Vector3(i +x, j + y);
					//bounds.Encapsulate(v0 + Vector3.forward/2);
					//bounds.Encapsulate(v0 - Vector3.forward/2);
					bounds.Encapsulate(v0);
					//bounds.Encapsulate(v0 + Vector3.one/2);
					//bounds.Encapsulate(v0 - Vector3.one/2);
					
				}
			}
			
			foreach (Bounds b in propDict.Keys.ToList()){
				if (bounds.Intersects(b)){
					
					propDict.Remove(b);
					paintBounds(b,Color.white);
	
				} 
			}
		
			paintBounds(bounds,color);
			
			if (color.Equals(Color.black)){
				propDict[bounds] = prop;
			} else {
				propDict.Remove(bounds);
			}
			

			
			texture.Apply();
		}
		
		public void Start()
		{
			
			propDict = new Dictionary<Bounds, PropOverlay>();
			var stringOps = new List<String>();
			foreach (PropOverlay overlay in propOverlayList) stringOps.Add(overlay.name);
			listBox.optionStrings = stringOps.ToArray();
		
			
			
			rectTransform = GetComponent<RectTransform>();
			rectTransform.localScale = new Vector3(1.0f * width/height,1,1);
			texture = new Texture2D(width,height);
			texture.filterMode = FilterMode.Point;
			GetComponent<RawImage>().texture = texture;
			var colors = texture.GetPixels();
			for (int i = 0; i < colors.Length;i++) {
				colors[i] = Color.white;
			}
			texture.SetPixels(colors);
			texture.Apply();
			
			scaleFactorX = rectTransform.rect.width / width;
			scaleFactorY = rectTransform.rect.height / height;
			
	
			
			Vector3[] corners = new Vector3[4];
			GetComponent<RawImage>().rectTransform.GetWorldCorners(corners);
			imageLowerCorner= corners[0];
		}
		
		public void outputRoom () {
			foreach (Bounds b in propDict.Keys){
				
				var obj = GameObject.Instantiate(propDict[b].prop);
				b.Encapsulate(b.min - Vector3.one/2);
				b.Encapsulate(b.max + Vector3.one/2);
				
				Aligner.AlignBoundingBoxFaceToBoundingBoxFace(Vector3.up, Vector3.forward,b,obj);
				
			}
			/*var colors = texture.GetPixels();
			
			for (int i = 0; i < colors.Length;i++){
				var col = colors[i];
				if (col.Equals(Color.white)){
					var v = getXYFromPosition(i);
					GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = v;
				}
			}*/
			for (int i = 0; i < width;i++){
				for (int j = 0; j < height;j++){
					var c = texture.GetPixel(i,j);
					if (c.Equals(Color.white)){
						var  v = new Vector2(i,j);
						GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = v;
					}
					
				}
			}
		}
		
		public bool resize = false;
		public bool finalize;
		
		public void Update(){
			
			if (resize){
				resize = false;
				Start();
			}
			if (finalize){
				finalize = false;
				outputRoom();

			}
		}
		
		
	}
}
