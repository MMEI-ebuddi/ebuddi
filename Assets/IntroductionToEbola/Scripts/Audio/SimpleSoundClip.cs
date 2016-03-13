using UnityEngine;
using System.Collections;

/// <summary>
/// Made this very simple sound player class so that we can play the speech sounds during the introduction without depending on either app's bespoke sound systems
/// </summary>
public class SimpleSoundClip 
{
	static public GameObject PlaySound(AudioClip clip, bool bLoop)
	{
		if( clip == null )
		{
			Debug.LogError( "Null audio clip passed to PlaySound()" );
			return new GameObject("Audio_Null");
		}

		GameObject go = new GameObject("Audio_" + clip.name);
		go.transform.position = Vector3.zero;              
		
		AudioSource audio = go.AddComponent<AudioSource>();
		audio.clip = clip;    
		audio.Play();
		
		if (bLoop)
		{
			audio.loop = true;
		}
		else
		{
			UnityEngine.Object.Destroy(go, clip.length);
		}

		return go;
	}
	
    static public GameObject PlaySound(AudioClip clip)
    {
       return PlaySound(clip, false);
    }
}
