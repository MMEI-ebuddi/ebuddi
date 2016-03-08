using UnityEngine;
using System.Collections;

public class PlaySimpleSoundClip : MonoBehaviour {

	public AudioClip SoundResource;
	
	public bool Loop = false;

    public float Volume = 1.0f;

	// Use this for initialization
	void Start () {
        AudioSource audioSource = SimpleSoundClip.PlaySound((AudioClip)Resources.Load(SoundResource.name), Loop).GetComponent<AudioSource>();

        audioSource.volume = Volume;
	}
	
	
}
