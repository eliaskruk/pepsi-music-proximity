using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class scrollFix : MonoBehaviour, IBeginDragHandler, IEndDragHandler {
	public Vector2 startPos;
	public int maxDistance = 30;

	public void OnBeginDrag (PointerEventData data){
		startPos = data.position;
	}
	
	public void OnEndDrag (PointerEventData data){
		float distance = Vector2.Distance (startPos, data.position);
		
		if (distance < maxDistance) {
			GameObject elemSelec = data.pointerCurrentRaycast.gameObject;

			Transform transf = elemSelec.transform;
			while (transf.parent != null){
				if (transf.parent.GetComponent<Button> () != null)
				{
					//Debug.Log(transf.parent.GetComponent<Button> ());
					transf.parent.GetComponent<Button> ().onClick.Invoke();
					break;
				}
				transf = transf.parent.transform;
			}
		} 
	}
}