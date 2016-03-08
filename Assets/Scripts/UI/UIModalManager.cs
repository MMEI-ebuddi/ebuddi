using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIModalManager : MonoBehaviour {
	
	public static UIModalManager instance;
	public UIAlert customAlert;

	void Awake() {
		instance = this;
	}
	

	
	public void buttonPressed(string button) {
		this._buttonPressed = button;
	}
	
	//Variables
	private string _buttonPressed = "none";
	
	public IEnumerator ShowAlert(string title, string message, string[] buttons, System.Action<string> pressed) {

		Debug.Log("Error " + message);

		this._buttonPressed = "none";
		if (buttons.Length > 2) {
			Debug.LogWarning("UIPopup.ShowPopup can only show 2 buttons.", gameObject);
			yield return null;
		}

		customAlert.Show(title, message, buttons);
	
		while(this._buttonPressed == "none")
			yield return new WaitForEndOfFrame();

		pressed(_buttonPressed);
		
	}


//	public void ShowActivity() {
//		activityView.SetActive(true);
//	}
//
//	public void HideActivity() {
//		activityView.SetActive(false);
//	}
//
//
//	void Update() {
//		if (activityView != null) {
//			if (activityView.activeSelf) activitySpinner.transform.Rotate(new Vector3(0, 0, -150f * Time.deltaTime));
//		}
//	}
//





}
