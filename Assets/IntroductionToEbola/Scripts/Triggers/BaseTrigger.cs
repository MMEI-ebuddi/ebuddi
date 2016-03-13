using UnityEngine;
using System.Collections;

public class BaseTrigger : MonoBehaviour {

	public GameObject ObjectToEnable;
    public IntroductionFader Fader;

    private bool m_bAlreadryTriggered = false;

	public virtual bool Triggered()
	{
		return false;
	}	
	
	public virtual void Update()
	{
        if (!m_bAlreadryTriggered && Triggered())
		{
            m_bAlreadryTriggered = true;

            if (Fader != null)
            {
                Fader.Fade(ObjectToEnable, gameObject);
            }
            else
            {
                if (ObjectToEnable != null)
                    ObjectToEnable.SetActive(true);

                DisableScene();
            }
		}
		
	}
	
	public virtual void DisableScene()
	{
		gameObject.SetActive(false);
	}
}
