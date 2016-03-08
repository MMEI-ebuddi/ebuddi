using UnityEngine;
using System.Collections;

public class audiotest : MonoBehaviour {

    private GameObject m_Sound;

	// Use this for initialization
	void Start () {
        m_Sound = SimpleSoundClip.PlaySound((AudioClip)Resources.Load("Audio/Scene1"));
	}
	
	// Update is called once per frame
	void Update () {
	    if (m_Sound==null)
        {
            Debug.Log("speech finished!");
        }
	}
}
