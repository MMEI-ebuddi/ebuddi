using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMSupport;

public class Equipment : MonoBehaviour 
{	
    public BaseActionScriptableObject ItemSO;
	private Vector3 startingPosition;

	public  void Awake() {
		startingPosition = this.transform.position;
//		base.Awake();
	}

	void OnMouseEnter() {
		Highlight();
	}

	void OnMouseExit() {
		UnHighlight();
	}



	public  void Highlight()
	{
		iTween.Stop(this.gameObject);
		iTween.MoveTo(this.gameObject, iTween.Hash("position", startingPosition + new Vector3(0, 0.04f, 0),
		                                           "time", 0.10f, "islocal", false));
//		base.Highlight();
	}


	public  void UnHighlight() {
		iTween.Stop(this.gameObject);
		iTween.MoveTo(this.gameObject, iTween.Hash("position", startingPosition,
		                                           "time", 0.25f, "islocal", false));
//		base.UnHighlight();
	}



	void OnMouseDown() {
		if (EquipmentManager.Instance.CanChooseEquipment(this)) {
			EquipmentManager.Instance.ItemClicked(ItemSO);
		}
	}

}
