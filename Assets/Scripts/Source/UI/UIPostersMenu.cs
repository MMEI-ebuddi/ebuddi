using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMSupport;


public class UIPostersMenu : MonoBehaviour {

	public List<Button> posters = new List<Button>();
	public ViewNewUser viewNewUser;


	void Awake() {
		SetPostersActive(false);
	}

	public void SetPostersActive(bool active) {
		foreach (Button poster in posters) {
			poster.interactable = active;
		}
	}
	

	public void PosterDonningTapped() {
		AnalyticsRecorder.Instance.StopSession();

		if (UILoading.instance != null) UILoading.instance.LoadScene("Scene1");
		else Application.LoadLevel("Scene1");
	}

	public void PosterHazardsTapped() {
		AnalyticsRecorder.Instance.StopSession();
		if (UILoading.instance != null) UILoading.instance.LoadScene("Scene2");
		else Application.LoadLevel("Scene2");
	}

	public void PosterDoffingTapped() {
		AnalyticsRecorder.Instance.StopSession();
		if (UILoading.instance != null) UILoading.instance.LoadScene("Scene3");
		else Application.LoadLevel("Scene3");
	}

	public void PosterTriageTapped() {
		AnalyticsRecorder.Instance.StopSession();
		if (UILoading.instance != null) UILoading.instance.LoadScene("Triage");
		else Application.LoadLevel("Triage");
	}

	public void PosterQuestionnaireTapped() {
		((TitleScene)GameManager.Instance.Scene).AddQuestionnaireStages(QuizType.post);
		GameManager.Instance.Scene.GotoNextState();
	}

//	public void OptionsTapped() {
////		ViewTitleScene.instance.Shi
////		TMMessenger.Send("UIOptions".GetHashCode());
//	}








}
