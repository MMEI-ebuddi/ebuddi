using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMSupport;

public class Scene2 : Scene
{
	static float sBasePercentage = 50.0f;

	State		m_FoundRisksState;
	State		m_MissedRisksState;
	State		m_ReviewMissedRisksState;

	State		m_HazardsState;
	State		m_FoundHazardsState;
	State		m_MissedHazardsState;
	State		m_EndOfHazardsState;

	bool		m_CameraSway;
	bool		m_EnableCameraMovement;
	
	[Header("Debug only:")]
	public bool skipToHazards;

    public ActionListScriptableObject ActionList;

	protected override void Init()
	{
		base.Init();
		TMMessenger.Instance.AddToMessageQueue(new MessageCallback(this.MessageCallback));

		m_Avatar = oldAvatar;
		m_CameraSway = false;
		m_EnableCameraMovement = false;

		if (!skipToHazards) EnableObject(m_Avatar.gameObject, false);
		else {
			EnableObject(m_Avatar.gameObject, true);
		}

		if (tempAvatar != null) EnableObject(tempAvatar.gameObject, false);

		CreateMessage("EnableCameraSwap");
		CreateProgressState(0.001f, ModuleType.hazards, true);

		if (!skipToHazards) {
			//risks
			CreateCameraCut(OurCamera.eCameraFocus.Buddy, 0);

			CreateBuddyUI("10.01", "BuddyDialogClosed");
			CreateBuddyUI("10.02", "BuddyDialogClosed");
			CreateCameraCut(OurCamera.eCameraFocus.Main, 1.3f);
			CreateCustomState(new Callback(FindRiskHandler));
			m_ReviewMissedRisksState = CreateCustomState(new Callback(AssessRiskHandler));

			m_HazardsState = CreateCameraCut(OurCamera.eCameraFocus.Buddy, 1);
			CreateProgressState(0.33f, ModuleType.hazards, true);

			CreateCustomState(new Callback(HideRisksHandler));
			EnableObject(m_Avatar.gameObject, true);
			CreateBuddyUI("Buddy_HazardIntro", "BuddyDialogClosed");
			CreateMessage("DisableCameraSwap");
			CreateMessage("ScreenFadeDownUp", 2.2f);
		}

		CreateCameraCut(OurCamera.eCameraFocus.Avatar, 2.4f);

//		//all hazards called from this state
		CreateCustomState(new Callback(FindHazardHandler));
//		CreateProgressState(0.66f, ModuleType.hazards, true);

		m_EndOfHazardsState = CreateSetProgress(sBasePercentage + (sScenePercentage * 1.0f));

		CreateCameraCut(OurCamera.eCameraFocus.Buddy, 1f);
		CreateCustomState(new Callback(HazardsRoundup));
		// We're done
		m_States[m_States.Count - 1].m_Next = null;

		// The two buddy states for having found all the risks, or missed some
		m_FoundRisksState = CreateBuddyUI("Buddy_RisksFound", "BuddyDialogClosed");
		m_MissedRisksState = CreateBuddyUI("Buddy_RisksMissed", "BuddyDialogClosed");
		m_FoundRisksState.m_Next = m_HazardsState;
		m_MissedRisksState.m_Next = m_ReviewMissedRisksState;

		// The two buddy states for having found all the hazards, or missed some
		m_FoundHazardsState = CreateBuddyUI("Buddy_HazardsFound", "BuddyDialogClosed");
		m_MissedHazardsState = CreateBuddyUI("Buddy_HazardsMissed", "BuddyDialogClosed");
		m_FoundHazardsState.m_Next = m_EndOfHazardsState;
		m_MissedHazardsState.m_Next = m_EndOfHazardsState;
//		CreateProgressState(1f, ModuleType.hazards, true);

		m_SceneIndex = 1;
//		UIProgressBar.SetProgressPercentage(sBasePercentage);
//		CreateProgressState(1, ModuleType.hazards, true);
		
	}

	State CreateCustomState(Callback CustomCallback)
	{
		return(AllocateState(CustomCallback));
	}

	IEnumerator Start()
	{
		GameManager.Instance.Buddy.WearPPE();

		yield return new WaitForEndOfFrame();
		TMSupport.TMMessenger.Send("OPENEXITTOTITLE".GetHashCode() );
//		UIManager.Instance.OpenUI(UIManager.eUIReferenceName.ProgressBar);
//		ViewScene.instance.progressBar.SetModuleProgress(ModuleType.hazards, 0.001f, false);

		//always wear basic PPE
		m_Avatar.SetBaseClothing();
		m_Avatar.WearBsicPPE();

	}

	protected void MessageCallback(TMMessageNode Message)
	{
		if (m_CurrentState == null)
			return;

		if (Message.Message == "EnableCameraSwap".GetHashCode())
			m_CameraSway = true;
		else if (Message.Message == "DisableCameraSwap".GetHashCode())
			m_CameraSway = false;
		else if (Message.Message == "UserQuit".GetHashCode())
			m_CurrentState.m_Handler(eMode.UserQuit);
		else if (m_CurrentState.m_UIWaitIdent != null && Message.Message == m_CurrentState.m_UIWaitIdent.GetHashCode())
			m_CurrentState.m_Handler(eMode.Exit, Message);
		else
			m_CurrentState.m_Handler(eMode.UIMessage, Message);
	}

	void FindRiskHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			RiskManager.Instance.Activate();
			m_EnableCameraMovement = true;
		}
		else if (Mode == eMode.Running)
		{
		}
		else if (Mode == eMode.UIMessage)
		{
			if(Message.Message == "FinishedRisks".GetHashCode())
			{
				m_EnableCameraMovement = false;
				if(RiskManager.Instance.WereAllRisksFound())
					GotoState(m_FoundRisksState);
				else
					GotoState(m_MissedRisksState);
			}
			else if(Message.Message == "FoundRisk".GetHashCode())
			{
			
				Risk.eType riskType = (Risk.eType)Message.Sender;
				string conversationId = "";
				if (riskType == Risk.eType.ContaminatedSheets) conversationId = "10.05";
				else if (riskType == Risk.eType.VomitOnFloor) conversationId = "10.07";
				else if (riskType == Risk.eType.BucketObstacle) conversationId = "10.13";
				else if (riskType == Risk.eType.PaperTowel) conversationId = "10.10";
				else if (riskType == Risk.eType.BucketOfVomit) conversationId = "10.22";

				if (!string.IsNullOrEmpty(conversationId)) GameManager.Instance.Buddy.TriggerConversationById(conversationId, false);
				else {
					//old sound
					string FoundMediaName = "Risk_" + ((Risk.eType)Message.Sender).ToString() + "_Found";
					GameManager.Instance.Buddy.TriggerConversation(FoundMediaName, false);
				}
			}
		}
	}

	void HideRisksHandler(eMode Mode, TMMessageNode Message = null)
	{
		RiskManager.Instance.Activate(false);
		GotoNextState();
	}

	void AssessRiskHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			Risk.eType		RiskType = RiskManager.Instance.HighlightNextMissedRisk();
			string			FoundMediaName = "Risk_" + RiskType.ToString() + "_Missed";

			//missing new sounds, should be here

			GameManager.Instance.Buddy.TriggerConversation(FoundMediaName);
		}
		else if (Mode == eMode.Running)
		{
		}
		else if (Mode == eMode.UIMessage)
		{
			if(Message.Message == "BuddyDialogClosed".GetHashCode())
			{
				Risk.eType		RiskType = RiskManager.Instance.HighlightNextMissedRisk();

				if(RiskType == Risk.eType.MAX)
					GotoNextState();
				else
				{
					string		FoundMediaName = "Risk_" + RiskType.ToString() + "_Missed";

					GameManager.Instance.Buddy.TriggerConversation(FoundMediaName);
				}
			}
		}
	}



	public override void BuddyUIHandler(eMode Mode, TMMessageNode Message = null)
	{
		base.BuddyUIHandler(Mode, Message);

		if (Mode == eMode.Enter)
		{
			//if all hazards found pan back to the buddy
			if (m_CurrentState == m_FoundHazardsState) {



			}
			
		}
	}



	public void AllHazardsFinished() {
		GotoState(m_FoundHazardsState);
	
	}


	void FindHazardHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			HazardManager.Instance.Activate();
		}
		else if (Mode == eMode.Running)
		{
		}
		else if (Mode == eMode.UIMessage)
		{
			if (Message.Message == "FinishedHazards".GetHashCode())
			{
//				GotoNextState();
 				if (HazardManager.Instance.WereAllHazardsFound())
					AllHazardsFinished();
 				else
 					GotoState(m_MissedHazardsState);

				ViewHazards.instance.progressBar.SetModuleProgress(ModuleType.hazards, 0.66f, true);
			}
			else if (Message.Message == "FoundHazard".GetHashCode())
			{
				string conversationId = "";

				Hazard.eType hazardType = (Hazard.eType)Message.Sender;
				if (hazardType == Hazard.eType.PatientHeadZone) conversationId = "10.21";
				else if (hazardType == Hazard.eType.UncoveringWrists) conversationId = "10.24";
				else if (hazardType == Hazard.eType.BucketUpturned) conversationId = "10.22";
				
				if (!string.IsNullOrEmpty(conversationId))  GameManager.Instance.Buddy.TriggerConversationById(conversationId);
				else {
					//old sound
					string FoundMediaName = "Hazard_" + ((Hazard.eType)Message.Sender).ToString() + "_Found";
					GameManager.Instance.Buddy.TriggerConversation(FoundMediaName);
				}
			}
			else if (Message.Message == "MissedHazard".GetHashCode())
			{
				string FoundMediaName = "Hazard_" + ((Hazard.eType)Message.Sender).ToString() + "_Missed";

				GameManager.Instance.Buddy.TriggerConversation(FoundMediaName);
			}
		}
	}

	void HazardsRoundup(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
            UIManager.Instance.BringUpCanvas(UIManager.eUIReferenceName.HazardRoundup);

			UserProfile.sCurrent.SetSceneCompletedTime(m_SceneIndex, m_TimeInScene, false);
		}
		else if (Mode == eMode.Running)
		{
		}
		else if (Mode == eMode.UIMessage)
		{
			if (Message.Message == "HazardRoundupClosed".GetHashCode())
			{
				EndScene(eMode.Enter);
			}
		}
	}


	//TODO find what is this?
	float			m_CameraInterp;



	void Update()
	{
		if (m_CurrentState == null)
			GotoState(m_States[0]);
		else if (m_CurrentState != null)
		{
			m_CurrentState.m_Handler(eMode.Running);
			m_TimeInScene += Time.deltaTime;
		}
		
		Vector3		CameraOffset = Vector3.zero;

		if(m_CameraSway)
		{
			CameraOffset.x = Mathf.Sin(Time.time) * 0.01f;
			CameraOffset.y = Mathf.Sin((Time.time + 1) * 1.01f) * 0.01f;
			CameraOffset.z = Mathf.Sin((Time.time + 2) * 0.99f) * 0.01f;
		}
		OurCamera.Instance.SetCameraOffset(CameraOffset);

		if(m_EnableCameraMovement)
		{
			if(Input.GetKey(KeyCode.A))
				m_CameraInterp += (Time.deltaTime * 0.3f);
			else if (Input.GetKey(KeyCode.D))
				m_CameraInterp -= (Time.deltaTime * 0.3f);

			m_CameraInterp = Mathf.Clamp(m_CameraInterp, 0, 1);

			OurCamera.Instance.SetCameraInterp(OurCamera.eCameraFocus.Main, OurCamera.eCameraFocus.MainAlternate, m_CameraInterp);
		}
	}

	void OnDestroy()
	{
		TMMessenger.Instance.RemoveFromMessageQueue(this);
	}
}
