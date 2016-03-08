using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIQuizRightWrong: MonoBehaviour {

	public static UIQuizRightWrong instance;

	public Text instruction;
	public GameObject answerPrototype;
	public Transform otherImagesRoot;
	public List<GameObject> answers = new List<GameObject>();

	public RightWrongQuiz quiz;
	
	void Awake() {
		instance = this;
		answerPrototype.GetComponent<RawImage>().enabled = false;
		Hide();
	}

	public void Show(RightWrongQuiz quiz) {

		this.quiz = quiz;

		instruction.text = quiz.question;
		instruction.enabled = true;

		//spawn other answers
		foreach (Texture2D other in quiz.GetPossibleAnswers()) {

			GameObject newAnswer = (GameObject)Instantiate(answerPrototype);
			newAnswer.transform.SetParent(otherImagesRoot.transform, false);
			RawImage newImage = newAnswer.GetComponent<RawImage>();
			newImage.texture = other;
			newImage.enabled = true;
			answers.Add(newAnswer);
		}

	}


	public void Hide() {
		instruction.enabled = false;
		foreach (GameObject answer in answers) {
			Destroy(answer);
		}
		answers = new List<GameObject>();
	}



	public void AnswerTapped(GameObject answer) {
		GameManager.Instance.Scene.RightWrongQuizAnswer((Texture2D)answer.GetComponent<RawImage>().texture);
	}



	public void MarkWrongAnswer(Texture2D answer) {

		foreach (GameObject a in answers) {

			if ((Texture2D)a.GetComponent<RawImage>().texture == answer) {
				a.transform.FindChild("wrong").GetComponent<Image>().enabled = true;
			}
		}

	}

}
