using System.Collections;
using UnityEngine;
using TMSupport;

public class Hazard_FeelingFeint : Hazard
{
	public Avatar		m_Avatar;
	public Question question1Prefab;
	public Question question2Prefab;
	public Question question;
	public UIQuestion uiQuestion;
//	public UIDialog uiDialog;
	public Scene2 sceneManager;
	public Animator doorAnimator;
	private Avatar avatar;
	public bool isActive = false;

	public Mode currentMode;
	public enum Mode {
		inActive,
		start,
		question,
		hesitation,
		leaving
	}


	void Awake() {
		TMMessenger.Instance.AddToMessageQueue(new MessageCallback(this.MessageCallback));
	}


	override protected void Init()
	{
		base.Init();
		m_HazardType = eType.FeelingFeint;
		m_EventTime = 6.6f;
		m_TimeBeforeClickIsGood = 0.2f;
		m_AnimPlayedOnAvatar = true;
//		HazardManager.Instance.Register(this);
		question = Instantiate(question1Prefab);
		enabled = false;
		isActive = false;
	}

	public override void Activate()
	{
		//STARTING 
//		UIManager.Instance.BringUpDialog(UIManager.eUIReferenceName.BuddyDialog, "Some text here about the fainting");
		isActive = true;
		currentMode = Mode.start;

		OurCamera.Instance.BlurOff();
		sceneManager.DirtyAvatarOn();
		sceneManager.m_Avatar.m_Top.SetActive(true);
		sceneManager.m_Avatar.m_Bottom.SetActive(true);
		sceneManager.m_Avatar.m_FaceShield.SetActive(true);
		
		avatar = sceneManager.m_Avatar;
		avatar.OnAnimationEventReceived += OnAnimationEventReceived;
		
		
		
		base.Activate();
		enabled = true;
		m_Avatar = sceneManager.m_Avatar;
		m_Avatar.SetAnimationTrigger("FaintingStart");
		Invoke("ShowQuestionUI", 7.5f);

	}



	void OnEnable() {
		UIQuestion.OnAnswerGiven += OnAnswerGiven;
	}

	void OnDisable() {
		UIQuestion.OnAnswerGiven -= OnAnswerGiven;
	}


	void ShowQuestionUI() {
		uiQuestion.Show(question);
		currentMode = Mode.question;
	}



	void MessageCallback(TMMessageNode Message)
	{
		if(!isActive) return;
		
		if (Message.Message == "BuddyDialogClosed".GetHashCode())
		{
			if (currentMode == Mode.start) {
//				OurCamera.Instance.MoveTo(OurCamera.eCameraFocus.Avatar, 1f);

			}
		}
	}



	void OnAnswerGiven(Question question, string answer) {

		if (question.question == question1Prefab.question) {

			Debug.Log("question 1 answer");

			//first question leave or help take off equipment
			if (this.question.Answer(answer)) {

				uiQuestion.Hide();
				UIManager.Instance.CloseUI(UIManager.eUIReferenceName.BuddyDialog);

				m_Avatar.SetAnimationTrigger("Fainting_No");

				//ask second question
				uiQuestion.Hide();
				Invoke("AskSecondQuestion", 8.2f);
				currentMode = Mode.hesitation;
			}
			else {
				uiQuestion.RefreshButtons();
				ViewScene.instance.buddyDialog.SetText("This is incorrect answer, we need some proper script here");
//				UIManager.Instance.BringUpDialog(UIManager.eUIReferenceName.BuddyDialog, "This is incorrect answer, we need some proper script here");
			}
		}

		else if (question.question == question2Prefab.question) {

			Debug.Log("question 2 answer");

			//second question, leve or stay
			if (this.question.Answer(answer)) {

				uiQuestion.Hide();
				Leaving();
				Invoke("FinishedFainting", 13f);
				currentMode = Mode.leaving;
			}
			else {
				//wrong answer
				uiQuestion.RefreshButtons();
				ViewScene.instance.buddyDialog.SetText("No we need to keep safe and keep serving, you need to leave");
			}
		}
	}



	//triggered by the animation on Avatar
	void OnAnimationEventReceived (string eventName)
	{
		Debug.Log("Animation event " + eventName); 

		if (eventName == "FaintingOpenDoor") {
			doorAnimator.SetTrigger("open");
		}
	}







	void AskSecondQuestion() {
		question = Instantiate(question2Prefab);
		uiQuestion.Show(question);
	
	}



	void Leaving() {
		m_Avatar.SetAnimationTrigger("Fainting_Yes");
	}


	void FinishedFainting() {
		isActive = false;
		OurCamera.Instance.MoveTo(OurCamera.eCameraFocus.Buddy, 1f);
		doorAnimator.SetTrigger("reset");
		StartCoroutine(Finish());

	}


	IEnumerator Finish() {
		yield return new WaitForSeconds(1.5f);
		HazardManager.Instance.MoveToNextHazard();
		HazardManager.Instance.TriggerNextHazzard();
		avatar.OnAnimationEventReceived -= OnAnimationEventReceived;
	}

}







