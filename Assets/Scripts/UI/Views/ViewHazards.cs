using UnityEngine;
using System.Collections;

public class ViewHazards: ViewScene {

	public static ViewHazards instance;

	public UIRisks uiRisks;



	public override void Awake() {
		instance = this;
		base.Awake();
	}

	void Start () {
	
		//set real progress bar values

	}



	public void ShowRisksUI() {
		uiRisks.gameObject.SetActive(true);
	}
	
	public void HideRisksUI() {
		uiRisks.gameObject.SetActive(false);
	}
}
