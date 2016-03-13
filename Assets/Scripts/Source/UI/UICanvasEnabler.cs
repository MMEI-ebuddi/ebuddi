using UnityEngine;
using System.Collections;

public class UICanvasEnabler : MonoBehaviour {

	public GameObject canvas;

	// Use this for initialization
	void Start () {

		Invoke("EnableCanvas", 1f);
	}
	
	// Update is called once per frame
	void EnableCanvas () {
		canvas.SetActive(true);
	}
}
