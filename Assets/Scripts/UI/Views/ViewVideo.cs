using UnityEngine;
using System.Collections;
using MMT;

public class ViewVideo : UIView
{
	public static ViewVideo instance;


	public MobileMovieTexture mmt;


	void Awake()
	{
		instance = this;
	}



	public override void ViewWillAppear() 
	{
		base.ViewWillAppear();
		mmt.onFinished += OnFinished;
	}


	void Start() {
		LoadVideo("MovieSamples/sample.ogv");
	}

	public override void ViewWillDisappear() 
	{
		base.ViewWillDisappear();
		mmt.onFinished -= OnFinished;
	}


	public void LoadVideo(string path)
	{
		if(mmt.IsPlaying) mmt.Stop();
		mmt.gameObject.GetComponent<MobileMovieTexture>().Path = path;

		Invoke("Play", 0.5f); 

	}


	public void OnFinished(MobileMovieTexture sender)
	{
		Debug.Log("Video finished playing");
	} 




	void Play() {
		mmt.Play();
	}


}
