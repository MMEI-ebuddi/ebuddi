using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UIDisclaimer : MonoBehaviour {

	public Button continueButton;
	public Text buildText;



	void Start() {
		buildText.text = string.Format("Alpha Build {0}.{1}.{2}", GameManager.sBuildVersion[0].ToString(), GameManager.sBuildVersion[1].ToString(), GameManager.sBuildVersion[2].ToString());

		continueButton.gameObject.SetActive(false);
		float activateButtonDelay = 5f;
		if (Application.isEditor) activateButtonDelay = 0.5f;

		Invoke("ActivateContinueButton", activateButtonDelay);
	}


	void ActivateContinueButton() {
		continueButton.gameObject.SetActive(true);
	}


	public void ContinueClicked()
	{
		if (UILoading.instance != null) UILoading.instance.LoadScene("Title");
		else Application.LoadLevel("Title");
	}

}
