using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMSupport;

public class Scene : MonoBehaviour
{
	protected static float		sScenePercentage = 25.0f;

	protected delegate void Callback(eMode Mode, TMMessageNode Message = null);

	//remove ASAP
	public Avatar oldAvatar;
	public Avatar tempAvatar;
	public AudioClip genericHandWashAudio;

	public enum eMode
	{
		Enter = 0,
		Running,
		ActionClicked,
		WrongActionClicked,
		DialogClosed,
		UIMessage,
		AudioFinished,
		UserQuit,
		Exit
	}

	protected class State
	{
        internal BaseActionScriptableObject     m_ActionSO;
		internal Callback						m_Handler;
		internal string							m_UIWaitIdent;
		internal eActions						m_Action = eActions.MAX;
		internal EquipmentManager.eEquipment	m_Equipment = EquipmentManager.eEquipment.MAX;
		internal string							m_MediaIdent;
		internal EquipmentManager.eEquipment	m_AvatarEquipment = EquipmentManager.eEquipment.MAX;
		internal bool							m_Guided;
		internal float							m_Timer;
		internal bool							m_Flag;
		internal int 							criticalStep = 0;

		internal State							m_Next;
		internal int							m_Index;
		internal float timeActive = 0;

		public override string ToString()
		{
			string s = "";
			s += "m_Index           = " + m_Index.ToString() + "\n";
			if( m_Next != null )
				s += "Next State Index  = " + m_Next.m_Index.ToString() + "\n";
			s += "m_UIWaitIdent     = " + m_UIWaitIdent + "\n";
			s += "m_Action          = " + m_Action.ToString() + "\n";
			s += "m_Equipment       = " + m_Equipment.ToString() + "\n";
			s += "m_MediaIdent      = " + m_MediaIdent + "\n";
			s += "m_AvatarEquipment = " + m_AvatarEquipment.ToString() + "\n";
			s += "m_Guided          = " + m_Guided.ToString() + "\n";
			s += "m_Timer           = " + m_Timer.ToString() + "\n";
			s += "m_Flag            = " + m_Flag.ToString() + "\n";
			s += "----------------\n";
			return s;
		}
	}


	protected class VideoState : State
	{
		internal AudioClip audioClip;
	}



	protected class ObjectState : State
	{
		internal GameObject						m_GameObject;
		internal bool							m_Boolean;
        internal Vector3                        m_Position;
		internal string videoPath;

		public override string ToString()
		{
			string s = "ObjectState\n";
			s += "----------------\n";
			s += base.ToString();
			if( m_GameObject != null )
				s += "m_GameObject = " + m_GameObject.name + "\n";
			s += "m_Boolean  = " + m_Boolean.ToString() + "\n";
			s += "m_Position = " + m_Position.ToString() + "\n";
			return s;
		}
	}
	protected class CameraCutState : State
	{
		internal OurCamera.eCameraFocus			m_CameraFocus;
		internal float							m_EarlyOutTimer;

		public override string ToString()
		{
			string s = "CameraCutState\n";
			s += "----------------\n";
			s += base.ToString();
			s += "m_CameraFocus   = " + m_CameraFocus.ToString() + "\n";
			s += "m_EarlyOutTimer = " + m_EarlyOutTimer.ToString() + "\n";
			return s;
		}
	}

	protected class CameraAnimatorState : State
	{
		internal string triggerName = "";
		internal float m_EarlyOutTimer;
		
		public override string ToString()
		{
			string s = "CameraAnimatorState\n";
			s += "----------------\n";
			s += base.ToString();
			s += "m_EarlyOutTimer = " + m_EarlyOutTimer.ToString() + "\n";
			return s;
		}
	}

	
	protected class RigSwapState : State
	{
		internal Avatar newAvatar;
		
		public override string ToString()
		{
			string s = "RigSwapState\n";
			s += "----------------\n";
			s += base.ToString();
			return s;
		}
	}

	protected class ProgressState : State
	{
		internal ModuleType moduleType;
		internal float normalizedProgress;
		internal bool animated;
	}



	protected class RightWrongState : State
	{
		internal List<Sprite>					m_Sprites;
		internal string							m_CorrectMediaIdent;
		internal string							m_IncorrectMediaIdent;
		internal bool							m_CorrectChosen;
		internal bool							m_HaveChosen;
		internal RightWrongDialog				m_DialogUsed;

		public override string ToString()
		{
			string s = "RightWrongState\n";
			s += "----------------\n";
			s += base.ToString();
			s += "m_CorrectMediaIdent   = " + m_CorrectMediaIdent + "\n";
			s += "m_IncorrectMediaIdent = " + m_IncorrectMediaIdent + "\n";
			s += "m_CorrectChosen       = " + m_CorrectChosen.ToString() + "\n";
			s += "m_HaveChosen          = " + m_HaveChosen.ToString() + "\n";
			return s;
		}
	}

	protected class CriticalStepsState : State
	{
		internal List<Texture2D> images;
		
		public override string ToString()
		{
			string s = "CriticalStepsState\n";
			s += "----------------\n";
			s += base.ToString();
			return s;
		}
	}


	protected class QuizState : State
	{
		internal Quiz quiz;

		public override string ToString()
		{
			string s = "QuizState\n";
			s += "----------------\n";
			s += base.ToString();
			return s;
		}
	}

	protected class RightWrongQuizState : State
	{
		internal RightWrongQuiz quiz;
		
		public override string ToString()
		{
			string s = "RightWrongQuizState\n";
			s += "----------------\n";
			s += base.ToString();
			return s;
		}
	}



	protected class QuestionState : State
	{
		internal Question question;
		
		public override string ToString()
		{
			string s = "QuestionState\n";
			s += "----------------\n";
			s += base.ToString();
			return s;
		}
	}




	protected List<State>	m_States;
	protected State			m_CurrentState;

	public enum eActions
	{
		Preparation = 0,
		WashHands,
		FaceMask,
		Gloves,
		Gown,
		Hood,
		Apron,
		FaceShield,
		Inspect,
		Boots,
		CleanBoots,
		ThrowAway,
		ThrowInBucket,
		Name,
		TakeTemperature,
		AskQuestions,
		MAX
	}

	[HideInInspector] public  Avatar			m_Avatar;
	protected int				m_SceneIndex;
	public int					SceneIndex { get { return m_SceneIndex; } }

	protected UIItemSelection	m_UIItemSelection;
	protected UIDialog			m_BuddyDialog;

	protected bool				m_Guided;

	protected float				m_TimeInScene;

	void Awake()
	{
		Init();

        Screen.SetResolution(1280, 800, true);
	}

	protected virtual void Init()
	{
		// This is a hack so playing a scene from the editor will load the first user profile it finds
//		if (UserProfile.sCurrent.Age == 0)
//			UserProfile.Load();

		m_States = new List<State>();
		m_TimeInScene = 0;

		Instantiate(Resources.Load("Prefabs/Reporting/HeatMapper"));

        AnalyticsRecorder.Instance.StartSession(Application.loadedLevelName);
	}

    public int GetCurrentStateIndex()
    {
        if (m_States != null)
        {
            if (m_CurrentState != null)
                return m_CurrentState.m_Index;
            else
                return -1;
        }
        else
            return -1;
    }

	internal void SetAvatar(Avatar NewAvatar)
	{
		m_Avatar = NewAvatar;
	}

	protected State AllocateState(Callback Handler, State NewState = null)
	{
		//Debug.LogWarning( "AllocateState...");
		if(NewState == null)
			NewState = new State();

		if (m_States.Count > 0 && m_States[m_States.Count - 1].m_Next == null)
			m_States[m_States.Count - 1].m_Next = NewState;

		NewState.m_Handler = Handler;
		NewState.m_Index = m_States.Count;
		m_States.Add(NewState);
		return (NewState);
	}

////////////////////////////////////////////////////////////////////////////////
// Buddy dialog

	//old
	protected State CreateBuddyUI(string MediaIdent, string UIWaitIdent = null)
	{
		State NewState = AllocateState(new Callback(BuddyUIHandler));

		NewState.m_UIWaitIdent = UIWaitIdent;
		NewState.m_MediaIdent = MediaIdent;
		return(NewState);
	}

	//refactored
	protected State CreateBuddyConversationUI(string conversationId, string UIWaitIdent = null)
	{
		State		NewState = AllocateState(new Callback(BuddyUIConversationHandler));
		
		NewState.m_UIWaitIdent = UIWaitIdent;
		NewState.m_MediaIdent = conversationId;
		return(NewState);
	}


	//Old need removing
	public virtual void BuddyUIHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			int num = 0;
			if (int.TryParse(m_CurrentState.m_MediaIdent.Substring(0, 1), out num)) {
				GameManager.Instance.Buddy.TriggerConversationById(m_CurrentState.m_MediaIdent, m_CurrentState.m_UIWaitIdent == "BuddyDialogClosed");
			}
			else {
				//old sound
				GameManager.Instance.Buddy.TriggerConversation(m_CurrentState.m_MediaIdent, m_CurrentState.m_UIWaitIdent == "BuddyDialogClosed");
			}

		}
		else if (Mode == eMode.DialogClosed || Mode == eMode.Exit)
		{
			GotoNextState();
		}
	}

	//refactored new function
	public virtual void BuddyUIConversationHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			GameManager.Instance.Buddy.TriggerConversationById(m_CurrentState.m_MediaIdent, m_CurrentState.m_UIWaitIdent == "BuddyDialogClosed");
			
		}
		else if (Mode == eMode.DialogClosed || Mode == eMode.Exit)
		{



			GotoNextState();
		}
	}


////////////////////////////////////////////////////////////////////////////////
// Scene description
	protected void CreateSceneDescriptionUI(string MediaIdent, string UIWaitIdent)
	{
		State		NewState = AllocateState(new Callback(SceneDescriptionHandler));

		NewState.m_UIWaitIdent = UIWaitIdent;
		NewState.m_MediaIdent = MediaIdent;
	}

	void SceneDescriptionHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			// Set the text
			string Text = "";
			string AudioFileName = "";
			string Ident = m_CurrentState.m_MediaIdent;
			string Header = MediaNodeManager.GetText("Header_" + m_CurrentState.m_MediaIdent);

			if (MediaNodeManager.GetTextAndAudio(Ident, ref Text, ref AudioFileName) == true)
			{
				MediaClipManager.Instance.Play(AudioFileName);
				UIManager.Instance.BringUpDialog(UIManager.eUIReferenceName.SceneDescription1, Text, Header);
			}
		}
		else if (Mode == eMode.Exit)
		{
			GotoState(m_CurrentState.m_Next);
		}
	}

////////////////////////////////////////////////////////////////////////////////
// Movie playback





	protected void CreateMoviePlayback(string MediaIdent, AudioClip audioClip = null, BaseActionScriptableObject actionSO = null)
	{
//        Debug.Log("CreateMoviePlayBack: " + MediaIdent);

		VideoState NewState = AllocateState(new Callback(MoviePlayback), new VideoState()) as VideoState;

		if (actionSO != null) NewState.m_ActionSO = actionSO;
		NewState.m_MediaIdent = MediaIdent;
		if (audioClip != null) NewState.audioClip = audioClip;
	}




	public virtual void MoviePlayback(eMode Mode, TMMessageNode Message = null)
	{
		VideoState ThisState = m_CurrentState as VideoState;

		if (Mode == eMode.Enter)
		{
			MediaClipManager.Instance.PlayMovie(ThisState.m_MediaIdent, ThisState.audioClip);
//			OurCamera.Instance.BlurOn();

		}

		else if (Mode == eMode.UIMessage)
		{
			if (Message.Message == "MediaClipFinished".GetHashCode())
			{
				UIManager.Instance.CloseUI(UIManager.eUIReferenceName.MoviePlay);
				GotoState(m_CurrentState.m_Next);
//				OurCamera.Instance.BlurOff();
			}
		}
		else if (Mode == eMode.UserQuit)
		{
			UIManager.Instance.CloseUI(UIManager.eUIReferenceName.MoviePlay);
//			OurCamera.Instance.BlurOff();
		}
	}


	//progress
	
	protected void CreateProgressState(float normalizedProgress, ModuleType moduleType, bool animated)
	{
		ProgressState NewState = AllocateState(new Callback(ProgressCallback), new ProgressState()) as ProgressState;
		NewState.normalizedProgress = normalizedProgress;
		NewState.moduleType = moduleType;
		NewState.animated = animated;
	}
	
	
	
	
	public virtual void ProgressCallback(eMode Mode, TMMessageNode Message = null)
	{
		ProgressState ThisState = m_CurrentState as ProgressState;
		
		if (Mode == eMode.Enter) {
			ViewScene.instance.progressBar.SetModuleProgress(ThisState.moduleType, ThisState.normalizedProgress, ThisState.animated); 
			GotoNextState();
		}
	}







////////////////////////////////////////////////////////////////////////////////
// Camera cut with early out
	protected State CreateCameraCut(OurCamera.eCameraFocus CameraFocus, float CameraCutTime, float EarlyOutTime = -1)
	{
		CameraCutState		NewState = AllocateState(new Callback(CameraCut), new CameraCutState()) as CameraCutState;

		NewState.m_CameraFocus = CameraFocus;
		NewState.m_Timer = CameraCutTime;
		NewState.m_EarlyOutTimer = EarlyOutTime;
		return(NewState);
	}


	void CameraCut(eMode Mode, TMMessageNode Message = null)
	{
		CameraCutState		ThisState = m_CurrentState as CameraCutState;

		if (Mode == eMode.Enter)
		{
			bool	NewFocus = OurCamera.Instance.MoveTo(ThisState.m_CameraFocus, m_CurrentState.m_Timer);

			if (m_CurrentState.m_Timer == 0 || !NewFocus)
				GotoState(m_CurrentState.m_Next);
			else if (ThisState.m_EarlyOutTimer > 0)
				Invoke("GotoNextState", ThisState.m_EarlyOutTimer);
		}
		else if (Mode == eMode.UIMessage)
		{
			if (Message.Message == "CameraCutFinished".GetHashCode())
			{
				GotoNextState();
			}
		}
	}

////////////////////////////////////////////////////////////////////////////////
// Enabling a GameObject
	protected void EnableObject(GameObject BaseObject, bool Enable, string videoPath = "")
	{
		ObjectState		NewState = AllocateState(new Callback(ObjectEnableHandler), new ObjectState()) as ObjectState;

		NewState.m_GameObject = BaseObject;
		NewState.m_Boolean = Enable;
		NewState.videoPath = videoPath;
	}

	void ObjectEnableHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			ObjectState		ThisState = m_CurrentState as ObjectState;

			ThisState.m_GameObject.SetActive(ThisState.m_Boolean);
			GotoNextState();
		}
	}


////////////////////////////////////////////////////////////////////////////////
// Position an avatar
    protected void PositionObject(GameObject BaseObject, Vector3 position)
    {
        ObjectState NewState = AllocateState(new Callback(PositionObjectHandler), new ObjectState()) as ObjectState;

        NewState.m_GameObject = BaseObject;
        NewState.m_Position = position;
    }

    void PositionObjectHandler(eMode Mode, TMMessageNode Message = null)
    {
        if (Mode == eMode.Enter)
        {
            ObjectState ThisState = m_CurrentState as ObjectState;

            ThisState.m_GameObject.transform.position = ThisState.m_Position;

            GotoNextState();
        }
    }

////////////////////////////////////////////////////////////////////////////////
// Waiting for a specified amount of time
	protected void CreateWait(float Time, string UIWaitIdent = null)
	{
		State		NewState = AllocateState(new Callback(Wait));

		NewState.m_Timer = Time;
		NewState.m_UIWaitIdent = UIWaitIdent;
	}

	public virtual void Wait(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			if (m_CurrentState.m_Timer > 0)
				Invoke("GotoNextState", m_CurrentState.m_Timer);
		}
		else if (Mode == eMode.Exit)
		{
			GotoState(m_CurrentState.m_Next);
		}
		else if (Mode == eMode.Running) {
			m_CurrentState.timeActive += Time.deltaTime;
		}
	}

////////////////////////////////////////////////////////////////////////////////
// Send a message
	protected void CreateMessage(string Message, float Value = 0)
	{
		State		NewState = AllocateState(new Callback(MessageHandler));

		NewState.m_MediaIdent = Message;
		NewState.m_Timer = Value;
	}

	void MessageHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			TMMessenger.Send(m_CurrentState.m_MediaIdent.GetHashCode(), 0, m_CurrentState.m_Timer);
			GotoState(m_CurrentState.m_Next);
		}
	}

////////////////////////////////////////////////////////////////////////////////
// Enable canvas objects
	protected void EnableCanvasObjects(GameObject BaseObject, bool Enable)
	{
		ObjectState		NewState = AllocateState(new Callback(CanvasObjectEnableHandler), new ObjectState()) as ObjectState;

		NewState.m_GameObject = BaseObject;
		NewState.m_Boolean = Enable;
	}

	void CanvasObjectEnableHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			ObjectState		ThisState = m_CurrentState as ObjectState;

			EnableCanvasUI(ThisState.m_GameObject, ThisState.m_Boolean);
			GotoNextState();
		}
	}



////////////////////////////////////////////////////////////////////////////////
// Right/Wrong UI selection
	protected virtual void CreateRightWrongUISelection(EquipmentManager.eEquipment Equipment, Sprite CorrectImage, Sprite InCorrectImage, string QuestionIdent, string MediaBaseName, bool Guided)
	{
		RightWrongState		NewState = AllocateState(new Callback(RightWrongUISelect), new RightWrongState()) as RightWrongState;

		NewState.m_AvatarEquipment = Equipment;
		NewState.m_Sprites = new List<Sprite>(2);
		NewState.m_Sprites.Add(CorrectImage);
		NewState.m_Sprites.Add(InCorrectImage);
		NewState.m_MediaIdent = QuestionIdent;
		NewState.m_CorrectMediaIdent = MediaBaseName + "Correct_" + Equipment.ToString();
		NewState.m_IncorrectMediaIdent = MediaBaseName + "Incorrect_" + Equipment.ToString();
		NewState.m_Guided = Guided;
		NewState.m_CorrectChosen = false;
		NewState.m_HaveChosen = false;
	}



	protected virtual void RightWrongUISelect(eMode Mode, TMMessageNode Message = null)
	{
		RightWrongState		ThisState = m_CurrentState as RightWrongState;

		if (Mode == eMode.Enter)
		{
			ThisState.m_DialogUsed = UIManager.Instance.BringUpCanvas(UIManager.eUIReferenceName.RightWrongSelection) as RightWrongDialog;
			ThisState.m_DialogUsed.SetCorrectAndIncorrectImages(ThisState.m_Sprites[0], ThisState.m_Sprites[1], ThisState.m_Guided);
			ThisState.m_DialogUsed.EnableButtons(true);
			ThisState.m_CorrectChosen = false;
			ThisState.m_HaveChosen = false;
			OurCamera.Instance.BlurOn();
		}
		if (Mode == eMode.UIMessage)
		{
//			bool		CanHaveAnouthGo = m_Guided || ThisState.m_HaveChosen == false;
			bool		CanHaveAnouthGo = true;

			if (CanHaveAnouthGo && Message.Message == "RightWrong_Correct".GetHashCode())
			{
				ThisState.m_CorrectChosen = true;
				ThisState.m_HaveChosen = true;
				GameManager.Instance.Buddy.TriggerConversation(ThisState.m_CorrectMediaIdent);
				ThisState.m_DialogUsed.EnableButtons(false);
				OurCamera.Instance.BlurOff();
			}
			else if (CanHaveAnouthGo && Message.Message == "RightWrong_Incorrect".GetHashCode())
			{
				ThisState.m_CorrectChosen = false;
				ThisState.m_HaveChosen = true;
				GameManager.Instance.Buddy.TriggerConversation(ThisState.m_IncorrectMediaIdent);
				ThisState.m_DialogUsed.EnableButtons(false);
				OurCamera.Instance.BlurOff();
			}
			else if (ThisState.m_HaveChosen && Message.Message == "BuddyDialogClosed".GetHashCode())
			{
				if (ThisState.m_CorrectChosen)	// || m_Guided == false)
					UIManager.Instance.CloseUI(UIManager.eUIReferenceName.RightWrongSelection);

				if(ThisState.m_CorrectChosen)
					RightWrong_Correct();
				else
					RightWrong_Incorrect();
			}

		}

	}

	protected virtual void RightWrong_Correct()
	{
		GotoNextState();
	}
	protected virtual void RightWrong_Incorrect()
	{
	}



	//TODO REMOVE ASAP, old progress bar code

////////////////////////////////////////////////////////////////////////////////
// Set progress
	protected State CreateSetProgress(float Progress)
	{
		State		NewState = AllocateState(new Callback(SetProgressHandler));

		NewState.m_Timer = Progress;
		return (NewState);
	}

	void SetProgressHandler(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			UIProgressBar.SetProgressPercentage(m_CurrentState.m_Timer);
			GotoNextState();
		}
	}


////////////////////////////////////////////////////////////////////////////////
// End the scene

	protected void CreateEndScene()
	{
		AllocateState(new Callback(EndScene));
	}

	protected void EnableCanvasUI(GameObject BaseObject, bool Enable = true)
	{
		Canvas[] CanvasObjects = BaseObject.GetComponentsInChildren<Canvas>();

		for (int loop = 0; loop < CanvasObjects.Length; loop++)
		{
			CanvasObjects[loop].enabled = Enable;
		}
	}

////////////////////////////////////////////////////////////////////////////////
// Handle callbacks
	protected State CreateCallback(Callback TheCallback)
	{
		return(AllocateState(TheCallback));
	}

	public virtual void GotoNextState()
	{
		GotoState(m_CurrentState.m_Next);
	}

	protected virtual void GotoState(State NextState)
	{        
		m_CurrentState = NextState;

		//Debug.Log( "State Transition : \n" + m_CurrentState.ToString() );

		if (m_CurrentState == null)
			EndScene();
		else if (m_CurrentState.m_Handler != null)
			m_CurrentState.m_Handler(eMode.Enter);        
	}

	protected void SetSceneCompletionTime(eMode Mode = eMode.UIMessage, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
			UserProfile.sCurrent.SetSceneCompletedTime(m_SceneIndex, m_TimeInScene, false);
			GotoNextState();
		}
	}

	protected void EndScene(eMode Mode = eMode.UIMessage, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter)
		{
            AnalyticsRecorder.Instance.StopSession();

			// Update the save so this scene is now finished and so unlocked for practice
			UserProfile.sCurrent.SceneComplete(m_SceneIndex, false);

			Exit();
			if (m_SceneIndex == 2)
			{
				UserProfile.sCurrent.Save();
				TMMessenger.Send("RETURNTOTITLE".GetHashCode());
			}
			else
			{
				// If the next scene has never been played before, then set the completion count
				if (UserProfile.sCurrent.GetSceneCompleteCount(m_SceneIndex + 1) == -1)
					UserProfile.sCurrent.SceneComplete(m_SceneIndex + 1, false);

				UserProfile.sCurrent.Save();
				// Splash is 1st scene, title is 2nd scene, levels start at 2, so next start at +3
//				TMMessenger.Send("LOADSCENE".GetHashCode(), 0, m_SceneIndex + 3);

				//back to title
				TMMessenger.Send("RETURNTOTITLE".GetHashCode());

			}
		}
	}

	public virtual void Exit()
	{
	}

    // Called during test stage when the user selected the wrong action to perform next
	protected virtual void WrongTestActionSelected(BaseActionScriptableObject expectedBASO, BaseActionScriptableObject clickedBASO, Dictionary<string, string> pAdditionalAttributes = null)
    {
        if (expectedBASO == null || clickedBASO == null)
        {
            Debug.LogWarning("Null expectedBASO or clickedBASO");
            return;
        }

        Debug.Log("WRONG ACTION! Expected: " + expectedBASO.Name + " but you clicked " + clickedBASO.Name + " at time: " + m_TimeInScene.ToString());
		AnalyticsRecorder.Instance.LogWrongAction(expectedBASO.Name, clickedBASO.Name, m_TimeInScene, pAdditionalAttributes);
    }

    // Called during test stage when the user selected the correct action to perform next
	protected virtual void CorrectTestActionSelected(BaseActionScriptableObject selectedBASO, Dictionary<string, string> pAdditionalAttributes = null)
    {
        Debug.Log("CORRECT ACTION! " + selectedBASO.Name + " at time: " + m_TimeInScene.ToString());
        AnalyticsRecorder.Instance.LogCorrectAction(selectedBASO.Name, m_TimeInScene, pAdditionalAttributes);
    }








	protected virtual void CreateRightWrongQuizSelection(BaseActionScriptableObject actionSO, RightWrongQuiz sourceQuiz, QuizType type)
	{
		RightWrongQuizState NewState = AllocateState(new Callback(RightWrongQuizUISelect), new RightWrongQuizState()) as RightWrongQuizState;
		NewState.m_ActionSO = actionSO;
		RightWrongQuiz quiz = new RightWrongQuiz();
		quiz.correctImage = sourceQuiz.correctImage;
		quiz.incorrectImage = sourceQuiz.incorrectImage;
		quiz.question = sourceQuiz.question;
		quiz.type = type;
		NewState.quiz = quiz;	
	}


	protected virtual void CreateRightWrongQuizSelection(RightWrongQuiz sourceQuiz)
	{
		RightWrongQuizState NewState = AllocateState(new Callback(RightWrongQuizUISelect), new RightWrongQuizState()) as RightWrongQuizState;
		RightWrongQuiz quiz = new RightWrongQuiz();
		quiz.correctImage = sourceQuiz.correctImage;
		quiz.incorrectImage = sourceQuiz.incorrectImage;
		quiz.question = sourceQuiz.question;
		quiz.type = sourceQuiz.type;
		quiz.isLastQuestion = sourceQuiz.isLastQuestion;
//		quiz.isQuestionnaire = sourceQuiz.isQuestionnaire;
//		quiz.isPreQuestionnaire = sourceQuiz.isPreQuestionnaire;
		NewState.quiz = quiz;	
	}
	
	
	protected virtual void RightWrongQuizUISelect(eMode Mode, TMMessageNode Message = null) {

		RightWrongQuiz quiz = ((RightWrongQuizState)m_CurrentState).quiz;

		if (Mode == eMode.Enter)
		{	
			//show quiz UI
			if (OurCamera.Instance != null) OurCamera.Instance.BlurOn();
			UIQuizRightWrong.instance.Show(quiz);    
		}
	}


	public virtual void RightWrongQuizAnswer(Texture2D answer) {

		RightWrongQuiz quiz = ((RightWrongQuizState)m_CurrentState).quiz;
		
		if (quiz.Answer(answer)) {
			//correct
			if (OurCamera.Instance != null) OurCamera.Instance.BlurOff();
			UIQuizRightWrong.instance.Hide();
	
			GotoNextState();
		}
		else {
			//mark wrong answer here
			UIQuizRightWrong.instance.MarkWrongAnswer(answer);
		}
		AnalyticsRecorder.Instance.LogRightWrongQuizAnswer(quiz, answer.name);
	}



	protected virtual void CreateCameraAnimationWithTime(string triggerName, float time) {
		
		CameraAnimatorState NewState = AllocateState(new Callback(CameraAnimationWithTimeHandler), new CameraAnimatorState()) as CameraAnimatorState;
		NewState.triggerName = triggerName;
		NewState.m_Timer = time;
	}
	


	protected virtual void CameraAnimationWithTimeHandler(eMode Mode, TMMessageNode Message = null) 
	{
		Debug.Log("parent " + Mode.ToString());
		if (Mode == eMode.Enter)
		{

		}
		else if (Mode == eMode.Running) {
			Debug.Log(m_CurrentState.m_Timer);
			m_CurrentState.m_Timer -= Time.deltaTime;
			if (m_CurrentState.m_Timer < 0) {
				GotoNextState();
			}
		}
		
	}






	//AVATAR WORKAROUND, THIS NEED TO BE REMOVED AS SOON AS WE HAVE FINAL RIG
	
	public void DirtyAvatarOn() {

		if (!tempAvatar.gameObject.activeSelf) {
			Debug.Log("dirty avatar on");
			tempAvatar.gameObject.SetActive(true);
			tempAvatar.CopyOtheAvatar(m_Avatar);

			tempAvatar.m_FaceShield.SetActive(true);
			m_Avatar.gameObject.SetActive(false);
			m_Avatar = tempAvatar;
		}
	}
	
	public void DirtyAvatarOff() {
		
		if (tempAvatar.gameObject.activeSelf) {
			Debug.Log("dirty avatar off");
			oldAvatar.gameObject.SetActive(true);
			oldAvatar.CopyOtheAvatar(tempAvatar);
			tempAvatar.gameObject.SetActive(false);
			m_Avatar = oldAvatar;
		}
		
	}

	void EnableTemp() {
		Debug.Log("enable");
		tempAvatar.gameObject.SetActive(true);
	}







}
