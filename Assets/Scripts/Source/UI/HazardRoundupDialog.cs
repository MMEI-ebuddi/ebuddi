using UnityEngine;
using UnityEngine.UI;
using TMSupport;

public class HazardRoundupDialog : UIDialog
{
	public Image[]			m_Images;

	[Header("Sprites")]
	public Sprite			m_CorrectSprite;
	public Sprite			m_IncorrectSprite;

	void Awake()
	{
		if(HazardManager.Instance != null)
		{
			for (int loop = 0; loop < HazardManager.Instance.m_NumberOfHazardsToPresent; loop++)
			{				
				bool		WasHazardFound = HazardManager.Instance.WasHazardFound(loop);

				m_Images[loop].sprite = WasHazardFound ? m_CorrectSprite : m_IncorrectSprite;
			}
		}
	}

	public void CloseClicked()
	{
		UIManager.Instance.CloseUI(UIManager.eUIReferenceName.HazardRoundup);
		TMMessenger.Send("HazardRoundupClosed".GetHashCode());
	}
}
