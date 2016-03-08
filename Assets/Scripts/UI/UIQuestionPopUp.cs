using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class UIQuestionPopUp : MonoBehaviour 
{
	public delegate void OnAnswerGivenHandler(Question question, string answer);
	public event OnAnswerGivenHandler OnAnswerGiven;


	public UIQuestionPopupAnswer answerPrefab;
	public Transform answersRoot;
	public Text questionLabel;

	private Question question;


	void Awake() {
		answerPrefab.gameObject.SetActive(false);
	}



	public void Show(Question question)
	{
		this.question = question;
		questionLabel.text = question.question;

		//populate answers
		foreach (string possibleAnswer in question.possibleAnswers) {
			UIQuestionPopupAnswer answerCopy = (UIQuestionPopupAnswer)Instantiate(answerPrefab);
			answerCopy.gameObject.SetActive(true);
			answerCopy.transform.SetParent(answersRoot, false);
			answerCopy.Init(possibleAnswer);
		}



		gameObject.SetActive(true);
	}



	public void Hide()
	{
		gameObject.SetActive(false);

		//cleanup
		foreach (Transform child in answersRoot) Destroy(child.gameObject);

	}




	public void OKButton()
	{
		if (!string.IsNullOrEmpty(SelectedAnswer())) {

			if (OnAnswerGiven != null) OnAnswerGiven(question, SelectedAnswer());
		}

		//mark wrong answers
		foreach (UIQuestionPopupAnswer answer in answersRoot.GetComponentsInChildren<UIQuestionPopupAnswer>()) {
			if (answer.toggle.isOn) {
				answer.MarkWrong();
			}
		}


	}






	private string SelectedAnswer() {

		string result = "";

		foreach (UIQuestionPopupAnswer answer in answersRoot.GetComponentsInChildren<UIQuestionPopupAnswer>()) {
			if (answer.toggle.isOn) {
				result = answer.answerLabel.text;
			}
		}
		return result;

	}





}
