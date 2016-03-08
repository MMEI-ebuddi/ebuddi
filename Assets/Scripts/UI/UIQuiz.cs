using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIQuiz : MonoBehaviour {


	public RawImage sourceImage;
	public Text instruction;
	public GameObject answerPrototype;
	public Transform otherImagesRoot;
	public List<GameObject> answers = new List<GameObject>();



	void Awake() {
		answerPrototype.GetComponent<RawImage>().enabled = false;
		Hide();
	}

	public void Show(Quiz quiz) {

		sourceImage.texture = quiz.sourceImage;
		sourceImage.enabled = true;
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
		sourceImage.texture = null;
		sourceImage.enabled = false;
		instruction.enabled = false;
		foreach (GameObject answer in answers) {
			Destroy(answer);
		}
		answers = new List<GameObject>();
	}



	public void AnswerTapped(GameObject answer) {

		Scene3 scene3 = (Scene3)GameManager.Instance.Scene;

		scene3.QuizAnswer((Texture2D)answer.GetComponent<RawImage>().texture);

	}



	public void MarkWrongAnswer(Texture2D answer) {

		foreach (GameObject a in answers) {

			if ((Texture2D)a.GetComponent<RawImage>().texture == answer) {
				a.transform.FindChild("wrong").GetComponent<Image>().enabled = true;
			}
		}

	}

}
