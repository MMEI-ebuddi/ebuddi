using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMSupport;

public class Scene3 : Scene
{
	static float		sBasePercentage = 75.0f;

	public UIClipboard	m_Clipboard;
	public UICriticalSteps m_CriticalSteps;

	List<State>		m_IncorrectDoffingStates = new List<State>();
	int				m_IncorrectDoffingCount;
	State			m_DoffingStartState;
	State			m_GuidedStartState;
	State			m_EnhancedGuidedStartState;
	State			m_BadDoffingState;
	State			m_RepeatGuidedState;
	State			m_DoffingCompleteState;
	int				m_UnguidedIncorrectDoffingCount;

	int				m_DonningBaseIndex;	

	int				m_NumCorrectDoffings;

    public Transform BootWashStartPosition;

	public ActionListScriptableObject basicPPEActionList;
	public ActionListScriptableObject enchancedPPEActionList;
    private ActionListScriptableObject ActionList;
	public List<Texture2D> faceDoffCriticalSteps = new List<Texture2D>();
	public List<Texture2D> gownDoffCriticalSteps = new List<Texture2D>();
	public Quiz faceshieldQuiz;
	public Quiz handWashQuiz;
	public UIQuiz quizUI;
	public Animation virusDropAnimation;



	[Header("Debugging only")]
	public bool skipToVirus = false;
	public bool skipFaceShield = false;
	public bool skipToBoots = false;
	public bool skipGuided = false;


	protected override void Init()
	{
		base.Init();
		TMMessenger.Instance.AddToMessageQueue(new MessageCallback(this.MessageCallback));		

		m_SceneIndex = 2;
		m_Guided = true;
		m_NumCorrectDoffings = 0;
		m_UnguidedIncorrectDoffingCount = 0;

		m_Clipboard.Init();
		m_Clipboard.gameObject.SetActive(true);

	}

	IEnumerator Start()
	{
		//TEMP
		tempAvatar.gameObject.SetActive(false);
		virusDropAnimation.gameObject.SetActive(false);

		GameManager.Instance.Buddy.WearScrubs();

		yield return new WaitForEndOfFrame();
		UIManager.Instance.OpenUI(UIManager.eUIReferenceName.DoffingItemSelection);
		yield return new WaitForEndOfFrame();


		ViewScene.instance.progressBar.SetModuleProgress(ModuleType.doffing, 0.001f, animated:false);

		m_Avatar = oldAvatar;


		m_Avatar.gameObject.SetActive(true);
		m_Avatar.EbolaDisplayEnable();
    

		if (skipToVirus) {
			ActionList = basicPPEActionList;
			EquipmentManager.Instance.SetActionList(ActionList);
			EquipmentManager.Instance.CreateChosenStatesFromSave(ActionList);
			EquipmentManager.Instance.DressAvatar(m_Avatar);
			CreateDoffingStage();
		}

		else {
			//add indent here

			bool basicPPESupported = PlayerPrefs.GetInt("basicPPESupported", 1) == 1;
			bool enchancedPPESupported = PlayerPrefs.GetInt("enhancedPPESupported", 1) == 1;


			//show choice only if both PPE types are supported
			if (basicPPESupported && enchancedPPESupported) {
				CreatePPEChoiceUI("");
				//begin with first state

			}
			else {
				//only one PPE supported
				if (basicPPESupported) SetPPE(ePPEType.Basic);
				else if (enchancedPPESupported) SetPPE(ePPEType.Enhanced);
			}

		}

		GotoState(m_States[0]);
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
		}
		if (Mode == eMode.UIMessage)
		{
			if (Message.Message == "BASIC_PPE_CHOICE".GetHashCode() || Message.Message == "ENHANCED_PPE_CHOICE".GetHashCode())
			{                

				UIManager.Instance.CloseUI(UIManager.eUIReferenceName.BasicEnhancedPPE);

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
	}



	void SetPPE (ePPEType type) {

		if (type == ePPEType.Basic) {
			ActionList = basicPPEActionList;
			EquipmentManager.Instance.ChoosenDoffinPPE(ePPEType.Basic);
		}
		else {
			//enhanced
			ActionList = enchancedPPEActionList;
			EquipmentManager.Instance.ChoosenDoffinPPE(ePPEType.Enhanced);
		}

		EquipmentManager.Instance.SetActionList(ActionList);
		EquipmentManager.Instance.CreateChosenStatesFromSave(ActionList);
		EquipmentManager.Instance.DressAvatar(m_Avatar);
		
		
		if (!skipGuided) CreateGuidedStage();
		CreateDoffingStage();
		
		m_BadDoffingState = CreateIncorrectDoffingBuddyUI("BadDoffing", "BuddyDialogClosed", m_DoffingStartState);
		m_RepeatGuidedState = CreateIncorrectDoffingBuddyUI("RepeatGuided", "BuddyDialogClosed", m_GuidedStartState);

	}





	#region Guided 

    void CreateGuidedStage()
    {
        m_GuidedStartState = CreateCameraCut(OurCamera.eCameraFocus.Buddy, 0.9f);

		if (EquipmentManager.Instance.ProtectionRequired == ePPEType.Basic) {
			CreateBuddyConversationUI("8.02", "BuddyDialogClosed");
			CreateBuddyConversationUI("8.03", "BuddyDialogClosed");
		}
		else if (EquipmentManager.Instance.ProtectionRequired == ePPEType.Enhanced) {
			CreateBuddyConversationUI("11.01", "BuddyDialogClosed");
			CreateBuddyConversationUI("11.02", "BuddyDialogClosed");
		}
                
		EquippedItems equippedItems = EquipmentManager.Instance.GetEquippedItems();

        WashHands(true);

		bool bootsInspected = false;

		foreach (BaseActionScriptableObject action in equippedItems.GetOrderedItems(EquipmentManager.Instance.GetActionList()))
        {
            if (action.Automatic)
                continue;
            
            EquipmentManager.eEquipment Equipment = (EquipmentManager.eEquipment)action.Item;


			if (skipFaceShield) {
				if (Equipment != EquipmentManager.eEquipment.FaceShield) {
					continue;
				}
			}

			if (skipToBoots) {
				if (Equipment == EquipmentManager.eEquipment.OuterGloves ||
				    Equipment == EquipmentManager.eEquipment.ReusableApron || 
				    Equipment == EquipmentManager.eEquipment.Gown ||
				    Equipment == EquipmentManager.eEquipment.Hood ||
				    Equipment == EquipmentManager.eEquipment.FaceMask ||
				    Equipment == EquipmentManager.eEquipment.FaceShield ) {
					continue;
				}
			}

			//override for boots washing
			if (Equipment == EquipmentManager.eEquipment.Boots && bootsInspected) {
				Equipment = EquipmentManager.eEquipment.BootsRemoval;
			}


            HandleGuidedAction(action, Equipment);

            if (action.WashHandsAter)
                WashHands(false);


			if (Equipment == EquipmentManager.eEquipment.Boots) bootsInspected = true;
        }
              
        // End of guided simulation
//        CreateSetProgress(sBasePercentage + (sScenePercentage * 0.33f));
		CreateProgressState(0.33f, ModuleType.doffing, true);
        CreateBuddyConversationUI("8.14", "BuddyDialogClosed");
    }



    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // REFACTOR: I split the massive bunch of if statements that was above into separate functions to make it a bit easier to follow
    // and to assess the situation so I can rework it soon. This is still ugly and is going to be reworked
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void HandleGuidedAction(BaseActionScriptableObject actionSO, EquipmentManager.eEquipment equipment) 
    {
        string EquipmentName = equipment.ToString();

        if (equipment == EquipmentManager.eEquipment.InnerGloves || equipment == EquipmentManager.eEquipment.OuterGloves)
        {
            Guided_Doff_Gloves(actionSO, EquipmentName, equipment);
        }
        
        else
        {
            if (equipment == EquipmentManager.eEquipment.FaceShield)
            {
				CreateGuidedDoffingUISelection(actionSO, equipment.ToString(), equipment);
                Guided_Doff_FaceShield(actionSO, EquipmentName, equipment);
            }
            else if (equipment == EquipmentManager.eEquipment.Hood)
            {
				CreateGuidedDoffingUISelection(actionSO, equipment.ToString(), equipment);
				CreateGuidedDoffingAnimation(actionSO, "", equipment);
            }
            else if (equipment == EquipmentManager.eEquipment.Gown)
            {
				CreateGuidedDoffingUISelection(actionSO, equipment.ToString(), equipment);
                Guided_Doff_Gown(actionSO, EquipmentName, equipment);
            }
            else if (equipment == EquipmentManager.eEquipment.Boots)
            {

				CreateGuidedDoffingUISelection(actionSO, equipment.ToString(), equipment);
                Guided_Doff_BootsInspect(actionSO, EquipmentName, equipment);
            }
			else if (equipment == EquipmentManager.eEquipment.BootsRemoval) {

				CreateGuidedDoffingUISelection(actionSO, "BootsRemoval", EquipmentManager.eEquipment.BootsRemoval);
				Guided_Doff_BootsWash(actionSO, "BootsRemoval", EquipmentManager.eEquipment.BootsRemoval);
			}

            else if (equipment == EquipmentManager.eEquipment.ReusableApron)
            {
				CreateGuidedDoffingUISelection(actionSO, equipment.ToString(), equipment);
                Guided_Doff_ReusableApron(actionSO, EquipmentName, equipment);
            }
            else
            {
				CreateGuidedDoffingUISelection(actionSO, equipment.ToString(), equipment);
                CreateGuidedDoffingAnimation(actionSO, EquipmentName, equipment);
                CreateCameraCut(OurCamera.eCameraFocus.Main, 0.9f, 0.7f);
                CreateWait(2.3f);
            }

            CreateDisposal(equipment);
        }
    }
        

    private void WashHands(bool playVideo=false)
    {
		if (EquipmentManager.Instance.ProtectionRequired == ePPEType.Basic) {
	        CreateGuidedDoffingUISelection(EquipmentManager.Instance.GetActionList().GetActionOrder()[0], "8.05");  
		}
		else if (EquipmentManager.Instance.ProtectionRequired == ePPEType.Enhanced)  {
			CreateGuidedDoffingUISelection(EquipmentManager.Instance.GetActionList().GetActionOrder()[0], "11.04");  
		}

        if (playVideo) {
			if (EquipmentManager.Instance.BasicPPE()) {
				CreateMoviePlayback("HandwashingInnerGloves_offset", genericHandWashAudio);
			}
			else {
				//play video with blue gloves on
				CreateMoviePlayback("HandwashingBlueGloves_offset", genericHandWashAudio);	
			}
		}

        CreateMessage("HideEbola");
        CreateWait(1.0f);
        CreateCameraCut(OurCamera.eCameraFocus.Buddy, 0.9f);      
    }




    private void Guided_Doff_Gloves(BaseActionScriptableObject actionSO, string strItemName, EquipmentManager.eEquipment equipment)
    {     
        PositionObject(GameObject.FindGameObjectWithTag("BioHazardBucket"), GameObject.Find("BioHazardBucketStandardPosition").transform.position);
        CreateGuidedDoffingUISelection(actionSO, strItemName, equipment);
        CreateCameraCut(OurCamera.eCameraFocus.Main, 0.9f, 0.7f);

        if (equipment == EquipmentManager.eEquipment.OuterGloves)
            CreateMoviePlayback("OuterGlovesDoffing");
        else
            CreateMoviePlayback("InnerGlovesDoffing");

        CreateMessage("ShowEbola");
        CreateGuidedDoffingAnimation(actionSO, strItemName, equipment); // refactor: might be an issue here because it used to share GLOVES as common for 2 tasks        
    }

	private void Guided_Doff_BootsInspect(BaseActionScriptableObject actionSO, string strItemName, EquipmentManager.eEquipment equipment)
	{
		CreateGuidedDoffingAnimation(actionSO, strItemName, equipment, 0.1f);
		CreateCameraCut(OurCamera.eCameraFocus.Main, 0.9f, 0.7f);
		CreateMessage("ShowEbola");
		CreateWait(2.3f);
	}


    private void Guided_Doff_BootsWash(BaseActionScriptableObject actionSO, string strItemName, EquipmentManager.eEquipment equipment)
    {       
        PositionObject(m_Avatar.gameObject, BootWashStartPosition.position);  
		CreateCameraCut(OurCamera.eCameraFocus.Main, 0.9f, 0.7f);
		CreateGuidedDoffingAnimation(actionSO, strItemName, equipment);
        CreateWait(7f);
    }
	

    private void Guided_Doff_FaceShield(BaseActionScriptableObject actionSO, string strItemName, EquipmentManager.eEquipment equipment)
    {
        CreateCameraCut(OurCamera.eCameraFocus.Main, 0.9f, 0.7f);

		CreateGuidedDoffingUISelection(actionSO, strItemName, EquipmentManager.eEquipment.FaceShield, 1);
		CreateGuidedDoffingUISelection(actionSO, "", EquipmentManager.eEquipment.FaceShield, 2);
		CreateGuidedDoffingUISelection(actionSO, "", EquipmentManager.eEquipment.FaceShield, 3);
		CreateGuidedDoffingAnimation(actionSO, "", equipment, 4f, 4);
		CreateCameraCut(OurCamera.eCameraFocus.Buddy, 0.9f, 0.7f);
    }
	

    private void Guided_Doff_Gown(BaseActionScriptableObject actionSO, string strItemName, EquipmentManager.eEquipment equipment)
    {
        CreateGuidedDoffingAnimation(actionSO, strItemName, equipment);
        CreateCameraCut(OurCamera.eCameraFocus.Main, 0.9f, 0.7f);
        CreateMoviePlayback("GownDoffingProcedure", null, actionSO);
        CreateCallback(new Callback(SwapToDisposalObject));
        CreateMessage("ShowEbola");
    }


    private void Guided_Doff_ReusableApron(BaseActionScriptableObject actionSO, string strItemName, EquipmentManager.eEquipment equipment)
    {
        // Hide the bucket if it's our second go around
        GameObject bucket = GameObject.Find("Boot_Washing_Bucket");

        if (bucket != null)
            bucket.SetActive(false);

        PositionObject(GameObject.FindGameObjectWithTag("BioHazardBucket"), GameObject.Find("BioHazardBucketApronPosition").transform.position);
        CreateGuidedDoffingAnimation(actionSO, strItemName, equipment, 0.1f);
        CreateCameraCut(OurCamera.eCameraFocus.Main, 0.9f, 0.7f);
        CreateWait(18f);
    }
         
	#endregion

	#region Doffing

    // REFACTOR: reworking this old function
    void CreateDoffingStage()
	{
		m_DoffingStartState = CreateCallback(new Callback(BeginDoffingStage));
		CreateCameraCut(OurCamera.eCameraFocus.Main, 0.7f);


		// Wash hands		

		//debug
		if (!skipToVirus) {
			CreateCallback(new Callback(BringInCheckList));
			Handle_WashHands(false);
			CreateQuizSelection(EquipmentManager.Instance.GetActionList().GetActionOrder()[0], handWashQuiz, Random.Range(0, handWashQuiz.images.Count-2));
		}

		CreateMessage("HideEbola");

		//using this flag to help handling booths inspecting and washing
		bool innerGlovesAdded = false;


        EquippedItems equippedItems = EquipmentManager.Instance.GetEquippedItems();

        foreach (BaseActionScriptableObject action in equippedItems.GetOrderedItems(EquipmentManager.Instance.GetActionList()))
        {
            if (action.Automatic)
                continue;

            EquipmentManager.eEquipment Equipment = (EquipmentManager.eEquipment)action.Item;

			if (skipToVirus) {
				if (Equipment == EquipmentManager.eEquipment.FaceShield) 
				{
					HandleAction(action, Equipment);
					if (action.WashHandsAter) Handle_WashHands(true);
				}
			}
			else {

				//override second boots action as boots removal
				if (Equipment == EquipmentManager.eEquipment.Boots) {
					if (innerGlovesAdded) Equipment = EquipmentManager.eEquipment.BootsRemoval;
				}


				HandleAction(action, Equipment);
				if (action.WashHandsAter) Handle_WashHands(true);

				if (Equipment == EquipmentManager.eEquipment.InnerGloves) innerGlovesAdded = true;
			}


        }
	
		CreateCallback(new Callback(GoodDoffingProcedure));

		// End of guided simulation
		m_DoffingCompleteState = CreateBuddyUI("GoodDoffing_Done", "BuddyDialogClosed");
		CreateCallback(new Callback(EndOfScene));
	}



    // Refactor: Moved all the code out of a massive loop to this function to make things easier to follow. To be replaced with the Stage system in the future.
    private void HandleAction(BaseActionScriptableObject actionSO, EquipmentManager.eEquipment equipment)
    {
        string EquipmentName = equipment.ToString();

		CreateDoffingUISelection(actionSO, EquipmentName, equipment);

		if (actionSO.rightWrongSet.Count > 0) {
			CreateRightWrongQuizSelection(actionSO, actionSO.rightWrongSet[Random.Range(0, actionSO.rightWrongSet.Count-1)], QuizType.training);
		}

        if (equipment == EquipmentManager.eEquipment.FaceShield)
        {

			//first round of doffing
			CreateQuizSelection(actionSO, faceshieldQuiz, 0);
			CreateDoffingAnimation(actionSO, "", EquipmentManager.eEquipment.FaceShield, 6f);

//			//virus drop
			CreateDoffingAnimation(actionSO, "virusDropLeanForward", EquipmentManager.eEquipment.FaceShield, 6f);
			CreateVirusDropAnimation(actionSO);
			CreateDoffingAnimation(actionSO, "virusDropContinue", EquipmentManager.eEquipment.FaceShield, 7f);


        }                  
    }


    private void Handle_WashHands(bool handWashAgain)
    {
        CreateMessage("ShowEbola");

		string madiaIndent = "";
		if (handWashAgain) madiaIndent = "WashHandsAgain";
		else madiaIndent = "WashHands";

		CreateDoffingUISelection(EquipmentManager.Instance.GetActionList().GetActionOrder()[0], madiaIndent); // Refactor TODO remove this hard ref
        CreateMessage("HideEbola");
        CreateWait(1.0f);
    }




	protected override void RightWrongQuizUISelect(eMode Mode, TMMessageNode Message = null) {

		if (Mode == eMode.Enter) {
			Debug.Log("right wrong select " + m_CurrentState.m_ActionSO.Item);
			if (m_CurrentState.m_ActionSO.Item == BaseActionScriptableObject.ITEM.Hood) {
				DirtyAvatarOff();
				m_Avatar.TakeOff(EquipmentManager.eEquipment.Hood);
			}
		}
		base.RightWrongQuizUISelect(Mode, Message);
	}






	#endregion

	#region Guided UISelection

////////////////////////////////////////////////////////////////////////////////
// Guided UI selection
////////////////////////////////////////////////////////////////////////////////

    // REFACTOR: NEW function
	void CreateGuidedDoffingUISelection(BaseActionScriptableObject actionSO, string MediaIdent, int criticalStep = -1)
    {
        State NewState = AllocateState(new Callback(GuidedUISelect));

        NewState.m_ActionSO = actionSO;
        NewState.m_UIWaitIdent = NewState.m_Action.ToString();
        NewState.m_MediaIdent = "GuidedDoffing_" + MediaIdent;
        NewState.m_AvatarEquipment = (EquipmentManager.eEquipment)actionSO.Item;
        NewState.m_Flag = false;

		if (criticalStep != -1) 
		{
			Debug.Log("Create doff animation with critical stage: " + criticalStep.ToString());
			NewState.criticalStep = criticalStep;
		}

    }

    // REFACTOR: NEW function
	void CreateGuidedDoffingUISelection(BaseActionScriptableObject actionSO, string MediaIdent, EquipmentManager.eEquipment AvatarEquipment, int criticalStep = -1)
    {

        State NewState = AllocateState(new Callback(GuidedUISelect));

        NewState.m_ActionSO = actionSO;
        NewState.m_UIWaitIdent = NewState.m_Action.ToString();
        if (!string.IsNullOrEmpty(MediaIdent)) NewState.m_MediaIdent = "GuidedDoffing_" + MediaIdent;
        NewState.m_AvatarEquipment = (EquipmentManager.eEquipment)actionSO.Item;
        NewState.m_Flag = false;

		if (criticalStep != -1) {
			NewState.criticalStep = criticalStep;
		}
    }



    // REFACTOR: WIP
    void GuidedUISelect(eMode Mode, TMMessageNode Message = null)
    {
        if (Mode == eMode.Enter)
        {
			//faceshield
			if (m_CurrentState.m_ActionSO.Item == BaseActionScriptableObject.ITEM.FaceShield) {
				m_UIItemSelection.Highlight(m_CurrentState.m_ActionSO, false);

				if (m_CurrentState.criticalStep == 0) {

					if (EquipmentManager.Instance.ProtectionRequired == ePPEType.Basic) {
						GameManager.Instance.Buddy.TriggerConversationById(m_CurrentState.m_ActionSO.basicPPEselectConversation.name);
					}
					else {
						//enchanced
						GameManager.Instance.Buddy.TriggerConversationById(m_CurrentState.m_ActionSO.enchancedPPEselectConversation.name);
					}
		            m_Guided = true;
				}
				else if (m_CurrentState.criticalStep == 1) {
					m_CriticalSteps.Init(faceDoffCriticalSteps, true);

					m_CurrentState.m_Timer = m_Avatar.PlayDoffingAnimationWithTime(1.5f, "shieldoff");
					m_CriticalSteps.ShowNext("8.12.2");
				}
				else if (m_CurrentState.criticalStep == 2) {
					m_CurrentState.m_Timer = m_Avatar.PlayDoffingAnimationWithTime(1.5f, "shieldoff2");
					m_CriticalSteps.ShowNext("8.12.3");
				}
				else if (m_CurrentState.criticalStep == 3) {
					m_CurrentState.m_Timer = m_Avatar.PlayDoffingAnimationWithTime(1.5f, "shieldoff3");
					m_CriticalSteps.ShowNext("8.12.4");
				}
			}
			else {

				if (m_CurrentState.criticalStep == 0) {

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
					m_Guided = true;
				}
			}
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
					m_UIItemSelection.Highlight(m_CurrentState.m_ActionSO, false);
					///TEMP
					if (m_CurrentState.m_ActionSO.Item == BaseActionScriptableObject.ITEM.FaceShield) {
						DirtyAvatarOn();
					}

					//Hiding objects here!
					//skipping boots as we have no legs!
					if (m_CurrentState.m_ActionSO.Item != BaseActionScriptableObject.ITEM.Boots) {
	                    m_Avatar.SetupForDoffingAnimation((EquipmentManager.eEquipment)m_CurrentState.m_ActionSO.Item);
					}
                }
				else {
					//handWash
					if (m_CurrentState.m_ActionSO.Action == BaseActionScriptableObject.ACTION_TYPE.WASH_HANDS) {
						//unhighlight handwashing button
						m_UIItemSelection.Highlight(m_CurrentState.m_ActionSO, false);
					}
				}
            }
            GotoNextState();
//			Debug.Log("Next state");
        }
    }

	#endregion

	#region doffing UI selection

////////////////////////////////////////////////////////////////////////////////
// Begin doffing
	void BeginDoffingStage(eMode Mode, TMMessageNode Message = null)
	{
		Debug.Log("Begin doffing stage");

        AnalyticsRecorder.Instance.SetTimeOffset(m_TimeInScene);

		DirtyAvatarOff();
		EquipmentManager.Instance.DressAvatar(m_Avatar);
		m_Guided = false;
		GotoNextState();
	}

////////////////////////////////////////////////////////////////////////////////
// Doffing UI selection
	void GoodDoffingProcedure(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			m_NumCorrectDoffings++;
			if (m_NumCorrectDoffings == 1)
			{
				GameManager.Instance.Buddy.TriggerConversation("GoodDoffing_Repeat");
				ViewScene.instance.progressBar.SetModuleProgress(ModuleType.doffing, 0.66f, true);
			}
			if (m_NumCorrectDoffings >= 2)
			{
				m_CurrentState = m_DoffingCompleteState;
				m_CurrentState.m_Handler(eMode.Enter);
				ViewScene.instance.progressBar.SetModuleProgress(ModuleType.doffing, 1f, true);
				UserProfile.sCurrent.SetSceneCompletedTime(m_SceneIndex, m_TimeInScene, false);
			}
		}
		else if (Mode == eMode.UIMessage)
		{
			if (Message.Message == "BuddyDialogClosed".GetHashCode())
			{
				//Bumping back to do one more round of doffing
				m_CurrentState = m_DoffingStartState;
				m_CurrentState.m_Handler(eMode.Enter);
			}
		}
	}

////////////////////////////////////////////////////////////////////////////////
// Doffing UI selection

    // Refactor replacement function
    private void CreateDoffingUISelection(BaseActionScriptableObject actionSO, string MediaIdent)
    {
        State NewState = AllocateState(new Callback(DoffingUISelect));

        NewState.m_ActionSO = actionSO;
        NewState.m_UIWaitIdent = NewState.m_Action.ToString();
        NewState.m_MediaIdent = "GoodDoffing_" + MediaIdent;        

    }



    // Refactor replacement function
    void CreateDoffingUISelection(BaseActionScriptableObject actionSO, string MediaIdent, EquipmentManager.eEquipment AvatarEquipment)
    {
        State NewState = AllocateState(new Callback(DoffingUISelect));

        NewState.m_ActionSO = actionSO;
        NewState.m_UIWaitIdent = NewState.m_Action.ToString();
        NewState.m_MediaIdent = "GoodDoffing_" + MediaIdent;
        NewState.m_AvatarEquipment = AvatarEquipment;
    }




    // Refactor todo: Tidy this old code up and move it away from enums and to the BASOs
	void DoffingUISelect(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{

		}

		else if (Mode == eMode.ActionClicked)
		{
            if (m_CurrentState.m_ActionSO != null)
            {
                CorrectTestActionSelected(m_CurrentState.m_ActionSO);
            }

			//this needs a cleanup
			bool ShouldUseBuddyClose = (m_CurrentState.m_AvatarEquipment != EquipmentManager.eEquipment.FaceShield && m_CurrentState.m_AvatarEquipment != EquipmentManager.eEquipment.Hood);


			//select
			if (EquipmentManager.Instance.ProtectionRequired == ePPEType.Basic) {
				if (m_CurrentState.m_ActionSO.basicPPEanimationConversation != null) GameManager.Instance.Buddy.TriggerConversationById(m_CurrentState.m_ActionSO.basicPPEanimationConversation.name);
				else {
					//old sound
					GameManager.Instance.Buddy.TriggerConversation(m_CurrentState.m_MediaIdent, ShouldUseBuddyClose);
				}
			}
			else {
				//enchanced
				if (m_CurrentState.m_ActionSO.enchancedPPEanimationConversation != null) GameManager.Instance.Buddy.TriggerConversationById(m_CurrentState.m_ActionSO.enchancedPPEanimationConversation.name);
				else {
					//old sound
					GameManager.Instance.Buddy.TriggerConversation(m_CurrentState.m_MediaIdent, ShouldUseBuddyClose);
				}
			}


			if (m_CurrentState.m_AvatarEquipment != EquipmentManager.eEquipment.MAX)
			{
				m_UIItemSelection.Highlight(m_CurrentState.m_Equipment, false);

				if (m_CurrentState.m_AvatarEquipment != EquipmentManager.eEquipment.Boots && m_CurrentState.m_AvatarEquipment != EquipmentManager.eEquipment.BootsRemoval) 
				{
					m_Avatar.TakeOff(m_CurrentState.m_AvatarEquipment);

				}
			}

				m_UIItemSelection.Highlight(m_CurrentState.m_Action, false);
				GotoNextState();
		}

		else if (Mode == eMode.WrongActionClicked)
		{
            if (m_CurrentState.m_ActionSO != null)
            {
                WrongTestActionSelected(m_CurrentState.m_ActionSO, Message.ActionSO);
            }

			m_UnguidedIncorrectDoffingCount++;
			if (m_UnguidedIncorrectDoffingCount == 3)
			{
				m_CurrentState = m_RepeatGuidedState;
				m_UnguidedIncorrectDoffingCount = 0;
			}
			else
				m_CurrentState = m_BadDoffingState;

			m_NumCorrectDoffings = 0;
			
			m_CurrentState.m_Handler(eMode.Enter);
			ViewScene.instance.progressBar.SetModuleProgress(ModuleType.doffing, 0.33f, true);
			UserProfile.sCurrent.IncorrectDoffing();
		}
	}


//Doffing animation
	void CreateDoffingAnimation(BaseActionScriptableObject actionSO, string MediaIdent, EquipmentManager.eEquipment AvatarEquipment, float time = 0f)
	{
		State NewState = AllocateState(new Callback(DoffingAnimationHandler));
		
		NewState.m_ActionSO = actionSO;
		NewState.m_UIWaitIdent = NewState.m_Equipment.ToString();
		NewState.m_MediaIdent = MediaIdent;
		NewState.m_AvatarEquipment = AvatarEquipment;
		NewState.m_Timer = time;

	}
	


	
	//called during Animation, 
	void DoffingAnimationHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			if (m_CurrentState.m_ActionSO.Item == BaseActionScriptableObject.ITEM.FaceShield) {
				DirtyAvatarOn();

				if (m_NumCorrectDoffings == 0) {

					Debug.Log(m_CurrentState.m_MediaIdent);
					if (m_CurrentState.m_MediaIdent == "virusDropLeanForward" || m_CurrentState.m_MediaIdent == "virusDropContinue") {
						//skip 
						Debug.Log("SKIP");
//						GotoNextState();
						m_CurrentState.m_Timer = 0;
					}
					else {
						m_CurrentState.m_Timer = m_Avatar.PlayDoffingAnimationWithTime(6f, "Doff-FaceShield");
					}
				}
				else {
					//second round virus drop
					if (m_CurrentState.m_MediaIdent == "virusDropLeanForward") {
						//first stage of virus drop animation
						m_CurrentState.m_Timer = m_Avatar.PlayDoffingAnimationWithTime(1.5f, "shieldoff");
						GameManager.Instance.Buddy.TriggerConversationById("11.16");
					}
					else if (m_CurrentState.m_MediaIdent == "virusDropContinue") {
						//last stage of virus drop animaition after camera flythrough
						m_CurrentState.m_Timer = m_Avatar.PlayDoffingAnimationWithTime(5.7f, "virusDropletContinue");
						GameManager.Instance.Buddy.TriggerConversationById("11.16.T");
					}
				}
			}
		}
		else if (Mode == eMode.Running)
		{
			//MAIN TIMER
			m_CurrentState.m_Timer -= Time.deltaTime;
			if (m_CurrentState.m_Timer <= 0)
			{
				//closing of state
				if (m_CurrentState.m_ActionSO.Item == BaseActionScriptableObject.ITEM.FaceShield) {
					//dispose face shield
					if (m_NumCorrectDoffings == 0) {
						m_Avatar.HideDisposedObject(EquipmentManager.eEquipment.FaceShield);
					}
					else {

						Debug.Log(m_CurrentState.m_MediaIdent);

						//virus drop
						if (m_CurrentState.m_MediaIdent == "virusDropContinue") {
							m_Avatar.HideDisposedObject(EquipmentManager.eEquipment.FaceShield);
						}
					}

				}
				GotoNextState();
			}
		}
	}



#endregion


	public override void MoviePlayback(eMode Mode, TMMessageNode Message = null) {

		if (m_CurrentState.m_ActionSO != null) {

			if (Mode == eMode.Enter)
			{
				if (m_CurrentState.m_ActionSO.Item == BaseActionScriptableObject.ITEM.Gown) {
					m_CriticalSteps.Init(gownDoffCriticalSteps, false);
					Debug.Log("Init gown critical steps");
				}	                    
			}

			else if (Mode == eMode.Running) {


				if (m_CurrentState.m_ActionSO.Item == BaseActionScriptableObject.ITEM.Gown) {
					//gown critical steps
					if (m_CriticalSteps.currentIndex == 0 && MediaClipManager.Instance.clipTime > 25f) {
//					if (m_CriticalSteps.currentIndex == 0 && MediaClipManager.Instance.clipTime > 2f) {
						m_CriticalSteps.ShowNext();
					}
					else if (m_CriticalSteps.currentIndex == 1 && MediaClipManager.Instance.clipTime > 32f) {
						m_CriticalSteps.ShowNext("8.09.3");
					}
					else if (m_CriticalSteps.currentIndex == 2 && MediaClipManager.Instance.clipTime > 40f) {
						m_CriticalSteps.ShowNext("8.09.4");
					}
					else if (m_CriticalSteps.currentIndex == 3 && MediaClipManager.Instance.clipTime > 55f) {
						m_CriticalSteps.ShowNext("8.09.5");
					}
				}
				
			}


			else if (Mode == eMode.UIMessage)
			{
				if (Message.Message == "MediaClipFinished".GetHashCode())
				{
					if (m_CurrentState.m_ActionSO.Item == BaseActionScriptableObject.ITEM.Gown) {
						m_CriticalSteps.CleanUp();
					}

				}
			}
			else if (Mode == eMode.UserQuit)
			{
				if (m_CurrentState.m_ActionSO.Item == BaseActionScriptableObject.ITEM.Gown) {
					m_CriticalSteps.CleanUp();
				}
			}
		}

		base.MoviePlayback(Mode, Message);
	}





	//virusDrop
	void CreateVirusDropAnimation(BaseActionScriptableObject actionSO)
	{
		State NewState = AllocateState(new Callback(VirusDropAnimationHandler));
		
		NewState.m_ActionSO = actionSO;
		NewState.m_MediaIdent = "virusDrop";
		
	}


	void VirusDropAnimationHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			if (m_CurrentState.m_ActionSO.Item == BaseActionScriptableObject.ITEM.FaceShield) {
				
				if (m_NumCorrectDoffings == 0) {
					//skip if this is first round
					GotoNextState();
					return;
				}
				else {
					//second round virus drop
					m_CurrentState.m_Timer = virusDropAnimation.clip.length;
					virusDropAnimation.gameObject.SetActive(true);
					virusDropAnimation.Play();
				}
			}
		}
		else if (Mode == eMode.Running)
		{
			//MAIN TIMER
			m_CurrentState.m_Timer -= Time.deltaTime;
			if (m_CurrentState.m_Timer <= 0)
			{
				virusDropAnimation.gameObject.SetActive(false);
				GotoNextState();
			}
		}
	}



	//QUIZ

	protected void CreateQuizSelection(BaseActionScriptableObject actionSO, Quiz sourceQuiz, int step)
	{
		QuizState NewState = AllocateState(new Callback(QuizUISelect), new QuizState()) as QuizState;
		NewState.m_ActionSO = actionSO;
		Quiz quiz = new Quiz();
		quiz.images = sourceQuiz.images;
		quiz.Setup(quiz.images[step]);
        quiz.Question = sourceQuiz.Question;
		NewState.quiz = quiz;
		
	}
	

	void QuizUISelect(eMode Mode, TMMessageNode Message = null) {

		if (Mode == eMode.Enter)
		{
			if (m_CurrentState.m_ActionSO.Item == BaseActionScriptableObject.ITEM.FaceShield) {
				
				if (m_NumCorrectDoffings > 0) {
					//override SKIP for virus droplet on second round
					GotoNextState();
					return;
				}

			}

			Quiz quiz = ((QuizState)m_CurrentState).quiz;

			//show quiz UI
			OurCamera.Instance.BlurOn();
			quizUI.Show(quiz);            
		}

	}


	//called from quizUI directly
	public void QuizAnswer(Texture2D answer) {

		Quiz quiz = ((QuizState)m_CurrentState).quiz;

		if (quiz.Answer(answer)) {
            
            Debug.Log("Quiz correct! Expected: " + quiz.GetCorrectAnswer().name + " and got" + answer.name);

			//dirty dirty! remove this after introducing final rig
			if (m_CurrentState.m_ActionSO.Item == BaseActionScriptableObject.ITEM.FaceShield) {

			}

			//correct answer
			OurCamera.Instance.BlurOff();
			quizUI.Hide();
			GotoNextState();
		}
		else {
            Debug.Log("Wrong quiz! Expected: " + quiz.GetCorrectAnswer().name + " but got " + answer.name);
			//wrong answer
			//mark wrong answer here
			quizUI.MarkWrongAnswer(answer);
		}

        AnalyticsRecorder.Instance.LogQuizAnswer(quiz, answer.name);
	}





////////////////////////////////////////////////////////////////////////////////
// Incorrect buddy UI
	State CreateIncorrectDoffingBuddyUI(string MediaIdent, string UIWaitIdent, State NextState)
	{
		State		NewState = new State();
		
		NewState.m_Handler = new Callback(IncorrectBuddyUIHandler);
		NewState.m_UIWaitIdent = UIWaitIdent;
		NewState.m_MediaIdent = MediaIdent;
		NewState.m_Next = NextState;
		m_States.Add(NewState);
		m_IncorrectDoffingStates.Add(NewState);
		return (NewState);
	}

	void IncorrectBuddyUIHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			GameManager.Instance.Buddy.TriggerConversation(m_CurrentState.m_MediaIdent, true);
		}
		else if (Mode == eMode.DialogClosed || Mode == eMode.Exit)
		{
			EquipmentManager.Instance.DressAvatar(m_Avatar);
			GotoNextState();
		}
	}





////////////////////////////////////////////////////////////////////////////////
// Guided animation handler       
    void CreateGuidedDoffingAnimation(BaseActionScriptableObject actionSO, string MediaIdent, EquipmentManager.eEquipment AvatarEquipment, float EarlyExitTime = 0f, int criticalStep = -1)
    {
        State NewState = AllocateState(new Callback(GuidedAnimHandler));

        NewState.m_ActionSO = actionSO;
        NewState.m_UIWaitIdent = NewState.m_Equipment.ToString();
        NewState.m_AvatarEquipment = AvatarEquipment;
        NewState.m_Timer = EarlyExitTime;


		if (criticalStep != -1) 
		{
			Debug.Log("Create doff animation with critical stage: " + criticalStep.ToString());
			NewState.criticalStep = criticalStep;
		}
    }

	


	//called during Animation, 
	void GuidedAnimHandler(eMode Mode, TMMessageNode Message = null)
	{

		if (Mode == eMode.Enter)
		{
			Debug.Log(m_CurrentState.m_ActionSO.Item);
			if (m_CurrentState.m_ActionSO.Item == BaseActionScriptableObject.ITEM.ReusableApron) {
				m_Avatar.SwapApron();
			}

			if(m_CurrentState.m_Timer == 0)
				m_CurrentState.m_Timer = m_Avatar.PlayDoffingAnimation(m_CurrentState.m_AvatarEquipment);
		
			if (m_CurrentState.criticalStep != 0)
			{
				if (m_CurrentState.criticalStep == 4) {
					m_CurrentState.m_Timer = m_Avatar.PlayDoffingAnimationWithTime(4.5f, "shieldoff4");
					//hide critical steps UI
					m_CriticalSteps.CleanUp();				
				}
			}
			else {

				m_Avatar.PlayDoffingAnimation(m_CurrentState.m_AvatarEquipment);
			}

			if (m_CurrentState.m_Timer < 0)
				m_CurrentState.m_Timer = 3;

		}
		else if (Mode == eMode.Running)
		{
			//MAIN TIMER
			m_CurrentState.m_Timer -= Time.deltaTime;
			if (m_CurrentState.m_Timer <= 0)
			{
				if (m_CurrentState.criticalStep == 4) {
					//hide 
					m_Avatar.HideDisposedObject(EquipmentManager.eEquipment.FaceShield);
					Invoke("DirtyAvatarOff", 1.5f);
				}

				GotoNextState();
			}
		}

	}



////////////////////////////////////////////////////////////////////////////////
// Disposal handler
	void CreateDisposal(EquipmentManager.eEquipment Equipment)
	{
		State NewState = AllocateState(new Callback(DisposalHandler));

		NewState.m_Equipment = Equipment;
	}


	void DisposalHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			m_CurrentState.m_Timer = m_Avatar.PlayDisposalAnimationOnActiveEquipment();
			if(m_CurrentState.m_Timer == 0)
				GotoNextState();
		}
		else if (Mode == eMode.Running)
		{
			m_CurrentState.m_Timer -= Time.deltaTime;
			if (m_CurrentState.m_Timer <= 0)
			{
				OurCamera.Instance.MoveTo(OurCamera.eCameraFocus.Main, 0.7f);
				GotoNextState();
			}
		}
	}

////////////////////////////////////////////////////////////////////////////////
// Disposal handler
	void SwapToDisposalObject(eMode Mode, TMMessageNode Message = null)
	{
		m_Avatar.SwapToDisposalOject();
		GotoNextState();
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
                    m_CurrentState.m_Handler(eMode.ActionClicked);
                else
                    m_CurrentState.m_Handler(eMode.WrongActionClicked, Message);
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
		else if (Message.Message == "HideEbola".GetHashCode())
		{
            // Hide the bucket if it's our second go around
            GameObject bucket = GameObject.Find("Boot_Washing_Bucket");

            if (bucket != null)
                bucket.SetActive(false);

			m_Avatar.EbolaDisplayDisable();
		}
		else if (Message.Message == "ShowEbola".GetHashCode())
		{
			m_Avatar.EbolaDisplayEnable();
		}

		else if (Message.Message == "FullScreenButtonTapped".GetHashCode()) {

			Debug.Log("Full screen clicked message received");
			GotoNextState();
		}
		else m_CurrentState.m_Handler(eMode.UIMessage, Message);

	}




	//THIS NEED REFACTORING ASAP!
	void Update()
	{
		if (m_UIItemSelection == null)
			m_UIItemSelection = GameObject.FindObjectOfType<UIItemSelection>();
		

		if (m_CurrentState != null)
		{
			m_CurrentState.m_Handler(eMode.Running);
			m_TimeInScene += Time.deltaTime;
		}
	}


	void OnDestroy()
	{
		TMMessenger.Instance.RemoveFromMessageQueue(this);
	}



	////////////////////////////////////////////////////////////////////////////////
	// End of doffing
	void EndOfScene(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			UIFinishedPopup.instance.Show(ModuleType.doffing);

		}
		else if (Mode == eMode.UIMessage)
		{
			if (Message.Message == "END_OF_SCENE".GetHashCode())
			{
				EndScene(eMode.Enter);
			}
		}
	}



}



