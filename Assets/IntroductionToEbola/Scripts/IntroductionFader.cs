using UnityEngine;
using System.Collections;

public class IntroductionFader : MonoBehaviour {
    private GameObject m_ObjectToActivate, m_ObjectToDeactivate;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	  
	}

    public void FadedOut()
    {

    }

    public void FadedIn()
    {
        if (m_ObjectToActivate != null)
            m_ObjectToActivate.SetActive(true);

        if (m_ObjectToDeactivate != null)
            m_ObjectToDeactivate.SetActive(false);
    }

    public void Fade(GameObject activateObject, GameObject deactivateObject)
    {
        m_ObjectToActivate = activateObject;
        m_ObjectToDeactivate = deactivateObject;

        GetComponent<Animator>().SetTrigger("DoFade");
    }
}
