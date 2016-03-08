using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMSupport;

public class TitleScene : Scene
{
	public Animator cameraAnimator;
	public UIPostersMenu uiPostersMenu;
	public UIQuestionnaireClipboard questionnaireClipboard;
	public ViewWelcome viewWelcome;


	public bool skipToTheRoom = false;
	static bool	sHavePlayedBuddyIntro = false;

	public List<RightWrongQuiz> questionnaireQuestions = new List<RightWrongQuiz>();

	void OnDestroy()
	{
		TMMessenger.Instance.RemoveFromMessageQueue(this);
	
	}



	public IEnumerator StartTitleScene()
	{
		m_States = new List<State>();

		TMMessenger.Instance.AddToMessageQueue(new MessageCallback(this.MessageCallback));
		GameManager.Instance.Buddy.WearScrubs();


		yield return new WaitForEndOfFrame();


		if (!skipToTheRoom) {
			if (string.IsNullOrEmpty(UserProfile.sCurrent.Name)) {
				//states for initial animation
				CreateCameraAnimationWithTime("walkToTable", 8f);
				CreateCallback(new Callback(DoNothing));
				uiPostersMenu.SetPostersActive(false);
				CreateCameraAnimationWithTime("walkIn", 9f);
			}
			else {
				//jump straight to posters if user is loaded already
				cameraAnimator.SetBool("jumpToPosters", true);
				uiPostersMenu.SetPostersActive(true);

			}
		}
		else {
			//skipping to the room
//			UserProfile.Load("Admin");
			cameraAnimator.SetBool("jumpToPosters", true);
			uiPostersMenu.SetPostersActive(true);
		}


		if (sHavePlayedBuddyIntro == false)
		{
			CreateBuddyConversationUI("0.01", "BuddyDialogClosed");
#if UNITY_ANDROID
			CreateUIExplination("0.07", uiPostersMenu.posters[0].gameObject, "Videos/Title/DonningBasic_quarterRes.ogv");
#endif
#if UNITY_STANDALONE
			CreateUIExplination("0.07", uiPostersMenu.posters[0].gameObject, "Videos/Title/DonningBasic.ogv");
#endif
#if UNITY_ANDROID
			CreateUIExplination("0.09", uiPostersMenu.posters[2].gameObject, "Videos/Title/DoffingBasic_quarterRes.ogv");
#endif
#if UNITY_STANDALONE
			CreateUIExplination("0.09", uiPostersMenu.posters[2].gameObject, "Videos/Title/DoffingBasic.ogv");
#endif
#if UNITY_ANDROID
			CreateUIExplination("0.08", uiPostersMenu.posters[1].gameObject, "Videos/Title/Hazards_quarterRes.ogv");
#endif
#if UNITY_STANDALONE
			CreateUIExplination("0.08", uiPostersMenu.posters[1].gameObject, "Videos/Title/Hazards.ogv");
#endif

//			CreateUIExplination("0.09", uiPostersMenu.posters[2].gameObject, "Videos/Title/DoffingBasic.ogv");
//			CreateUIExplination("0.08", uiPostersMenu.posters[1].gameObject, "Videos/Title/Hazards.ogv");

//			CreateUIExplination("0.10", UITitle.Instance.m_Sections[3]);

			//questionnaire
			if (!skipToTheRoom) AddQuestionnaireStages(QuizType.pre);
			CreateCallback(new Callback(BuddyIntroPlayed));

		}

	                                                     
//		CreateCallback(new Callback(SaveAnalytics));
		CreateCallback(new Callback(DoNothing));
		GotoState(m_States[0]);
	}



	protected void MessageCallback(TMMessageNode Message)
	{
		if (Message.Message == "NEW_USER_COMPLETE".GetHashCode() || Message.Message == "LOAD_USER_COMPLETE".GetHashCode())
		{
			GotoNextState();
		}
		else if (m_CurrentState.m_UIWaitIdent != null && Message.Message == m_CurrentState.m_UIWaitIdent.GetHashCode())
		{
			m_CurrentState.m_Handler(eMode.Exit, Message);
			return;
		}
		else
			m_CurrentState.m_Handler(eMode.UIMessage, Message);
	}




	protected override void CameraAnimationWithTimeHandler(eMode Mode, TMMessageNode Message = null) 
	{
		if (Mode == eMode.Enter)
		{
			cameraAnimator.SetTrigger(((CameraAnimatorState)m_CurrentState).triggerName);
		}
		else if (Mode == eMode.Running) {
			m_CurrentState.m_Timer -= Time.deltaTime;
			if (m_CurrentState.m_Timer < 0) {
				GotoNextState();
			}
		}
	}



	#region Questionnaire
	
	public void AddQuestionnaireStages(QuizType quizType) {

		List<RightWrongQuiz> preQuestionnaiteQuestions = new List<RightWrongQuiz>(questionnaireQuestions);

		foreach (RightWrongQuiz quiz in preQuestionnaiteQuestions) {
			quiz.type = quizType;
			quiz.isLastQuestion = (preQuestionnaiteQuestions.IndexOf(quiz)) == preQuestionnaiteQuestions.Count-1;
			CreateRightWrongQuizSelection(quiz);
		}
		//force saving when post questionnaire, this need removing after per satge saving in
		if (quizType == QuizType.post) CreateCallback(new Callback(SaveAnalytics));
	}




	#endregion


	protected override void RightWrongQuizUISelect(eMode Mode, TMMessageNode Message = null) {
		
		RightWrongQuiz quiz = ((RightWrongQuizState)m_CurrentState).quiz;
		
		if (Mode == eMode.Enter)
		{	
			//show quiz UI and clipboard
			if (!questionnaireClipboard.isClipboardVisible) questionnaireClipboard.Show(true);
			UIQuizRightWrong.instance.Show(quiz);    
		}
	}



	public override void RightWrongQuizAnswer(Texture2D answer) {
		
		RightWrongQuiz quiz = ((RightWrongQuizState)m_CurrentState).quiz;
		
		if (quiz.Answer(answer)) {
			//hide clipboard is not last question
			if (quiz.isLastQuestion) {
				questionnaireClipboard.Hide(true);

				//post user data?
				AnalyticsData.Instance.PostAllUserData();
			}
		}
	

		base.RightWrongQuizAnswer(answer);
	}




	////////////////////////////////////////////////////////////////////////////////
	// Highlight new user
	protected void CreateUIExplination(string BuddySpeech, GameObject UIHighlight, string videoPath = "")
	{
		ObjectState		NewState = AllocateState(new Callback(HighlightUI), new ObjectState()) as ObjectState;

		NewState.m_MediaIdent = BuddySpeech;
		NewState.m_GameObject = UIHighlight;
		NewState.videoPath = videoPath;

	}

////////////////////////////////////////////////////////////////////////////////
// Highlight new user
	void HighlightUI(eMode Mode, TMMessageNode Message = null)
	{
		ObjectState ThisState = m_CurrentState as ObjectState;

		if (Mode == eMode.Enter)
		{

			//highlight poster
			if (ThisState.m_GameObject.GetComponent<UIHighlightable>() != null) {
				UIHighlightManager.instance.HighlightObject(ThisState.m_GameObject.GetComponent<UIHighlightable>());
			}


			//play video if needed
			if (!string.IsNullOrEmpty(ThisState.videoPath)) {
				((ViewTitleScene)ViewTitleScene.instance).LoadVideo(ThisState.videoPath);
			}


			GameManager.Instance.Buddy.TriggerConversationById(ThisState.m_MediaIdent, true);
		}
		else if (Mode == eMode.UIMessage)
		{
			if (Message.Message == "BuddyDialogClosed".GetHashCode())
			{
				//unhighlight poster
				UIHighlightManager.instance.UnhighlightAll();
				//stop video
				if (!string.IsNullOrEmpty(ThisState.videoPath)) {
					((ViewTitleScene)ViewTitleScene.instance).StopVideo();
				}

				GotoNextState();
			}
		}


	}

////////////////////////////////////////////////////////////////////////////////
// When buddy has finished talking
	void BuddyIntroPlayed(eMode Mode, TMMessageNode Message = null)
	{
		uiPostersMenu.SetPostersActive(true);

		//highlight all posters
		List<UIHighlightable> highlightablePosters = new List<UIHighlightable>();
		highlightablePosters.Add(uiPostersMenu.posters[0].GetComponent<UIHighlightable>());
		highlightablePosters.Add(uiPostersMenu.posters[1].GetComponent<UIHighlightable>());
		highlightablePosters.Add(uiPostersMenu.posters[2].GetComponent<UIHighlightable>());
		UIHighlightManager.instance.HighlightObjects(highlightablePosters);

//		UITitle.Instance.Activate();
		sHavePlayedBuddyIntro = true;
		GotoNextState();
	}

////////////////////////////////////////////////////////////////////////////////
// Do nothing
	void DoNothing(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter) {

		}
	}

	void Update()
	{
		if (m_States != null)
		{
			if (m_CurrentState != null) m_CurrentState.m_Handler(eMode.Running);
		}
	}


	void SaveAnalytics(eMode Mode, TMMessageNode Message = null)
	{
		if (Mode == eMode.Enter) {
			Debug.Log("Save");
			AnalyticsRecorder.Instance.StopSession();
		}
	}



}
