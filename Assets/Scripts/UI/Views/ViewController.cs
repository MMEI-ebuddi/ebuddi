using UnityEngine;
using System.Collections;

public class ViewController : MonoBehaviour {
	

	public UIView currentView;



	public void ChangeView(UIView targetView) {
		if (currentView != null) currentView.Hide();
		currentView = targetView;
		currentView.Show();
	}
	
	public void ChangeView(UIView targetView, AnimationDirection direction) {
		if (currentView != null) currentView.HideAnimated(direction);
		currentView = targetView;
		currentView.ShowAnimated(direction);
	}

	public void ChangeViewWithFade(UIView targetView, float fadeTime) {

		if (currentView != null) currentView.FadeOut(fadeTime);
		currentView = targetView;
		currentView.FadeIn(fadeTime);
	}



}
