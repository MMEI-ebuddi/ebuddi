using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UIQuestionPopupAnswer : MonoBehaviour {

	public Toggle toggle;
	public Text answerLabel;
	public Image wrongIndicator;


	public void Init(string answer) {
		toggle.isOn = false;
		toggle.group = transform.parent.GetComponent<ToggleGroup>();
		answerLabel.text = answer;
		wrongIndicator.enabled = false;
	}



	public void MarkWrong() {
		wrongIndicator.enabled = true;
	}

}
