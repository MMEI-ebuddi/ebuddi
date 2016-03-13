using UnityEngine;
using UnityEngine.UI;
using TMSupport;

public class UIRisks : MonoBehaviour
{
	static Color			sInactiveColour = new Color(0, 0, 0, 0.0f);
	static Color			sActiveColour = new Color(1, 1, 1, 1.0f);	

	public Image[]			m_Images;
    public Image[]          m_BackgroundImages;

	int						m_Found;

	void Awake()
	{
	}

	internal void Activate(int RiskCount)
	{
        Debug.Log("Risk count: " + RiskCount.ToString());
		m_Found = 0;
		gameObject.SetActive(true);
		for (int loop = 0; loop < m_Images.Length; loop++)
		{
            if (loop < RiskCount)
            {
                m_BackgroundImages[loop].gameObject.SetActive(true);
                m_Images[loop].gameObject.SetActive(true);
                m_BackgroundImages[loop].color = sActiveColour;
                m_Images[loop].color = sInactiveColour;
            }
            else
            {
                m_BackgroundImages[loop].gameObject.SetActive(false);                                
                m_Images[loop].gameObject.SetActive(false);
            }
		}
	}

	internal void AnotherRiskFound()
	{
		m_Images[m_Found].color = sActiveColour;
		m_Found++;
	}
}
