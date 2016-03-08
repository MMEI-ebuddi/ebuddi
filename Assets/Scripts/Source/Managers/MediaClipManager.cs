using System.Collections;
using UnityEngine;
using TMSupport;

public class MediaClipManager : MonoBehaviour
{
	static MediaClipManager				sInstance;
	public static MediaClipManager		Instance { get { return sInstance; } }

	static Hashtable		sNodes = new Hashtable(1000);

	public AudioSource		m_AudioSource;
	public float clipTime = 0;
	
//	MovieTexture			m_LastMoviePlayed;

	string					m_LastClipFilename;
	AudioClip				m_LastAudioClipForVideo;

	public delegate void	AudioFinished();

	AudioFinished			m_AudioFinishedCallback;

	void Awake()
	{
		sInstance = this;
		m_LastClipFilename = null;
		m_AudioFinishedCallback = null;
	}

	internal static void EnsureLoaded(string AudioFilename)
	{
		string		FileName = MediaNodeManager.GetAudioDirectory() + AudioFilename;

		if(sNodes[AudioFilename.GetHashCode()] == null)
		{
			AudioClip	LoadedAudioClip = Resources.Load(FileName) as AudioClip;

			sNodes[AudioFilename.GetHashCode()] = LoadedAudioClip;
		}
	}

	internal static void LanguageChanged()
	{
		sNodes.Clear();
	}


	internal bool Play(string AudioFileName, AudioFinished AudioFinishedCallback = null)
	{
		Debug.Log(AudioFileName);
		AudioClip		Clip = sNodes[AudioFileName.GetHashCode()] as AudioClip;

		CancelInvoke();
		m_AudioSource.Stop();
		m_LastClipFilename = null;       

		if(Clip != null)
		{           
			m_AudioSource.spatialBlend = 0;
			m_AudioSource.clip = Clip;
			m_AudioFinishedCallback = AudioFinishedCallback;
			m_AudioSource.Play();
			clipTime = 0;
			Invoke("MediaClipFinished", Clip.length);
			m_LastClipFilename = AudioFileName;
			return(true);
		}
		return(false);
	}

	internal bool Play(AudioClip Clip, AudioFinished AudioFinishedCallback = null)
	{

		CancelInvoke();
		m_AudioSource.Stop();
		m_LastClipFilename = null;       
		
		if(Clip != null)
		{          
			Debug.Log(Clip.name);
			m_AudioSource.spatialBlend = 0;
			m_AudioSource.clip = Clip;
			m_AudioFinishedCallback = AudioFinishedCallback;
			m_AudioSource.Play();
//			clipTime = 0;
			Invoke("MediaClipFinished", Clip.length);
			m_LastClipFilename = Clip.name;
			return(true);
		}
		return(false);
	}





	Vector2		m_ImageDimension;

	internal void PlayMovie(string MovieFileName, AudioClip audioclip = null)
	{
		ViewScene.instance.movieDialog.PlayVideo(MovieFileName, audioclip);
	}



    // Check if it's an offset video. If so it's one video with one audio track and the videos layed next to 
    // eachother so we can ust modify the UV to switch the video whilst keeping same audio
    private void SetupOffsetVideo(string MovieFileName, MovieDialog dialog)
    {
        bool bOffset = MovieFileName.Contains("_offset");

        dialog.ButtonOffset.gameObject.SetActive(bOffset);

        if (bOffset)
        {
            Rect uv = dialog.m_RawImage.uvRect;

            if (bOffset)
            {
                uv.width = 0.5f;
                dialog.m_RawImage.uvRect = uv;

                Vector2 sizeDelta = dialog.m_RawImage.rectTransform.sizeDelta;
                sizeDelta.y *= 2.0f;
                dialog.m_RawImage.rectTransform.sizeDelta = sizeDelta;
            }        
        }       
    }



	void MediaClipFinished()
	{        

		if (m_AudioFinishedCallback != null)
		{
			// We have to do this as the callback could trigger another media clip and if it sets the m_AudioFinishedCallback, it's going to get reset
			AudioFinished	Callback = m_AudioFinishedCallback;

			m_AudioFinishedCallback = null;
			Callback();
		}
		else
			TMMessenger.Send("MediaClipFinished".GetHashCode(), 0, m_LastClipFilename);        
	}


	internal void StopAudio()
	{
		m_AudioSource.Stop();   
	}




	internal void Exit()
	{
		Debug.Log("Exit");
		StopAudio();

	}


}







