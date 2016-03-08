using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMSupport;

public class UIPPEChoice : UIDialog
{
	public Button			m_BasicButton;
	public Button			m_EnhancedButton;


	public void BasicChosen()
	{
		TMMessenger.Send("BASIC_PPE_CHOICE".GetHashCode());
	}

	public void EnhancedChosen()
	{
		TMMessenger.Send("ENHANCED_PPE_CHOICE".GetHashCode());
	}
}
