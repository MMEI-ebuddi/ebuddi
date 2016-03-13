using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;


public class UIPoster : Button, IPointerEnterHandler, IPointerExitHandler {

	private Vector3 hoverOffset = new Vector3(-0.1f, 0, 0);
	private Vector3 normalPosition;


	void Awake() {
		normalPosition = this.transform.localPosition;
	}

	public void OnPointerEnter (PointerEventData eventData) 
	{
		if (interactable) this.transform.localPosition = normalPosition + hoverOffset;
	}

	public void OnPointerExit (PointerEventData eventData) 
	{
		if (interactable) this.transform.localPosition = normalPosition;
	}
	

}
