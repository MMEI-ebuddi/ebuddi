using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMSupport;


public class UIFinishedPopup : MonoBehaviour {

	public static UIFinishedPopup instance;
	public Text messageLabel;

	private CanvasGroup canvasGroup;



	void Awake() {
		instance = this;
		canvasGroup = this.GetComponent<CanvasGroup>();
		Hide();
	}



	public void Show (ModuleType moduleType) {
		if (moduleType == ModuleType.donning) {
			messageLabel.text = "You have successfully completed the donning section.";
		}
		else if (moduleType == ModuleType.doffing) {
			messageLabel.text = "You have successfully completed the doffing.";
		}
		else {
			Debug.Log(moduleType.ToString() + " not implemented yet");
		}

		canvasGroup.alpha = 1f;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
	}



	public void ShowTriageFinished(int mistakesMade) {
		if (mistakesMade == 0) {
			messageLabel.text = "You have successfully completed the triage section without any mistakes. Well done";
		}
		else {
			messageLabel.text = "You have successfully completed the triage section, however you made " + mistakesMade.ToString() + " mistakes.";
		}

		canvasGroup.alpha = 1f;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
	}





	public void Hide () {
		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}



	public void ConinueTapped() {
		TMMessenger.Send("END_OF_SCENE".GetHashCode());

	}

}
