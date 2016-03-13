using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using TMSupport;

public class BuddyDialog : UIDialog
{
	public static BuddyDialog instance;

	public enum eClose
	{
		Inactive = 0,
		Active,
		Highlighted
	}
	eClose					m_CloseState;

	RectTransform			m_CloseButtonRectTransform;
	Vector3					m_HighlightScale;

	void Awake()
	{
		instance = this;
		m_CloseButtonRectTransform = m_CloseButton.gameObject.GetComponent<RectTransform>();
		m_CloseState = eClose.Inactive;
		m_HighlightScale = Vector3.one;
	}

	//show in base class

	public void Hide() {
		this.gameObject.SetActive(false);
	}


	public void IconClicked()
	{
		GameManager.Instance.Buddy.IconClicked();
	}

	public void SkipClicked()
	{
		CloseClicked();
	}

	public void MuteClicked_()
	{
		AudioListener.volume = 1.0f - AudioListener.volume;
	}

	public void CloseClicked()
	{
		if(GameManager.Instance.Buddy.CloseClicked(m_CloseState) == true)
		{
			Hide();
			TMMessenger.Send("BuddyDialogClosed".GetHashCode());
		}

	}

	internal void SetCloseState(eClose CloseState)
	{
		if(CloseState == eClose.Inactive)
			DisableCloseButton(true);
		else
			DisableCloseButton(false);

		m_CloseState = CloseState;
	}

	public void Update()
	{
		if (m_CloseState == eClose.Highlighted)
		{
			m_HighlightScale.x = m_HighlightScale.y = 1.0f + Mathf.Sin(Time.time * 3) * 0.16f;
			m_CloseButtonRectTransform.localScale = m_HighlightScale;
		}


		if (Application.isEditor) {
			if (Input.GetButtonDown("s")) {
				SkipClicked();
			}
		}

	}
}
