using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMSupport;

public class Scene1 : Scene
{
	static float		sBasePercentage = 25.0f;
	
	public GameObject	m_PreDonningObjects;
	public GameObject	m_StationObjects;
	public UIClipboard	m_Clipboard;
	public UICriticalSteps uiCriticalSteps;
	public List<Texture2D> faceshieldCriticalSteps = new List<Texture2D>();
	public List<Texture2D> gowndCriticalSteps = new List<Texture2D>();
	public List<Texture2D> innerGlovesCriticalSteps = new List<Texture2D>();
	public UIEquipmentArea uiEquipmentArea;

	List<State>			m_IncorrectDonningStates = new List<State>();
	State				m_EquipmentStartState;
	State				m_GuidedStartState;
	State				m_DonningStartState;
	State				m_BadDonningState;
	State				m_RepeatGuidedState;
	State				m_DonningCompleteState;

	int					m_DonningBaseIndex;
	int					m_IncorrectDonningCount;

	int					m_NumCorrectDonnings;

    public BaseActionScriptableObject NameBadgeBASO;

	public bool skipGuided = false;



	protected override void Init()
	{
		base.Init();
		TMMessenger.Instance.AddToMessageQueue(new MessageCallback(this.MessageCallback));


		DirtyAvatarOff();
		FindAvatar();
		
		EnableCanvasUI(m_StationObjects, false);
		EnableCanvasUI(m_Avatar.gameObject, false);
		
		m_SceneIndex = 0;
		m_Guided = true;
		m_IncorrectDonningCount = 0;
		m_NumCorrectDonnings = 0;
		m_Clipboard.Init();
		m_Clipboard.gameObject.SetActive(true);


		CreateCameraCut(OurCamera.eCameraFocus.Buddy, 0.9f, 0.2f);
		CreateBuddyConversationUI("4.01", "BuddyDialogClosed");
		CreateBuddyConversationUI("4.07", "BuddyDialogClosed");


		bool basicPPESupported = PlayerPrefs.GetInt("basicPPESupported", 1) == 1;
		bool enchancedPPESupported = PlayerPrefs.GetInt("enhancedPPESupported", 1) == 1;


		//basic ppe introduction
		if (basicPPESupported) CreateBuddyConversationUI("4.09", "BuddyDialogClosed");

		//enchanced ppe introduction
		if (enchancedPPESupported) CreateBuddyConversationUI("4.11", "BuddyDialogClosed");

		//show choice only if both PPE types are supported
		if (basicPPESupported && enchancedPPESupported) {
			CreatePPEChoiceUI("PPEChoice");
			CreateProgressState(0.2f, ModuleType.donning, true);
		}
		else {
			//only one PPE supported
			if (basicPPESupported) SetPPE(ePPEType.Basic);
			else if (enchancedPPESupported) SetPPE(ePPEType.Enhanced);
		}

	}





	void FindAvatar()
	{
		m_Avatar = oldAvatar;
		m_Avatar.SetStreetClothes();
		if (m_Avatar.m_NameObject != null) m_Avatar.DisplayName(true);
	}


	IEnumerator Start()
	{
		GameManager.Instance.Buddy.WearScrubs();
	
		yield return new WaitForEndOfFrame();
		UIManager.Instance.OpenUI(UIManager.eUIReferenceName.DonningItemSelection);	
		TMMessenger.Send("OPENEXITTOTITLE".GetHashCode());

		ViewDonning.instance.progressBar.SetModuleProgress(ModuleType.donning, 0.001f, false);

	}
	
	public void SetupStatesPostEquipment()
	{
		Debug.Log("Setup states post equipment");

		if (!skipGuided) { CreateGuidedStage(); }
		CreateDonningStage();
		m_BadDonningState = CreateIncorrectDonningBuddyUI("BadDonning", "BuddyDialogClosed", m_DonningStartState);
		m_RepeatGuidedState = CreateIncorrectDonningBuddyUI("RepeatGuided", "BuddyDialogClosed", m_GuidedStartState);
		GotoNextState();
	}


	void CreatePreDonningStage(ePPEType ppeType )
	{
		Debug.Log("Creating pre donning stage for " + ppeType);

		if (ppeType == ePPEType.Basic) {
			CreateBuddyConversationUI("6.01", "BuddyDialogClosed");
		}
		else {
			Debug.LogError("Enchanced ppe not implemented yet");
		}


		CreateWait(0.3f);
		m_EquipmentStartState = m_States[m_States.Count - 1];
		CreateCameraCut(OurCamera.eCameraFocus.Rack, 1.7f);
		CreateBuddyConversationUI("6.01.1", "BuddyDialogClosed");
		CreateWait(0, "EquipmentChosen");
		EnableCanvasObjects(m_PreDonningObjects, false);
		EnableObject(m_Avatar.gameObject, true);
		CreateProgressState(0.4f, ModuleType.donning, true);

		CreateCameraCut(OurCamera.eCameraFocus.Buddy, 1.7f);
	}



    // REFACTOR TODO: Plan to replace this whole section with a new "stage" setup that will create sequences of actions from the unity UI using SOs
	void CreateGuidedStage()
	{
//		DirtyAvatarOff();
		CreateBuddyConversationUI("5.01", "BuddyDialogClosed");
		CreateBuddyConversationUI("5.02", "BuddyDialogClosed");
		CreateBuddyConversationUI("5.03", "BuddyDialogClosed");

		// Preparation
		EnableCanvasObjects(m_StationObjects, true);
		m_GuidedStartState = m_States[m_States.Count - 1];

        CreateGuidedDonningUISelection(EquipmentManager.Instance.GetActionList().GetActionOrder()[0]); // Refactor TODO remove this hard ref
	
		CreateCameraCut(OurCamera.eCameraFocus.Main, 0.9f, 0.7f);
		//animation remove jewlery
        CreateGuidedDonningAnimation(EquipmentManager.Instance.GetActionList().GetActionOrder()[0], "", EquipmentManager.eEquipment.Preparation);	 // Refactor TODO remove this hard ref	
		CreateCameraCut(OurCamera.eCameraFocus.Buddy, 0.9f);
		
        // Wash hands
        CreateGuidedDonningUISelection(EquipmentManager.Instance.GetActionList().GetActionOrder()[1]); // Refactor TODO remove this hard ref
		CreateMoviePlayback("HandWashing_offset", genericHandWashAudio);
        
        EquippedItems equippedItems = EquipmentManager.Instance.GetEquippedItems();

        foreach (BaseActionScriptableObject action in equippedItems.GetOrderedItems(EquipmentManager.Instance.GetActionList()))
        {
            if (action.Automatic)
            {
                m_Avatar.PutOn((EquipmentManager.eEquipment)action.Item);
                continue;
            }

            CreateGuidedDonningUISelection(action, ((EquipmentManager.eEquipment)action.Item).ToString(), (EquipmentManager.eEquipment)action.Item);

			if (action.Item == BaseActionScriptableObject.ITEM.FaceShield) {
				CreateRigSwapState(action, tempAvatar);
			}

            CreateCameraCut(OurCamera.eCameraFocus.Main, 0.9f, 0.7f);

            // Refactor todo: Need to change this when I add support for special item/event/camera cuts.
            if (action.Item == BaseActionScriptableObject.ITEM.OuterGloves)
            {
                CreateMoviePlayback("OuterGloveDonningProcedure");
                CreateCameraCut(OurCamera.eCameraFocus.MainAlternate, 0.9f);
            }

            CreateGuidedDonningAnimation(action, ((EquipmentManager.eEquipment)action.Item).ToString(), (EquipmentManager.eEquipment)action.Item);
			if (action.Item == BaseActionScriptableObject.ITEM.FaceShield) CreateCriticalSteps(action, faceshieldCriticalSteps);
			else if (action.Item == BaseActionScriptableObject.ITEM.Gown) {
				CreateCriticalSteps(action, gowndCriticalSteps);
			}
			else if (action.Item == BaseActionScriptableObject.ITEM.InnerGloves) {
				CreateCriticalSteps(action, innerGlovesCriticalSteps);
			}

            CreateCameraCut(OurCamera.eCameraFocus.Buddy, 0.9f);


			if (action.Item == BaseActionScriptableObject.ITEM.FaceShield) CreateRigSwapState(action, oldAvatar);
        }

        CreateNameBadgeAction();     	
		CreateProgressState(0.6f, ModuleType.donning, true);
        CreateBuddyConversationUI("6.09", "BuddyDialogClosed");
      
	}




    // REFACTOR TODO: Find a way to remove the need for this function at all
    void CreateNameBadgeAction()
    {
        // Name (if enhanced PPE)
        if (EquipmentManager.Instance.ProtectionRequired == ePPEType.Enhanced)
        {
            CreateCameraCut(OurCamera.eCameraFocus.Avatar, 1.3f);
            EnableCanvasObjects(m_Avatar.gameObject, true);            
            CreateGuidedDonningUISelection(NameBadgeBASO, "Name", EquipmentManager.eEquipment.Name);
            CreateWait(1.0f);
            CreateCameraCut(OurCamera.eCameraFocus.Main, 0.5f);
        }        
    }

	void CreateDonningStage()
	{
		// Preparation
		CreateBuddyUI("Buddy_UnguidedDonning_Intro", "BuddyDialogClosed");
		
		m_DonningStartState = CreateCallback(new Callback(BeginDonningStage));
		CreateCameraCut(OurCamera.eCameraFocus.Main, 0.7f);
		EnableCanvasObjects(m_StationObjects, true);
		CreateCallback(new Callback(BringInCheckList));
	    
        // Preparation
        CreateDonningUISelection(EquipmentManager.Instance.GetActionList().GetActionOrder()[0], "Preparation");

		// Wash hands
        CreateDonningUISelection(EquipmentManager.Instance.GetActionList().GetActionOrder()[1], "WashHands");               
        
        EquippedItems equippedItems = EquipmentManager.Instance.GetEquippedItems();
        foreach (BaseActionScriptableObject action in equippedItems.GetOrderedItems(EquipmentManager.Instance.GetActionList()))
        {
            if (action.Automatic)
            {
                continue;
            }


            CreateDonningUISelection(action, ((EquipmentManager.eEquipment)action.Item).ToString(), (EquipmentManager.eEquipment)action.Item);
			if (action.rightWrongSet.Count > 0) {
				CreateRightWrongQuizSelection(action, action.rightWrongSet[Random.Range(0, action.rightWrongSet.Count-1)], QuizType.training);
			}
        }        

		EnableCanvasObjects(m_Avatar.gameObject, true);
		CreateCallback(new Callback(GoodDonningProcedure));

		// End of donning simulation

		m_DonningCompleteState = CreateBuddyUI("GoodDonning_Done", "BuddyDialogClosed");
		CreateCallback(new Callback(EndOfScene));
	}

////////////////////////////////////////////////////////////////////////////////
// PPE choice
	private void CreatePPEChoiceUI(string MediaIdent)
	{
		State NewState = AllocateState(new Callback(PPEChoiceHandler));
		NewState.m_MediaIdent = MediaIdent;
	}


	void PPEChoiceHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			UIManager.Instance.BringUpDialog(UIManager.eUIReferenceName.BasicEnhancedPPE);
			GameManager.Instance.Buddy.TriggerConversationById("4.12", false);
		}
		if (Mode == eMode.UIMessage)
		{
			if (Message.Message == "BASIC_PPE_CHOICE".GetHashCode())
			{                
				SetPPE(ePPEType.Basic);
				GotoNextState();
			}
			else if (Message.Message == "ENHANCED_PPE_CHOICE".GetHashCode())
			{                
				SetPPE(ePPEType.Enhanced);
				GotoNextState();
			}
		}
	}


	void SetPPE (ePPEType ppeType) {
		if (ppeType == ePPEType.Basic) {
			EquipmentManager.Instance.ChoosenPPE(ePPEType.Basic);
			UIManager.Instance.CloseUI(UIManager.eUIReferenceName.BasicEnhancedPPE);
			CreatePreDonningStage(ePPEType.Basic);

		}
		else {
			//enhanced
			EquipmentManager.Instance.ChoosenPPE(ePPEType.Enhanced);
			UIManager.Instance.CloseUI(UIManager.eUIReferenceName.BasicEnhancedPPE);
			CreatePreDonningStage(ePPEType.Enhanced);
		}
	}


////////////////////////////////////////////////////////////////////////////////
// Guided UI selection

    
    void CreateGuidedDonningUISelection(BaseActionScriptableObject actionSO, string MediaIdent = "")
    {
        State NewState = AllocateState(new Callback(GuidedUISelect));

        NewState.m_ActionSO = actionSO;        
        NewState.m_UIWaitIdent = NewState.m_Action.ToString();
        if (!string.IsNullOrEmpty(MediaIdent)) NewState.m_MediaIdent = "GuidedDonning_" + MediaIdent;        
        NewState.m_Flag = false;
    }
    
    void CreateGuidedDonningUISelection(BaseActionScriptableObject actionSO, string OldMediaIdent, EquipmentManager.eEquipment AvatarEquipment)
    {
        State NewState = AllocateState(new Callback(GuidedUISelect));

        NewState.m_ActionSO = actionSO;        
        NewState.m_UIWaitIdent = NewState.m_Action.ToString();
		NewState.m_MediaIdent = "GuidedDonning_" + OldMediaIdent;
        NewState.m_Flag = false;
    }
    


	void GuidedUISelect(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			//add jewlery button at the right time
			if (m_CurrentState.m_ActionSO.Action == BaseActionScriptableObject.ACTION_TYPE.PREPARATION) EquipmentManager.Instance.AddPreparationStep();

            m_UIItemSelection.Highlight(m_CurrentState.m_ActionSO);

			//select
			if (EquipmentManager.Instance.ProtectionRequired == ePPEType.Basic) {
				if (m_CurrentState.m_ActionSO.basicPPEselectConversation != null) GameManager.Instance.Buddy.TriggerConversationById(m_CurrentState.m_ActionSO.basicPPEselectConversation.name);
				else {
					//old sound
					GameManager.Instance.Buddy.TriggerConversation(m_CurrentState.m_MediaIdent, false);
				}
			}
			else {
				//enchanced
				if (m_CurrentState.m_ActionSO.enchancedPPEselectConversation != null) GameManager.Instance.Buddy.TriggerConversationById(m_CurrentState.m_ActionSO.enchancedPPEselectConversation.name);
				else {
					//old sound
					GameManager.Instance.Buddy.TriggerConversation(m_CurrentState.m_MediaIdent, false);
				}
			}

//			if (m_CurrentState.m_ActionSO.ContainsConversation()) {
//
//				GameManager.Instance.Buddy.TriggerConversationById(m_CurrentState.m_ActionSO.conversationId, false);
//			}
//			else {
//				//old sound
//				GameManager.Instance.Buddy.TriggerConversation(m_CurrentState.m_MediaIdent, false);
//			}
			m_Guided = true;

		}
		else if (Mode == eMode.UIMessage)
		{            
			if (Message.Message == "BuddyDialogFinished".GetHashCode())
			{
				m_CurrentState.m_Flag = true;

                m_UIItemSelection.Highlight(m_CurrentState.m_ActionSO);             
			}
		}
		else if (Mode == eMode.ActionClicked && m_CurrentState.m_Flag)
		{
            if (m_CurrentState.m_ActionSO != null)
            {
                if (m_CurrentState.m_ActionSO.IsItem())
                {                 
                    m_Avatar.SetupForDonningAnimation((EquipmentManager.eEquipment)m_CurrentState.m_ActionSO.Item);
                }
				else {
					
					//badgeClicked
					if (m_CurrentState.m_ActionSO.Action == BaseActionScriptableObject.ACTION_TYPE.NAME_BADGE) {
						//puton player name
						m_Avatar.PutOn(EquipmentManager.eEquipment.Name);
					}
				}

                m_UIItemSelection.Highlight(m_CurrentState.m_ActionSO,false);
            }
           
			GotoNextState();
		}
	}

	//rig swapping

	void CreateRigSwapState(BaseActionScriptableObject actionSO, Avatar newAvatar) {

		RigSwapState NewState = AllocateState(new Callback(RigSwapHandler), new RigSwapState()) as RigSwapState;

		NewState.m_ActionSO = actionSO;
		NewState.newAvatar = newAvatar;
	}


	void RigSwapHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			Debug.Log("SWAP");
			Avatar newAvatar = ((RigSwapState)m_CurrentState).newAvatar;
			if (newAvatar == base.oldAvatar) {
				DirtyAvatarOff();

				//hide face shield in hand
				if (m_CurrentState.m_ActionSO.Item == BaseActionScriptableObject.ITEM.FaceShield) oldAvatar.m_AnimFaceShield.SetActive(false);
			}
			else DirtyAvatarOn();

			GotoNextState();
		}
	}


	//Critical Staps
	
	void CreateCriticalSteps(BaseActionScriptableObject actionSO, List<Texture2D> images) {

		CriticalStepsState NewState = AllocateState(new Callback(CreateCriticalStepsHandler), new CriticalStepsState()) as CriticalStepsState;
		
		NewState.m_ActionSO = actionSO;
		NewState.images = images;
	}
	
	
	void CreateCriticalStepsHandler(eMode Mode, TMMessageNode Message = null)
	{
		CriticalStepsState state = ((CriticalStepsState)m_CurrentState);

		if (Mode == eMode.Enter)
		{
			uiCriticalSteps.Init(state.images, true);
			uiCriticalSteps.ShowNext();
		}
		else if (Mode == eMode.ActionClicked) {
			Debug.Log ("Critical step action, step : " + uiCriticalSteps.currentIndex + " of " + (uiCriticalSteps.steps.Count-1).ToString());

			if (!uiCriticalSteps.allShown) {
				uiCriticalSteps.ShowNext();
			}
			else {
				//all showed
				uiCriticalSteps.CleanUp();
				GotoNextState();
			}
		}
	}




    void CreateGuidedDonningAnimation(BaseActionScriptableObject actionSO, string MediaIdent, EquipmentManager.eEquipment AvatarEquipment)
    {
        State NewState = AllocateState(new Callback(GuidedAnimHandler));

        NewState.m_ActionSO = actionSO;   
        NewState.m_UIWaitIdent = NewState.m_Equipment.ToString();
        NewState.m_MediaIdent = MediaIdent;
        NewState.m_AvatarEquipment = AvatarEquipment;
    }


	void GuidedAnimHandler(eMode Mode, TMMessageNode Message = null)
	{

		if (Mode == eMode.Enter)
		{
			m_CurrentState.m_Timer = m_Avatar.PlayDonningAnimation(m_CurrentState.m_AvatarEquipment);
			if (!string.IsNullOrEmpty(m_CurrentState.m_ActionSO.conversationId)) {
				GameManager.Instance.Buddy.TriggerConversationById(m_CurrentState.m_ActionSO.conversationId+".T", false);
			}
		}
		else if (Mode == eMode.Running)
		{
			//MAIN TIMER
			m_CurrentState.m_Timer -= Time.deltaTime;
			if (m_CurrentState.m_Timer <= 0)
			{
				GotoNextState();
			}
		}
	}

////////////////////////////////////////////////////////////////////////////////
// Begin donning
	void BeginDonningStage(eMode Mode, TMMessageNode Message = null)
	{
        AnalyticsRecorder.Instance.SetTimeOffset(m_TimeInScene);

		EquipmentManager.Instance.AddPreparationStep();


		m_Avatar.SetStreetClothes();
		m_Avatar.PutOn(EquipmentManager.eEquipment.Preparation);
		m_Avatar.TakeOff(EquipmentManager.eEquipment.Name);

		if (EquipmentManager.sChosenEquipment[(int)EquipmentManager.eEquipment.Boots] == true)
			m_Avatar.PutOn(EquipmentManager.eEquipment.Boots);
		if (EquipmentManager.sChosenEquipment[(int)EquipmentManager.eEquipment.ClosedToeShoes] == true)
			m_Avatar.PutOn(EquipmentManager.eEquipment.ClosedToeShoes);

		m_Guided = false;
		GotoNextState();
	}

////////////////////////////////////////////////////////////////////////////////
// Donning UI selection
	void GoodDonningProcedure(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			m_NumCorrectDonnings++;

			if (m_NumCorrectDonnings == 1)
			{
				GameManager.Instance.Buddy.TriggerConversation("GoodDonning_Repeat");
				ViewDonning.instance.progressBar.SetModuleProgress(ModuleType.donning, 0.8f, true);
			}
			if(m_NumCorrectDonnings >= 2)
			{
				m_CurrentState = m_DonningCompleteState;
				m_CurrentState.m_Handler(eMode.Enter);
				UserProfile.sCurrent.SetSceneCompletedTime(m_SceneIndex, m_TimeInScene, false);
				ViewDonning.instance.progressBar.SetModuleProgress(ModuleType.donning, 1f, true);
			}
		}
		else if (Mode == eMode.UIMessage)
		{
			if (Message.Message == "BuddyDialogClosed".GetHashCode())
			{
				m_CurrentState = m_DonningStartState;
				m_CurrentState.m_Handler(eMode.Enter);
			}
		}
	}




////////////////////////////////////////////////////////////////////////////////
// Donning UI selection
    // REFACTOR new function
    void CreateDonningUISelection(BaseActionScriptableObject actionSO, string MediaIdent, EquipmentManager.eEquipment AvatarEquipment)
    {
        State NewState = AllocateState(new Callback(DonningUISelect));

        NewState.m_ActionSO = actionSO;
        NewState.m_UIWaitIdent = NewState.m_Equipment.ToString();
        NewState.m_MediaIdent = "GoodDonning_" + MediaIdent;    
    }

     // REFACTOR new function
    State CreateDonningUISelection(BaseActionScriptableObject actionSO, string MediaIdent)
    {
        State NewState = AllocateState(new Callback(DonningUISelect));

        NewState.m_ActionSO = actionSO;
        NewState.m_UIWaitIdent = NewState.m_Action.ToString();
        NewState.m_MediaIdent = "GoodDonning_" + MediaIdent;      
        return (NewState);
    }

	void DonningUISelect(eMode Mode, TMMessageNode Message = null)
	{		
		if (Mode == eMode.Enter)
		{
			Debug.Log(m_CurrentState.m_ActionSO.ToString());
		}
		else if (Mode == eMode.ActionClicked)
		{            
			GameManager.Instance.Buddy.TriggerConversation(m_CurrentState.m_MediaIdent, false);

            if (m_CurrentState.m_ActionSO != null)
            {
                CorrectTestActionSelected(m_CurrentState.m_ActionSO);
                
                if (m_CurrentState.m_ActionSO.IsItem())
                {                                       
                    EquipmentManager.eEquipment thisItem = (EquipmentManager.eEquipment)m_CurrentState.m_ActionSO.Item;

                    if (thisItem == EquipmentManager.eEquipment.Preparation)

                        m_Avatar.TakeOff(thisItem);
                    else
                        m_Avatar.PutOn(thisItem);

                    if (thisItem == EquipmentManager.eEquipment.ReusableApron)
                        m_Avatar.PutOn(EquipmentManager.eEquipment.Name);
                }

				if (m_CurrentState.m_ActionSO.Action == BaseActionScriptableObject.ACTION_TYPE.PREPARATION) {

					//take off jewlery
					m_Avatar.TakeOffJewellery();
				}

			
                
                m_UIItemSelection.Highlight(m_CurrentState.m_ActionSO, false);
            }
           
			GotoNextState();
		}
		else if (Mode == eMode.WrongActionClicked)
		{
            if (m_CurrentState.m_ActionSO != null)
            {
                WrongTestActionSelected(m_CurrentState.m_ActionSO,Message.ActionSO);                
            }

			m_IncorrectDonningCount++;
			if (m_IncorrectDonningCount == 3)
			{
				m_CurrentState = m_RepeatGuidedState;
				m_IncorrectDonningCount = 0;
			}
			else
				m_CurrentState = m_BadDonningState;

			m_NumCorrectDonnings = 0;
			m_CurrentState.m_Handler(eMode.Enter);
			ViewDonning.instance.progressBar.SetModuleProgress(ModuleType.donning, 0.6f, true);
		}
		else if (Mode == eMode.DialogClosed)
		{
		}
	}

////////////////////////////////////////////////////////////////////////////////
// Incorrect donning
	State CreateIncorrectDonningBuddyUI(string MediaIdent, string UIWaitIdent, State NextState)
	{
		State		NewState = new State();

		NewState.m_Handler = new Callback(IncorrectBuddyUIHandler);
		NewState.m_UIWaitIdent = UIWaitIdent;
		NewState.m_MediaIdent = MediaIdent;
		NewState.m_Next = NextState;
		m_States.Add(NewState);
		m_IncorrectDonningStates.Add(NewState);
		return(NewState);
	}

	void IncorrectBuddyUIHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			GameManager.Instance.Buddy.TriggerConversation(m_CurrentState.m_MediaIdent);
			OurCamera.Instance.MoveTo(OurCamera.eCameraFocus.Buddy, 0.7f);
			UserProfile.sCurrent.IncorrectDonning();
//			CreateSetProgress(sBasePercentage + (sScenePercentage * 0.4f));
		}
		else if (Mode == eMode.DialogClosed || Mode == eMode.Exit)
		{
			m_Guided = true;						// Setting this to true will force the next donning stage to reset the clothes to the start
			GotoNextState();
		}
	}

////////////////////////////////////////////////////////////////////////////////
// Bring in the checklist
void BringInCheckList(eMode Mode, TMMessageNode Message = null)
{
	if (Mode == eMode.Enter)
	{
        m_Clipboard.Setup(EquipmentManager.Instance.GetEquippedItems().GetOrderedActions(EquipmentManager.Instance.GetActionList()));
	}
	else if (Mode == eMode.UIMessage)
	{
		if (Message.Message == "ClipboardDone".GetHashCode())
			GotoNextState();
	}
}

////////////////////////////////////////////////////////////////////////////////
// End of donning
	void EndOfScene(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
//			UIManager.Instance.OpenUI(UIManager.eUIReferenceName.EndOfDonning);
			UIFinishedPopup.instance.Show(ModuleType.donning);

			string		Text = "";
			string		AudioFileName = "";

			if (MediaNodeManager.GetTextAndAudio("Donning_Roundup_Body", ref Text, ref AudioFileName) == true)
				MediaClipManager.Instance.Play(AudioFileName);
		}
		else if (Mode == eMode.UIMessage)
		{
			if (Message.Message == "END_OF_SCENE".GetHashCode())
			{
				EndScene(eMode.Enter);
			}
		}
	}




	
	public override void Wait(eMode Mode, TMMessageNode Message = null)
	{
		base.Wait(Mode, Message);

		//if waiting for equipment to be choosen highlight
		if (m_CurrentState.m_UIWaitIdent == "EquipmentChosen") {
			if (m_CurrentState.timeActive >= 10f) {
				if (!uiEquipmentArea.equipmentHighlighted) uiEquipmentArea.HighlightCorrectEquipment();
			}
		}
	}


	protected void MessageCallback(TMMessageNode Message)
	{
		if(m_CurrentState == null)
			return;

		if (m_CurrentState.m_UIWaitIdent != null && Message.Message == m_CurrentState.m_UIWaitIdent.GetHashCode())
		{
			m_CurrentState.m_Handler(eMode.Exit, Message);
			return;
		}
		else if (Message.Message == "SceneActionClicked".GetHashCode())
		{
            if (Message.ActionSO != null)
            {
                if (m_CurrentState.m_ActionSO == Message.ActionSO)
                    m_CurrentState.m_Handler(eMode.ActionClicked, Message);
                else
                {                    
                    m_CurrentState.m_Handler(eMode.WrongActionClicked, Message);
                }
            }
            else
            {
                if (m_CurrentState.m_Action == (eActions)Message.Sender)
                {
                    m_CurrentState.m_Handler(eMode.ActionClicked);
                }
                else if (m_Guided == false)
                {
                    m_CurrentState.m_Handler(eMode.WrongActionClicked);
                }
            }
		}
		else if (Message.Message == "SceneEquipmentClicked".GetHashCode())
		{
			if (m_CurrentState.m_Equipment == (EquipmentManager.eEquipment)Message.Sender)
			{
				m_CurrentState.m_Handler(eMode.ActionClicked);
			}
			else if (m_Guided == false)
			{
				m_CurrentState.m_Handler(eMode.WrongActionClicked);
			}
		}
		else if (Message.Message == "FullScreenButtonTapped".GetHashCode()) {

			CreateCriticalStepsHandler(eMode.ActionClicked);
		}
		else if (Message.Message == "UserQuit".GetHashCode())
			m_CurrentState.m_Handler(eMode.UserQuit);
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
