using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMSupport;

public class Buddy : MonoBehaviour
{
	public Sprite		m_UIMainImage;
	public GameObject	m_Mesh;
	public LayerMask	m_LayerMask;

	[Header("Equipment")]
	public GameObject	m_FaceMask;
	public GameObject	m_Gloves;
	public GameObject	m_Gown;
	public GameObject	m_Hood;
	public GameObject	m_FaceShield;
	public GameObject	m_Goggles;
	public GameObject	m_Apron;

	[Header("BodyParts")]
	public GameObject	m_Top;
	public GameObject	m_Bottom;
	public GameObject	m_Hair;

	string				m_CurrentAudioFilename;

	Animator			m_Animator;
	bool				m_WaitingForAudioToActivateClose;
	BuddyDialog			m_ActiveBuddyDialog;
	string				m_ActiveMediaName;
	bool				m_AudioPlaying;
	bool				m_ContinuedNodes;

	MediaClipManager.AudioFinished		m_Callback;

	void Awake()
	{
		m_Animator = GetComponent<Animator>();
		m_WaitingForAudioToActivateClose = false;
		m_ActiveBuddyDialog = null;
		m_AudioPlaying = false;
		m_Callback = new MediaClipManager.AudioFinished(AudioFinished);

//		Conversation convo = new Conversation();
//		Debug.Log(MediaNodeManager.GetConversation(eLanguage.English, "0.01", ref convo));

	}

	public void WearScrubs()
	{
		m_FaceMask.SetActive(false);
		m_Gloves.SetActive(false);
		m_Gown.SetActive(false);
		m_Hood.SetActive(false);
		m_FaceShield.SetActive(false);
		m_Goggles.SetActive(false);
		m_Apron.SetActive(false);

		m_Top.SetActive(true);
		m_Bottom.SetActive(true);
		m_Hair.SetActive(true);
	}

	public void WearPPE()
	{
		m_FaceMask.SetActive(true);
		m_Gloves.SetActive(true);
		m_Gown.SetActive(true);
		m_Hood.SetActive(true);
		m_FaceShield.SetActive(true);
		m_Goggles.SetActive(false);
		m_Apron.SetActive(true);

		m_Top.SetActive(false);
		m_Bottom.SetActive(true);
		m_Hair.SetActive(false);
	}



	internal void TriggerConversation(string Ident, bool UseCloseButton = true, UIManager.eUIReferenceName UIReferenceName = UIManager.eUIReferenceName.BuddyDialog)
	{
		string		Text = "";

		m_ContinuedNodes = MediaNodeManager.GotContinuation(Ident);
		if (MediaNodeManager.GetTextAndAudio(Ident, ref Text, ref m_CurrentAudioFilename) == true)
		{
			Debug.Log(Text);
			m_ActiveMediaName = Ident;
			m_ActiveBuddyDialog = ViewScene.instance.buddyDialog;
			m_ActiveBuddyDialog.SetText(Text);
			m_WaitingForAudioToActivateClose = UseCloseButton;
			m_AudioPlaying = true;

			if(UseCloseButton && UserProfile.sCurrent.HasBuddySpoken(m_ActiveMediaName))
				m_ActiveBuddyDialog.SetCloseState(BuddyDialog.eClose.Active);	// .DisableCloseButton(true);
			else
				m_ActiveBuddyDialog.SetCloseState(BuddyDialog.eClose.Inactive);

			m_Animator.SetTrigger("Talking");
			m_Animator.speed = 0.6f;

			if(MediaClipManager.Instance.Play(m_CurrentAudioFilename, m_Callback) == false)
			{
				float		FakeDialogLength = 0.05f * Text.Length;

				CancelInvoke();
				Invoke("AudioFinished", FakeDialogLength);
			}
		}
	}


	internal void TriggerConversationById(string conversationId, bool UseCloseButton = true, UIManager.eUIReferenceName UIReferenceName = UIManager.eUIReferenceName.BuddyDialog)
	{
		Conversation conversation = new Conversation();

//		m_ContinuedNodes = MediaNodeManager.GotContinuation(Ident);
		if (MediaNodeManager.GetConversation(MediaNodeManager.Language, conversationId, ref conversation))
		{
			m_ActiveMediaName = conversation.name;
			m_ActiveBuddyDialog = ViewScene.instance.buddyDialog;
			m_ActiveBuddyDialog.SetText(conversation.script);
			m_WaitingForAudioToActivateClose = UseCloseButton;
			m_AudioPlaying = true;
			
			if(UseCloseButton && UserProfile.sCurrent.HasBuddySpoken(m_ActiveMediaName)) m_ActiveBuddyDialog.SetCloseState(BuddyDialog.eClose.Active);	// .DisableCloseButton(true);
			else m_ActiveBuddyDialog.SetCloseState(BuddyDialog.eClose.Inactive);

			//This need refactoring at some point, really should't be here
			m_Animator.SetTrigger("Talking");
			m_Animator.speed = 0.6f;


			if(MediaClipManager.Instance.Play(conversation.audioClip, m_Callback) == false)
			{
				float FakeDialogLength = 0.05f * conversation.script.Length;
				
				CancelInvoke();
				Invoke("AudioFinished", FakeDialogLength);
			}
		}
	}







	void AudioFinished()
	{
		m_AudioPlaying = false;
		if (m_WaitingForAudioToActivateClose || m_ContinuedNodes)
		{
			// Remember that buddy has now said this
			UserProfile.sCurrent.BuddyHasSpoken(m_ActiveMediaName);

			if(m_ContinuedNodes)
				TriggerNextDialog();
			else
			{
				// Highlight the close button
				m_ActiveBuddyDialog.SetCloseState(BuddyDialog.eClose.Highlighted);

				m_WaitingForAudioToActivateClose = false;
				m_Animator.SetTrigger("StopTalking");
				m_Animator.speed = 1;
			}
		}
		TMMessenger.Send("BuddyDialogFinished".GetHashCode());
	}



	private void TriggerNextDialog()
	{
		string		Ident = MediaNodeManager.GotContinuationIdent(m_ActiveMediaName);
		string		Text = "";

		if(MediaNodeManager.GetTextAndAudio(Ident, ref Text, ref m_CurrentAudioFilename) == true)
		{
			m_ActiveBuddyDialog.SetText(Text);

			m_ActiveMediaName = Ident;
			m_AudioPlaying = true;

			if (MediaClipManager.Instance.Play(m_CurrentAudioFilename, m_Callback) == false)
			{
				float	FakeDialogLength = 0.05f * Text.Length;

				CancelInvoke();
				Invoke("AudioFinished", FakeDialogLength);
			}

			m_ContinuedNodes = MediaNodeManager.GotContinuation(Ident);
		}
	}


	internal void IconClicked()
	{
		MediaClipManager.Instance.Play(m_CurrentAudioFilename, m_Callback);
		m_Animator.SetTrigger("Talking");
		m_AudioPlaying = true;
	}

	internal bool CloseClicked(BuddyDialog.eClose CloseState)
	{
		if(CloseState == BuddyDialog.eClose.Active)
		{
			UserProfile.sCurrent.SkippedBuddy();
		}

		if (m_AudioPlaying)
		{
			MediaClipManager.Instance.StopAudio();
			TMMessenger.Send("BuddyDialogFinished".GetHashCode());

			if (m_ContinuedNodes)
				TriggerNextDialog();
			else
			{
				m_Animator.SetTrigger("StopTalking");
				m_Animator.speed = 1;

				m_WaitingForAudioToActivateClose = false;
				m_ActiveMediaName = null;
				m_ActiveBuddyDialog = null;
			}
		}
		return(true);
	}


}
