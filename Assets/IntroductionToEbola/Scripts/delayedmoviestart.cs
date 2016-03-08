using UnityEngine;
using System.Collections;

public class delayedmoviestart : MonoBehaviour {

    public float Timer = 6.0f;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	    if (Timer >= 0.0f)
        {
            Timer -= Time.deltaTime;

            if (Timer <= 0.0f)
            {
#if (!UNITY_ANDROID && !UNITY_IOS)
                ((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();        
#endif
            }
        }
	}
}
