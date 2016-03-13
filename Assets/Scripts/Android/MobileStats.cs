using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class MobileStats : MonoBehaviour {

    public Text TextStats;

    float deltaTime = 0.0f;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        string strFPS = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        TextStats.text = strFPS + "\n" + Screen.width.ToString() + "x" + Screen.height.ToString();        
	}
}
