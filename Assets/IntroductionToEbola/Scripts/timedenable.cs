using UnityEngine;
using System.Collections;

public class timedenable : MonoBehaviour {
    public GameObject[] Frames;

    public float fDelay = 1f;

    private float m_fTimer = 0f;

    private int m_iIndex = 0;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {

        if (m_iIndex == Frames.Length)
            return;

        m_fTimer += Time.deltaTime;

        if (m_fTimer >= fDelay)
        {
            if (m_iIndex > 0)
                Frames[m_iIndex - 1].SetActive(false);

            Frames[m_iIndex].SetActive(true);
            m_fTimer = 0f;

            m_iIndex++;                        
        }
	}
}
