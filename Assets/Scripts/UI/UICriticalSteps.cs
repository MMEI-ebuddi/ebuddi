using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UICriticalSteps : MonoBehaviour {

	public static UICriticalSteps instance;

	public Transform imagesRoot;
	public Button fullScreenButton;
	public float fadeInTime = 0.75f;
	public int currentIndex = 0;
	public CanvasGroup canvasGroup;
	public GameObject criticalStepPrefab;
	public bool userControlledSteps = false;
	public List<UICriticalStep> steps = new List<UICriticalStep>();
	public bool allShown = false;




	void Awake() {
		instance = this;
		CleanUp();
	}


	public void Init(List<Texture2D> images, bool userControlledSteps) {

		CleanUp();
		this.userControlledSteps = userControlledSteps;

		foreach (Texture2D image in images) {
			AddImage(image);
		}

		allShown = false;
		currentIndex = 0;
	}



	void AddImage(Texture2D texture) {

		GameObject cell = (GameObject)Instantiate(criticalStepPrefab) as GameObject;
		cell.transform.SetParent(imagesRoot, false);
		RawImage rawImage = cell.GetComponent<RawImage>();
		rawImage.texture = texture;
		rawImage.color = Color.clear;
		UICriticalStep step = cell.GetComponent<UICriticalStep>();
		step.rawImage = rawImage;


		steps.Add(step);

	}


	public void CleanUp() {

		fullScreenButton.gameObject.SetActive(false);
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;

		if (steps.Count > 0) {
			foreach (UICriticalStep step in steps) Destroy(step.gameObject);
			steps = new List<UICriticalStep>();
		}
		allShown = false;
		currentIndex = 0;
		
		
	}



	public void ShowNext(string conversationId = "") {

		if (userControlledSteps) fullScreenButton.gameObject.SetActive(true);
		canvasGroup.interactable = (true);
		canvasGroup.blocksRaycasts = (true);

		if (currentIndex <= steps.Count -1 && !allShown) {

			if (currentIndex > 0) {
				//hide previous arrow
				steps[currentIndex-1].HideArrow();
			}

			iTween.ValueTo(steps[currentIndex].gameObject, iTween.Hash("from", 0, "to", 1f, "time", fadeInTime, "onupdate", "FadeIn"));

			if (!string.IsNullOrEmpty(conversationId)) GameManager.Instance.Buddy.TriggerConversationById(conversationId);

			if (currentIndex < steps.Count-1) currentIndex++;
			else {
				allShown = true;
			}
		}
	}



	public void ShowAll() {

		if (userControlledSteps)  fullScreenButton.gameObject.SetActive(true);
		canvasGroup.interactable = (true);
		canvasGroup.blocksRaycasts = (true);

		for (int i=0; i < steps.Count; i++) {
			iTween.ValueTo(steps[i].gameObject, iTween.Hash("from", 0, "to", 1f, "time", fadeInTime, "delay", (fadeInTime * i * 2f) , "onupdate", "FadeIn"));
		}

	}



}






