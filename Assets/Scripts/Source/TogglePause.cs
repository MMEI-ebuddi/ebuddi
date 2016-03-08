using UnityEngine;
using System.Collections;

public class TogglePause : MonoBehaviour {

    private bool m_bPaused = false;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            if (Time.timeScale == 1.0f)
                Time.timeScale = 0.0f;
            else
                Time.timeScale = 1.0f;

            m_bPaused = (Time.timeScale == 1.0f) ? false : true;
            AudioListener.pause = m_bPaused;
        }	
	}
    
    void OnGUI()
    {
        if (!m_bPaused)
            return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 50;
        style.normal.textColor = Color.red;
            
        GUI.Label(new Rect(10, 10, 800, 800), "PAUSED - PRESS RIGHT ALT TO UNPAUSE", style);
    }
   
}
