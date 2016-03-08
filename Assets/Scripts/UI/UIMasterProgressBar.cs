using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIMasterProgressBar : MonoBehaviour 
{
	private List<UIProgressBarSlider> progressBars = new List<UIProgressBarSlider>();


	void Awake() {
		progressBars = new List<UIProgressBarSlider>(this.transform.GetComponentsInChildren<UIProgressBarSlider>());


		//reset all to grey
		foreach (UIProgressBarSlider slider in progressBars) {
			slider.SetValue(0, false);
		}

		//TODO
		//load user data into the progressbar

	}



	public void SetModuleProgress(ModuleType moduleType, float normalizedValue, bool animated) {
		GetModuleSlider(moduleType).SetValue(normalizedValue, animated);
		Debug.Log(moduleType + " progress bar value " + normalizedValue);
	}



	private UIProgressBarSlider GetModuleSlider(ModuleType moduleType) {
		foreach (UIProgressBarSlider slider in progressBars) {
			if (slider.moduleType == moduleType) return slider;
		}
		return null;
	}



}
