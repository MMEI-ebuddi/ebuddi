using UnityEngine;
using UnityEngine.UI;
using TMSupport;
using MMT;
using TMSupport;


public class MovieDialog : UIDialog
{
	public RawImage m_RawImage;
    public Button ButtonOffset;
	public MobileMovieTexture mmt;
	public Button offsetButton;


	private AudioSource audioSource;
	private CanvasGroup canvasGroup;

	private string currentVideoName = "";


	void Awake() {
		canvasGroup = this.GetComponent<CanvasGroup>();
		audioSource = this.GetComponent<AudioSource>();
	}


	public override void Show() {
		canvasGroup.alpha = 1f;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
	}
	
	public override void Hide() {

		canvasGroup = this.GetComponent<CanvasGroup>();
		audioSource = this.GetComponent<AudioSource>();

		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}
	

    public void OffsetClicked()
    {
        Rect uv = m_RawImage.uvRect;
        uv.x = (uv.x == 0.0f) ? 0.5f : 0f;
        m_RawImage.uvRect = uv;
    }


	public void PlayVideo(string videoName, AudioClip audio = null) {

		Rect uvRect = m_RawImage.uvRect;
		uvRect.x = 0;

		//is offset video?
		if (videoName.Contains("offset")) {
			uvRect.width = 0.5f;
			offsetButton.gameObject.SetActive(true);
		}
		else {
			//normal video
			uvRect.width = 1f;
			if (offsetButton != null) offsetButton.gameObject.SetActive(false);
		}
		m_RawImage.uvRect = uvRect;

		currentVideoName = videoName;

		Show();

		if(mmt.IsPlaying) mmt.Stop();
		mmt.Path = videoName + ".ogv";
		mmt.Play();


		if (audio != null) {
			audioSource.clip = audio;
			audioSource.Play();
		}
	}


	public void Replay() {

		if (mmt.IsPlaying) mmt.Stop();

		if (audioSource.clip != null) PlayVideo(currentVideoName, audioSource.clip);
		else PlayVideo(currentVideoName);
	}



	public void MovieFinished()
	{
		mmt.Stop();
		if (audioSource.isPlaying) audioSource.Stop();
		TMMessenger.Send("MediaClipFinished".GetHashCode(), 0, currentVideoName);

		currentVideoName = "";

		Hide();
	}



	void Update() {
		if (!string.IsNullOrEmpty(currentVideoName)) {
			if (Input.GetKeyDown(KeyCode.Space) && Application.isEditor) {
				MovieFinished();
			}

			//check if movie finished
			if (!mmt.IsPlaying) {
				MovieFinished();
			}
			else {
				//increment clip time
				MediaClipManager.Instance.clipTime = (float)mmt.PlayPosition;
			}


		}
	}




}














