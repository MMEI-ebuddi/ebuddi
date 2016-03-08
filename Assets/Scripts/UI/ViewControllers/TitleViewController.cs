using UnityEngine;
using System.Collections;

public class TitleViewController : ViewController {

	public static TitleViewController instance;


	public ViewConfiguration viewConfiguration;
	public ViewTitleScene viewTitleScene;
	public ViewWelcome viewWelcome;
	public ViewNewUser viewNewUser;
	public ViewSignIn viewSignIn;


	void Awake() {
		instance = this;
	}



	void Start() {
		viewConfiguration.Hide();
		viewNewUser.Hide();
		viewSignIn.Hide();
		viewWelcome.Hide();
		viewTitleScene.Hide();

		TitleScene titleScene = (TitleScene)GameManager.Instance.Scene;
		if (!titleScene.skipToTheRoom) {
			//config logic here
			if (string.IsNullOrEmpty(UserProfile.sCurrent.Name)) {
				//user not logged in yet
				ToConfigView();
				
			}
			else {
				//coming back from other modules
				StartCoroutine(((TitleScene)GameManager.Instance.Scene).StartTitleScene());
				ToViewTitleScene();
			}
		}
		else {
			//debug skip
			StartCoroutine(((TitleScene)GameManager.Instance.Scene).StartTitleScene());
			ToViewTitleScene();
		}
	}




	public void ToNewUser() {
		ChangeViewWithFade(viewNewUser, 0.3f);
	}

	public void ToSignIn() {
		ChangeViewWithFade(viewSignIn, 0.3f);
	}

	public void HideCurrectViewWithFade() {
		currentView.FadeOut(0.3f);
	}

	public void ToConfigView() {
		ChangeView(viewConfiguration);
	}

	public void ToWelcomeView() {
		ChangeView(viewWelcome);
	}

	public void ToViewTitleScene() {
		ChangeView(viewTitleScene);
	}

}
