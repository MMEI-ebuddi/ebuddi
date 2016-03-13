using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class UIQuestion : MonoBehaviour {

	public delegate void OnAnswerGivenHandler (Question question, string answer);
	public static event OnAnswerGivenHandler OnAnswerGiven;


	public GameObject answerPrototype;
	public Transform answersRoot;
//	public Text questionText;
	public List<GameObject> answers = new List<GameObject>();
	public Question question;
	private CanvasGroup canvasGroup;


	public void Awake() {
		answerPrototype.SetActive(false);
		canvasGroup = this.GetComponent<CanvasGroup>();
		Hide();

	}

	public void Show(Question question) {

		this.question = question;
//		UIManager.Instance.BringUpDialog(UIManager.eUIReferenceName.BuddyDialog, question.question);

		RefreshButtons();

		canvasGroup.alpha = 1f;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;

	}


	public void Hide() {
		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;

		foreach (GameObject answer in answers) Destroy(answer);
		answers = new List<GameObject>();
	}

	

	public void Answer(GameObject answer) {

		if (OnAnswerGiven != null) OnAnswerGiven(question, answer.GetComponent<Text>().text);

	}



	public void RefreshButtons() {

		foreach (GameObject answer in answers) Destroy(answer);
		answers = new List<GameObject>();

		//possible answers without already given answers
		List<string> answersLeft = new List<string>();
		answersLeft.AddRange(question.possibleAnswers);
		//remove answers user gave already
		foreach (string answerGiven in question.answersGiven)
		{
			Debug.Log(answerGiven);
			answersLeft.Remove(answerGiven);
		}

		foreach (string answer in answersLeft) {
			
			GameObject newAnswer = (GameObject)Instantiate(answerPrototype);
			newAnswer.transform.SetParent(answersRoot, false);
			
			SceneQuestion sceneQuestion = (SceneQuestion)question;
			
			newAnswer.transform.localPosition = sceneQuestion.answersScreenSpacePositions[question.possibleAnswers.IndexOf(answer)];
			
			newAnswer.SetActive(true);
			newAnswer.GetComponent<Text>().text = answer;
			answers.Add(newAnswer);
		}
	}








}
