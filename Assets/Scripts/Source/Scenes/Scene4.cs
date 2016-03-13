using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMSupport;

public class Scene4 : Scene
{
	protected override void Init()
	{
		TMMessenger.Instance.AddToMessageQueue(new MessageCallback(this.MessageCallback));

		// This is a hack so playing a scene from the editor will load the first user profile it finds
		if (UserProfile.sCurrent.Age == 0)
			UserProfile.Load();

		m_States = new List<State>();

// Intro
	// Wait for scene description close
		CreateSceneDescriptionUI("Scene1_Description", "SceneDescription1Closed");

		m_SceneIndex = 3;
		CreateMoviePlayback("Test");
	}

	public override void Exit()
	{
		TMMessenger.Instance.RemoveFromMessageQueue(this.MessageCallback);
	}

	IEnumerator Start()
	{
		GameManager.Instance.Buddy.WearScrubs();
		
//		m_Avatar = GameObject.FindObjectOfType<Avatar>();
//		m_Avatar.SetStreetClothes();

		yield return new WaitForEndOfFrame();
//		TMSupport.TMMessenger.Send("OPENITEMSELECTION".GetHashCode());
//		TMSupport.TMMessenger.Send("OPENSCENEDESCRIPTION".GetHashCode(), m_SceneIndex);
		UIManager.Instance.OpenUI(UIManager.eUIReferenceName.DonningItemSelection);
		TMSupport.TMMessenger.Send("OPENEXITTOTITLE".GetHashCode() );
	}

	protected void MessageCallback(TMMessageNode Message)
	{
		if (m_CurrentState == null)
			return;

		if (m_CurrentState.m_UIWaitIdent != null && Message.Message == m_CurrentState.m_UIWaitIdent.GetHashCode())
		{
			m_CurrentState.m_Handler(eMode.Exit, Message);
			return;
		}
		else if (Message.Message == "SceneActionClicked".GetHashCode())
		{
			if (m_CurrentState.m_Action == (eActions)Message.Sender)
			{
				m_CurrentState.m_Handler((eMode.ActionClicked));
			}
			else if (m_Guided == false)
			{
				m_CurrentState.m_Handler((eMode.WrongActionClicked));
			}
		}
		else if (Message.Message == "UserQuit".GetHashCode())
		{
			m_CurrentState.m_Handler(eMode.UserQuit);
			Exit();
		}
		else
			m_CurrentState.m_Handler(eMode.UIMessage, Message);
	}

	void Update()
	{
		if (m_UIItemSelection == null)
			m_UIItemSelection = GameObject.FindObjectOfType<UIItemSelection>();

		if(m_CurrentState == null && m_UIItemSelection != null)
			GotoState(m_States[0]);
		else if (m_CurrentState != null)
		{
			m_CurrentState.m_Handler(eMode.Running);
			m_TimeInScene += Time.deltaTime;
		}
	}

	void OnDestroy()
	{
		TMMessenger.Instance.RemoveFromMessageQueue(this);
	}
}
