using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMSupport;

public class RightWrongDialog : UIDialog
{	
	[Header("First")]
	public Button			m_FirstButton;
	public Image			m_FirstImage;
    public Image            ImageFirstAction;

	[Header("Second")]
	public Button			m_SecondButton;	
	public Image			m_SecondImage;
    public Image            ImageSecondAction;

	[Header("Sprites")]
	public Sprite			m_CorrectSprite;
	public Sprite			m_IncorrectSprite;

	bool					m_Swapped;

	void Awake()
	{
	}

	public void EnableButtons(bool Enable = true)
	{
		m_FirstButton.enabled = Enable;
		m_SecondButton.enabled = Enable;
	}

	public void SetCorrectAndIncorrectImages(Sprite TrueImage, Sprite FalseImage, bool DisplayCorrect = true)
	{
		m_Swapped = Random.value >= 0.5f;

        ImageFirstAction.sprite = m_Swapped ? FalseImage : TrueImage;
        ImageSecondAction.sprite = m_Swapped ? TrueImage : FalseImage;

		m_FirstImage.sprite = m_Swapped ? m_IncorrectSprite : m_CorrectSprite;
		m_SecondImage.sprite = m_Swapped ? m_CorrectSprite : m_IncorrectSprite;
		        
		if(DisplayCorrect)
		{
            m_FirstImage.enabled = true;
            m_SecondImage.enabled = true;		
		}
		else
		{
            m_FirstImage.enabled = false;
            m_SecondImage.enabled = false;		
		}
	}

	public void ButtonClicked(int Choice)
	{
		int		Index = Choice;

		if(m_Swapped)
			Choice = 1 - Choice;

		if(Choice == 0)
		{
			m_FirstImage.enabled = Index == 0;
			m_SecondImage.enabled = Index == 1;
			TMMessenger.Send("RightWrong_Correct".GetHashCode());
		}
		else
		{
			m_FirstImage.enabled = Index == 0;
			m_SecondImage.enabled = Index == 1;
			TMMessenger.Send("RightWrong_Incorrect".GetHashCode());
		}
	}
}
