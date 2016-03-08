using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UICriticalStep : MonoBehaviour {


	public RawImage rawImage;
	public Image nextArrow;
	

	void FadeIn(float alpha) {

		rawImage.color = new Color(1f, 1f, 1f, alpha);
		if (UICriticalSteps.instance.userControlledSteps) nextArrow.color = new Color(1f, 1f, 1f, alpha);
		else nextArrow.color = Color.clear;
	}


	public void HideArrow() {
		nextArrow.color = new Color(1f, 1f, 1f, 0);
	}


}
