using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIAlert : MonoBehaviour {

	public Text title;
	public Text message;
	public Button[] buttons;
	public Text[] buttonLabels;


	

	public void Show(string pTitle, string pMessage, string[] pButtons) {

		this.gameObject.SetActive(true);
		title.text = pTitle;
		message.text = pMessage;

		if (pButtons.Length == 1) {
			buttons[0].gameObject.SetActive(false);
			buttonLabels[1].text = pButtons[0];
		}
		else if (pButtons.Length == 2) {
			buttons[0].gameObject.SetActive(true);
			buttonLabels[0].text = pButtons[0];
			buttons[1].gameObject.SetActive(true);
			buttonLabels[1].text = pButtons[1];
		}
	}



	public void Button0Tapped() {
		if (UIModalManager.instance != null) UIModalManager.instance.buttonPressed(buttons[0].GetComponentInChildren<Text>().text);
		this.gameObject.SetActive(false);
	}

	public void Button1Tapped() {
		if (UIModalManager.instance != null) UIModalManager.instance.buttonPressed(buttons[1].GetComponentInChildren<Text>().text);
		this.gameObject.SetActive(false);
	}

}
