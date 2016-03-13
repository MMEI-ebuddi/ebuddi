using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class UIHighlightManager : MonoBehaviour {
	
	public static UIHighlightManager instance;
	public List<UIHighlightable> currentHighlight = new List<UIHighlightable>();

	
	private float flashTime = 0.5f;
	private float flashTimeLeft = 0;

	void Awake() {
		instance = this;
	}

	
	public void HighlightAll() {
		foreach (UIHighlightable highlightable in FindObjectsOfType<UIHighlightable>()) highlightable.Highlight();
	}


	public void UnhighlightAll() {

		foreach (UIHighlightable highlightable in currentHighlight) highlightable.UnHighlight();
		currentHighlight.Clear();
	}


	public void HighlightObject(UIHighlightable newHighlight) {
	
		//clean old highlight
		if (currentHighlight.Count > 0) {
			UnhighlightAll();
		}

		List<UIHighlightable> newHighlights = new List<UIHighlightable>();
		newHighlights.Add(newHighlight);
		HighlightObjects(newHighlights);
		flashTimeLeft = flashTime;
	}


	public void HighlightObjects(List<UIHighlightable> newHighlight) {

		//clean old highlight
		if (currentHighlight.Count > 0) {
			UnhighlightAll();
		}

		currentHighlight.Clear();

		foreach (UIHighlightable highlightable in newHighlight) {
			currentHighlight.Add(highlightable);
			highlightable.Highlight();
		}

		flashTimeLeft = flashTime;
	}

	

	void Update () {
	

		if (currentHighlight.Count > 0) {
			flashTimeLeft -= Time.deltaTime;

			if (flashTimeLeft < 0) {
				flashTimeLeft = flashTime;

				foreach (UIHighlightable highlightable in currentHighlight) {
					if (highlightable.isHighlighted) highlightable.UnHighlight();
					else  highlightable.Highlight();
				}

			}
		}



	}




}











