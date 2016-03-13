using UnityEngine;
using System.Collections;

public class movietest : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if (!UNITY_ANDROID && !UNITY_IOS)
        ((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();        
#endif
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
