using UnityEngine;
using System.Collections;
using MMT;

public class ViewTitleScene : ViewScene {
	
	public GameObject videoPanel;
	public MobileMovieTexture mmt;
	public UIOptions optionsPanel;
	
	
	public override void Awake() {
		base.Awake();
	}

	void Start() {
		Show();
	}



	public void LoadVideo(string path)
	{
		if(mmt.IsPlaying) mmt.Stop();
		mmt.gameObject.GetComponent<MobileMovieTexture>().Path = path;

		Play();
	}


	public void StopVideo() {
		if(mmt.IsPlaying) 
			mmt.Stop();
		videoPanel.SetActive(false);
	}

	void Play() {
		videoPanel.SetActive(true);
		mmt.Play();
	}
	
	
	public void OnFinished(MobileMovieTexture sender)
	{
		Debug.Log("Video finished playing");
	} 



	public void LogoutTapped() {

		StartCoroutine(UIModalManager.instance.ShowAlert("Logout user?", "Are you sure you want to log out?", new string[] {"Cancel", "Logout"}, delegate(string buttonPressed) {

			if (buttonPressed == "Logout") {

				Debug.Log("Proper logout to be added here with destroing data");
				AnalyticsRecorder.Instance.StopSession();
				UserProfile.sCurrent = null;
				Application.LoadLevel("Title");

			}
		}));

	}



	public void ShowOptions() {
		optionsPanel.gameObject.SetActive(true);
	}


	public void HideOptions() {
		optionsPanel.gameObject.SetActive(false);
	}

}
