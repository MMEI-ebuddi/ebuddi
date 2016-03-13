using UnityEngine;
using System.Collections;

public class TriggerMovieClipFinished : BaseTrigger {
	
	public Renderer moviePlane;
    public AudioClip SoundResource;

    void Start()
    {
#if (!UNITY_ANDROID && !UNITY_IOS)
        SimpleSoundClip.PlaySound(MediaNodeManager.LoadAudioClipResource(SoundResource.name) );
		((MovieTexture)moviePlane.material.mainTexture).Play(); 
#endif
    }


	public override bool Triggered()
	{
 #if (!UNITY_ANDROID && !UNITY_IOS)
		return (!((MovieTexture)moviePlane.material.mainTexture).isPlaying);        		
#else
        return true;
#endif
	}


}
