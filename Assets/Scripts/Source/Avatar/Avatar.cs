using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Avatar : MonoBehaviour
{
	public delegate void OnAnimationEventReceivedHandler(string eventName);
	public event OnAnimationEventReceivedHandler OnAnimationEventReceived;

	[Header("Animator")]
	public RuntimeAnimatorController	m_DonningController;
	public RuntimeAnimatorController	m_RiskHazardController;
	public RuntimeAnimatorController	m_DoffingController;

	[Header("Equipment")]
	public GameObject	m_Jewelry;
	public GameObject	m_FaceMask;
	public GameObject	m_Gloves;
	public GameObject	m_Gown;
	public GameObject	m_Hood;
	public GameObject	m_FaceShield;
	public GameObject	m_Goggles;
	public GameObject	m_DoubleGloves;
	public GameObject	m_Apron;
	public GameObject	m_Shoes;
	public GameObject	m_Boots;
	public GameObject	m_Suit;

	[Header("BodyParts")]
	public GameObject	m_Top;
	public GameObject	m_Bottom;
	public GameObject	m_Hair;

	[Header("AnimatedEquipment")]
	public GameObject	m_AnimFaceShield;
	public GameObject	m_AnimGoggles;
	public GameObject	m_AnimFaceMask;
	public GameObject	m_AnimApron;
	public GameObject	m_AnimHood;

	[Header("DisposeEquipment")]
	public GameObject	m_DisposeGown;
	public GameObject	m_DisposeApron;

	public bool isTemporaryAvatar = false;



	[Header("UI")]
	public GameObject	m_NameObject;
	Button				m_NameButton;
	Text				m_NameText;
	Canvas				m_NameCanvas;
	Image				m_NameImage;

	public enum eClothing
	{
		Jewelry = 0,
		FaceMask,
		InnerGloves,
		Gown,
		Hood,
		FaceShield,
		OuterGloves,
		Apron,
		MAX
	}

	Animator			m_Animator;

	enum eDispose
	{
		Bin = 0,
		Bucket,
		None
	}

	class EquipmentNode
	{
		public string				m_DonningName;
		public float				m_DonningDuration;
		public List<GameObject>		m_ThingsToSwitchOffWhenDonning;

		public string				m_DoffingName;
		public float				m_DoffingDuration;
		public List<GameObject>		m_ThingsToSwitchOnWhenDoffing;

		public List<GameObject>		m_ObjectsToObscure;

		public GameObject			m_SwapObject;
		public GameObject			m_BodyObject;
		public GameObject			m_DisposeObject;
		public eDispose				m_DisposeType;
	}
	EquipmentNode[]				m_Equipment;

	GameObject[]				m_EbolaDisplays;
	bool						m_AllowEbolaDisplay;

	EquipmentManager.eEquipment m_CurrentDoffingEquipment;

	void Awake()
	{


		FindNameLabel();

		Scene		TheScene = GameObject.FindObjectOfType<Scene>();

		if (TheScene != null) TheScene.SetAvatar(this);
		m_Animator = GetComponent<Animator>();
		m_AllowEbolaDisplay = false;
		m_CurrentDoffingEquipment = EquipmentManager.eEquipment.MAX;

		if(TheScene is Scene1)
			m_Animator.runtimeAnimatorController = m_DonningController;
		else if (TheScene is Scene2)
			m_Animator.runtimeAnimatorController = m_RiskHazardController;
		else if (TheScene is Scene3)
		{
			if (!isTemporaryAvatar) m_Animator.runtimeAnimatorController = m_DoffingController;
			m_AllowEbolaDisplay = true;
		}

		SetupEquipmentData();

		m_EbolaDisplays = GameObject.FindGameObjectsWithTag("Ebola");
		EbolaDisplayDisable();
	}



	void FindNameLabel()
	{
		if( m_NameObject == null )
		{
			Debug.Log("No name object is defined for this avatar. This item must point to a Canvas with a button OR we are in the test scene?", this);
			return;
		}
		else {
			//canvas object have been found

			m_NameCanvas = m_NameObject.GetComponent<Canvas>();
			m_NameImage  = m_NameObject.GetComponentInChildren<Image>();
			m_NameButton = m_NameObject.GetComponentInChildren<Button>();
			m_NameText   = m_NameObject.GetComponentInChildren<Text>();

			if( m_NameCanvas == null || m_NameImage == null || m_NameButton == null || m_NameText == null )
			{
//				Debug.LogError("Invalid name object has been defined for this avatar. This item must point to a Canvas with a button, and image and a text item", this);
			}
		}
	}

	public void EbolaDisplayEnable()
	{
		if (m_AllowEbolaDisplay)
		{
			for (int loop = 0; loop < m_EbolaDisplays.Length; loop++)
			{
				m_EbolaDisplays[loop].SetActive(true);
			}
		}
	}
	public void EbolaDisplayDisable()
	{
		for (int loop = 0; loop < m_EbolaDisplays.Length; loop++)
		{
			m_EbolaDisplays[loop].SetActive(false);
		}
	}

	public void SetAnimationTrigger(string AnimationName)
	{
		//fainting animation is using the question, thats why we are using boo; instead of trigger
		if (AnimationName == "Dizzy") {
			m_Animator.SetBool("Dizzy", true);
		}
		else {
			m_Animator.SetTrigger(AnimationName);
		}
	}

	private void SetupEquipmentData()
	{
//		Debug.Log("Setup equipment data");
		m_Equipment = new EquipmentNode[(int)EquipmentManager.eEquipment.MAX];
		for(int loop = 0; loop < m_Equipment.Length; loop++)
		{
			m_Equipment[loop] = new EquipmentNode();
			m_Equipment[loop].m_DonningName = null;
			m_Equipment[loop].m_DonningDuration = -1;
			m_Equipment[loop].m_ThingsToSwitchOffWhenDonning = new List<GameObject>();

			m_Equipment[loop].m_DoffingName = null;
			m_Equipment[loop].m_DoffingDuration = -1;
			m_Equipment[loop].m_ThingsToSwitchOnWhenDoffing = new List<GameObject>();

			m_Equipment[loop].m_ObjectsToObscure = new List<GameObject>();

			m_Equipment[loop].m_SwapObject = null;
			m_Equipment[loop].m_BodyObject = null;
			m_Equipment[loop].m_DisposeObject = null;
		}

		//						Equipment									Body object			Anim object
		SetEquipmentGameObjects(EquipmentManager.eEquipment.Preparation,	m_Jewelry,			null);
		SetEquipmentGameObjects(EquipmentManager.eEquipment.FaceMask,		m_FaceMask,			m_AnimFaceMask);
		if (m_AnimFaceMask != null) SetEquipmentGameObjects(EquipmentManager.eEquipment.InnerGloves,	m_Gloves,			null);
		SetEquipmentGameObjects(EquipmentManager.eEquipment.Gown,			m_Gown,				null);
		if (m_AnimHood != null) SetEquipmentGameObjects(EquipmentManager.eEquipment.Hood,			m_Hood,				m_AnimHood);
		if (m_AnimFaceShield != null) SetEquipmentGameObjects(EquipmentManager.eEquipment.FaceShield,		m_FaceShield,		m_AnimFaceShield);
		if (m_Goggles != null && m_AnimGoggles != null) SetEquipmentGameObjects(EquipmentManager.eEquipment.Goggles,		m_Goggles,			m_AnimGoggles);
		SetEquipmentGameObjects(EquipmentManager.eEquipment.OuterGloves,	m_DoubleGloves,		null);
		if (m_Apron != null) SetEquipmentGameObjects(EquipmentManager.eEquipment.ReusableApron,	m_Apron,			m_AnimApron);
		SetEquipmentGameObjects(EquipmentManager.eEquipment.Boots,			m_Boots,			null);
		SetEquipmentGameObjects(EquipmentManager.eEquipment.BootsRemoval,	m_Boots,			null);
		SetEquipmentGameObjects(EquipmentManager.eEquipment.ClosedToeShoes, m_Shoes,			null);
		if (m_Suit != null) SetEquipmentGameObjects(EquipmentManager.eEquipment.Suit,			m_Suit,				null);

		if (m_DisposeApron != null) m_DisposeApron.SetActive( false );	// TODO : Mike Kaiser : This item seems to have been forgotten about. Should be present in one of these lists?

		//					  Equipment										Donning animation		Animtime
		SetDonningInformation(EquipmentManager.eEquipment.Preparation,		"RemoveJewellery",		4);
		SetDonningInformation(EquipmentManager.eEquipment.FaceMask,			"PutOnFaceMask",		4);
		SetDonningInformation(EquipmentManager.eEquipment.InnerGloves,		"PutOnInnerGloves",		4);
		SetDonningInformation(EquipmentManager.eEquipment.Gown,				"PutOnGown",			4);
		SetDonningInformation(EquipmentManager.eEquipment.Hood,				"PutOnHood",			4);
		SetDonningInformation(EquipmentManager.eEquipment.FaceShield,		"PutOnFaceShield",		5);
		SetDonningInformation(EquipmentManager.eEquipment.Goggles,			"PutOnGoggles",			4);
		SetDonningInformation(EquipmentManager.eEquipment.OuterGloves,		"PutOnOuterGloves",		4);
		SetDonningInformation(EquipmentManager.eEquipment.ReusableApron,	"PutOnApron",			4);

		//					  Equipment										Doffing animation		Animtime	Disposal			Disposal model
		if (m_AnimFaceMask != null) SetDoffingInformation(EquipmentManager.eEquipment.FaceMask,			"TakeOffFaceMask",		4,			eDispose.Bin,		m_AnimFaceMask);
		if (m_AnimHood != null) SetDoffingInformation(EquipmentManager.eEquipment.Hood,				"TakeOffHood",			4,			eDispose.Bucket,	m_AnimHood);
//		SetDoffingInformation(EquipmentManager.eEquipment.FaceShield,		"ShieldDoff1", 1.5,			eDispose.Bin,		m_AnimFaceShield);
//		SetDoffingInformation(EquipmentManager.eEquipment.FaceShield,		"ShieldDoff1",	5,			eDispose.Bin,		m_AnimFaceShield);
		if (m_AnimGoggles != null) SetDoffingInformation(EquipmentManager.eEquipment.Goggles,			"TakeOffGoggles",		4,			eDispose.Bucket,	m_AnimGoggles);
		SetDoffingInformation(EquipmentManager.eEquipment.ReusableApron,	"TakeOffApron",			18,			eDispose.Bucket,	m_AnimApron);
		if (m_Gown != null) SetDoffingInformation(EquipmentManager.eEquipment.Gown,				null,					4,			eDispose.Bin,		m_DisposeGown);
		SetDoffingInformation(EquipmentManager.eEquipment.Boots,			"InspectBoots",			4,			eDispose.None,		null);
		SetDoffingInformation(EquipmentManager.eEquipment.BootsRemoval,		"WashBoots",		    9f,			eDispose.None,		null);

		AddToDonningDisableList(EquipmentManager.eEquipment.Gown,			m_Top);
		if (m_Hair != null) AddToDonningDisableList(EquipmentManager.eEquipment.Hood,			m_Hair);

		AddToDonningDisableList(EquipmentManager.eEquipment.OuterGloves,	m_Gloves);
		AddToDoffingEnableList(EquipmentManager.eEquipment.OuterGloves,		m_Gloves);

		SetObscureList(EquipmentManager.eEquipment.Gown,					m_Top, m_Bottom);
		if (m_Hair != null) SetObscureList(EquipmentManager.eEquipment.Hood,					m_Hair);
		SetObscureList(EquipmentManager.eEquipment.OuterGloves,				m_Gloves);

//		if(m_AnimClipboard != null)
//			m_AnimClipboard.SetActive(false);
	}

	void SetObscureList(EquipmentManager.eEquipment Equipment, params GameObject[] ObscureObjects)
	{
		EquipmentNode	ThisEquipment = m_Equipment[(int)Equipment];

		for (int loop = 0; loop < ObscureObjects.Length; loop++)
		{
			ThisEquipment.m_ObjectsToObscure.Add(ObscureObjects[loop]);
		}
	}

	void AddToDonningDisableList(EquipmentManager.eEquipment Equipment, params GameObject[] DisableObjects)
	{
		EquipmentNode	ThisEquipment = m_Equipment[(int)Equipment];

		for (int loop = 0; loop < DisableObjects.Length; loop++)
		{
			ThisEquipment.m_ThingsToSwitchOffWhenDonning.Add(DisableObjects[loop]);
		}
	}

	void AddToDoffingEnableList(EquipmentManager.eEquipment Equipment, params GameObject[] EnableObjects)
	{
		EquipmentNode	ThisEquipment = m_Equipment[(int)Equipment];

		for (int loop = 0; loop < EnableObjects.Length; loop++)
		{
			ThisEquipment.m_ThingsToSwitchOnWhenDoffing.Add(EnableObjects[loop]);
		}
	}

	void SetDoffingInformation(EquipmentManager.eEquipment Equipment, string AnimationName, float DoffingDuration, eDispose DisposalType, GameObject DisposalModel)
	{
		EquipmentNode		ThisEquipment = m_Equipment[(int)Equipment];

		ThisEquipment.m_DoffingName = AnimationName;
		ThisEquipment.m_DoffingDuration = DoffingDuration;
		ThisEquipment.m_DisposeType = DisposalType;
		if(DisposalModel != null)
		{
			ThisEquipment.m_DisposeObject = DisposalModel;
			DisposalModel.SetActive(false);
		}
	}

	void SetEquipmentGameObjects(EquipmentManager.eEquipment Equipment, GameObject ClothingGameOject, GameObject AnimGameOject = null, bool startsEnabled = false)
	{
		m_Equipment[(int)Equipment].m_BodyObject = ClothingGameOject;
		if(AnimGameOject != null)
		{
			m_Equipment[(int)Equipment].m_SwapObject = AnimGameOject;
			AnimGameOject.SetActive(false);
		}
	}

	void SetDonningInformation(EquipmentManager.eEquipment Equipment, string DonningName, float DonningDuration)
	{
		m_Equipment[(int)Equipment].m_DonningName = DonningName;
		m_Equipment[(int)Equipment].m_DonningDuration = DonningDuration;
	}

	internal void SetStreetClothes()
	{
		SetBaseClothing();
		m_Jewelry.SetActive(true);
	}

	internal void SetBaseClothing()
	{
		if( m_Equipment == null )	// Mike Kaiser : It looks like EquipmentManager can sometime call this function before our Awake() is called!
			Awake();

		for (int loop = 0; loop < m_Equipment.Length; loop++)
		{
			if (m_Equipment[loop].m_BodyObject != null)
				m_Equipment[loop].m_BodyObject.SetActive(false);
		}
		m_Top.SetActive(true);
		m_Bottom.SetActive(true);
		if (m_Hair != null) m_Hair.SetActive(true);
		m_Boots.SetActive(true);
		if (m_NameCanvas != null) m_NameCanvas.enabled = false;
		if (m_NameImage != null) m_NameImage.enabled = true;
	}

	internal void SetFullyProtected()
	{
		if( m_Equipment == null )	// Mike Kaiser : It looks like EquipmentManager can sometime call this function before our Awake() is called!
			Awake();

		for (int loop = 0; loop < m_Equipment.Length; loop++)
		{
			if (m_Equipment[loop].m_BodyObject != null)
				m_Equipment[loop].m_BodyObject.SetActive(true);
		}
		m_Jewelry.SetActive(false);
		m_Bottom.SetActive(false);
		m_Top.SetActive(false);
		if (m_Hair != null) m_Hair.SetActive(false);
		if (m_NameCanvas != null) m_NameCanvas.enabled = true;
	}

	internal void PutOn(EquipmentManager.eEquipment Equipment)
	{
		if(Equipment == EquipmentManager.eEquipment.Name)
		{
			Debug.Log("Player name is : " + UserProfile.sCurrent.Name);
			m_NameText.text = UserProfile.sCurrent.Name;
			m_NameImage.enabled = false;
			return;
		}

		EquipmentNode	ThisEquipment = m_Equipment[(int)Equipment];

		if (ThisEquipment.m_BodyObject != null)
		{
			ThisEquipment.m_BodyObject.SetActive(true);
			for (int loop = 0; loop < ThisEquipment.m_ObjectsToObscure.Count; loop++)
			{
				ThisEquipment.m_ObjectsToObscure[loop].SetActive(false);
			}
		}
	}

	internal void TakeOff(EquipmentManager.eEquipment Equipment)
	{
		Debug.Log("Take off : " + Equipment.ToString());

		if (Equipment == EquipmentManager.eEquipment.Name)
		{
			m_NameText.text = "";
			m_NameImage.enabled = false;
			return;
		}

		EquipmentNode	ThisEquipment = m_Equipment[(int)Equipment];

		if (ThisEquipment.m_BodyObject != null)
		{
			ThisEquipment.m_BodyObject.SetActive(false);
			for (int loop = 0; loop < ThisEquipment.m_ObjectsToObscure.Count; loop++)
			{
				ThisEquipment.m_ObjectsToObscure[loop].SetActive(true);
			}
		}
	}







	internal void DisplayName(bool Enable)
	{
		m_NameButton.gameObject.SetActive(Enable);
	}

	public void AnimationCallback(string Message)
	{        
		HazardManager.Instance.HandleMessage(Message);
	}

	public void TakeOffJewellery()
	{
		m_Jewelry.SetActive(false);
	}

	public void SwapFaceShield()
	{
		SwapEquipment(EquipmentManager.eEquipment.FaceShield);
	}
	public void SwapApron()
	{
		SwapEquipment(EquipmentManager.eEquipment.ReusableApron);
	}
	public void SwapHood()
	{
		SwapEquipment(EquipmentManager.eEquipment.Hood);
	}
	public void SwapGoggles()
	{
		SwapEquipment(EquipmentManager.eEquipment.Goggles);
	}
	public void SwapFaceMask()
	{
		SwapEquipment(EquipmentManager.eEquipment.FaceMask);
	}
	void SwapEquipment(EquipmentManager.eEquipment Equipment)
	{
		EquipmentNode	ThisEquipment = m_Equipment[(int)Equipment];

		ThisEquipment.m_SwapObject.SetActive(!ThisEquipment.m_SwapObject.activeInHierarchy);
		ThisEquipment.m_BodyObject.SetActive(!ThisEquipment.m_BodyObject.activeInHierarchy);

		for (int loop = 0; loop < ThisEquipment.m_ObjectsToObscure.Count; loop++)
		{
			ThisEquipment.m_ObjectsToObscure[loop].SetActive(!ThisEquipment.m_BodyObject.activeInHierarchy);
		}
	}

	public void DropInBin()
	{
		if(m_CurrentDoffingEquipment != EquipmentManager.eEquipment.MAX)
		{
			EquipmentNode	ThisEquipment = m_Equipment[(int)m_CurrentDoffingEquipment];

			ThisEquipment.m_DisposeObject.SetActive(false);
		}
		m_CurrentDoffingEquipment = EquipmentManager.eEquipment.MAX;
	}


	//who ios calling this and how? Send Message?
	public void DropInBucket()
	{
		if(m_CurrentDoffingEquipment != EquipmentManager.eEquipment.MAX)
		{
			EquipmentNode	ThisEquipment = m_Equipment[(int)m_CurrentDoffingEquipment];

			ThisEquipment.m_DisposeObject.SetActive(false);
		}
		m_CurrentDoffingEquipment = EquipmentManager.eEquipment.MAX;
	}




	internal void SetupForDonningAnimation(EquipmentManager.eEquipment Equipment)
	{
		EquipmentNode		ThisEquipment = m_Equipment[(int)Equipment];

		if(ThisEquipment.m_SwapObject != null && Equipment != EquipmentManager.eEquipment.Hood && Equipment != EquipmentManager.eEquipment.ReusableApron)
		{
			ThisEquipment.m_SwapObject.SetActive(true);
			ThisEquipment.m_BodyObject.SetActive(false);
		}
		else
			PutOn(Equipment);
	}

	internal void SetupForDoffingAnimation(EquipmentManager.eEquipment Equipment)
	{
		Debug.Log("setup for doffing animation: " + Equipment.ToString());
		
		EquipmentNode	ThisEquipment = m_Equipment[(int)Equipment];

		if (ThisEquipment.m_SwapObject != null)
		{
			ThisEquipment.m_SwapObject.SetActive(false);
			ThisEquipment.m_BodyObject.SetActive(true);

		}
		else
			TakeOff(Equipment);
	}

	internal float PlayDisposalAnimationOnActiveEquipment()
	{
		if(m_CurrentDoffingEquipment != EquipmentManager.eEquipment.MAX)
		{
			EquipmentNode		ThisEquipment = m_Equipment[(int)m_CurrentDoffingEquipment];

			if (ThisEquipment.m_DisposeType == eDispose.Bin)
			{
				SetAnimationTrigger("PutInBin");
				OurCamera.Instance.MoveTo(OurCamera.eCameraFocus.Bin, 0.7f);
				return (3);
			}
			else if (ThisEquipment.m_DisposeType == eDispose.Bucket)
			{
				SetAnimationTrigger("PutInBucket");
				OurCamera.Instance.MoveTo(OurCamera.eCameraFocus.Bucket, 0.7f);
				return (3);
			}
		}
		return (0);
	}

	internal float PlayDonningAnimation(EquipmentManager.eEquipment Equipment)
	{


		EquipmentNode	ThisEquipment = m_Equipment[(int)Equipment];

		if(ThisEquipment.m_DonningName != null)
		{
			Debug.Log("PlayDonningAnimation " + ThisEquipment.m_DonningName);
			SetAnimationTrigger(ThisEquipment.m_DonningName);
			return (ThisEquipment.m_DonningDuration);
		}
		return (-1);
	}



	internal float PlayDoffingAnimation(EquipmentManager.eEquipment Equipment)
	{
		EquipmentNode	ThisEquipment = m_Equipment[(int)Equipment];
        
		m_CurrentDoffingEquipment = Equipment;
		if (ThisEquipment.m_DoffingName != null)
		{
			SetAnimationTrigger(ThisEquipment.m_DoffingName);
			return(ThisEquipment.m_DoffingDuration);
		}
		return (-1);
	}



	internal float PlayDoffingAnimationWithTime(float animationDuration, string triggerName)
	{
		SetAnimationTrigger(triggerName);
		return(animationDuration);
	}



	internal void FinishedDoffing(EquipmentManager.eEquipment Equipment)
	{
		Debug.Log("Finished doffing : " + Equipment.ToString());

		EquipmentNode	ThisEquipment = m_Equipment[(int)Equipment];

		if (ThisEquipment.m_DisposeObject == null)
			m_CurrentDoffingEquipment = EquipmentManager.eEquipment.MAX;

		if (ThisEquipment.m_DisposeObject != null)
			ThisEquipment.m_DisposeObject.SetActive(true);
		
		if (ThisEquipment.m_SwapObject != null && ThisEquipment.m_SwapObject != ThisEquipment.m_DisposeObject)
			ThisEquipment.m_SwapObject.SetActive(false);
	}



	public void SwapToDisposalOject()
	{
		if(m_CurrentDoffingEquipment != EquipmentManager.eEquipment.MAX)
		{
			EquipmentNode		ThisEquipment = m_Equipment[(int)m_CurrentDoffingEquipment];

			if (ThisEquipment.m_DisposeObject != null)
				ThisEquipment.m_DisposeObject.SetActive(true);

			if (ThisEquipment.m_SwapObject != null && ThisEquipment.m_SwapObject != ThisEquipment.m_DisposeObject)
				ThisEquipment.m_SwapObject.SetActive(false);
		}
	}


	public void HideDisposedObject(EquipmentManager.eEquipment equipment)
	{
		if (equipment == EquipmentManager.eEquipment.FaceShield) {
			if (m_FaceShield != null) {
				m_FaceShield.SetActive(false);
			}
		}

	}



	[ContextMenu("SwitchOffHeldEquipment")]
	internal void SwitchOffHeldEquipment()
	{
		if (m_AnimFaceShield != null) m_AnimFaceShield.SetActive(false);
		if (m_AnimGoggles != null) m_AnimGoggles.SetActive(false);
		if (m_AnimFaceMask != null) m_AnimFaceMask.SetActive(false);
		if (m_AnimApron != null) m_AnimApron.SetActive(false);
		if (m_AnimHood != null) m_AnimHood.SetActive(false);
		if (m_DisposeGown != null) m_DisposeGown.SetActive(false);
		if (m_DisposeApron != null) m_DisposeApron.SetActive(false);
//		if (m_AnimClipboard != null)
//			m_AnimClipboard.SetActive(false);
	}



	[ContextMenu("BaseClothing")]
	internal void BaseClothing()
	{
		m_Jewelry.SetActive(true);
		m_FaceMask.SetActive(false);
		m_Gloves.SetActive(false);
		m_Gown.SetActive(false);
		m_Hood.SetActive(false);
		m_FaceShield.SetActive(false);
		if (m_Goggles != null) m_Goggles.SetActive(false);
		m_DoubleGloves.SetActive(false);
		if (m_Apron != null) m_Apron.SetActive(false);
		m_Shoes.SetActive(false);
		m_Boots.SetActive(true);
		if (m_Suit != null) m_Suit.SetActive(false);
		m_Top.SetActive(true);
		m_Bottom.SetActive(true);
		if (m_Hair != null) m_Hair.SetActive(true);
	}

	[ContextMenu("FullyEquipped")]
	internal void FullyEquipped()
	{
		m_Jewelry.SetActive(false);
		m_FaceMask.SetActive(true);
		m_Gloves.SetActive(false);
		m_Gown.SetActive(true);
		m_Hood.SetActive(true);
		m_FaceShield.SetActive(true);
		if (m_Goggles != null) m_Goggles.SetActive(false);
		m_DoubleGloves.SetActive(true);
		if (m_Apron != null) m_Apron.SetActive(true);
		m_Shoes.SetActive(false);
		m_Boots.SetActive(true);
		if (m_Suit != null) m_Suit.SetActive(false);
		m_Top.SetActive(false);
		m_Bottom.SetActive(false);
		if (m_Hair != null) m_Hair.SetActive(false);
	}

    public void SetPosition(Vector3 newPos)
    {
        transform.position = newPos;
    }



	//used for hazards only
	public void WearBsicPPE() {
		this.PutOn(EquipmentManager.eEquipment.FaceShield);
		this.PutOn(EquipmentManager.eEquipment.Gown);
		this.PutOn(EquipmentManager.eEquipment.InnerGloves);

	}



	//Used for swapping rigs
	internal void CopyOtheAvatar(Avatar otherAvatar)
	{
		m_Jewelry.SetActive(otherAvatar.m_Jewelry.activeSelf);
		m_FaceMask.SetActive(otherAvatar.m_FaceMask.activeSelf);
		m_Gloves.SetActive(otherAvatar.m_Gloves.activeSelf);
		m_Gown.SetActive(otherAvatar.m_Gown.activeSelf);
		m_Hood.SetActive(otherAvatar.m_Hood.activeSelf);
		m_FaceShield.SetActive(otherAvatar.m_FaceShield.activeSelf);
		if (m_Goggles != null && otherAvatar.m_Goggles != null) m_Goggles.SetActive(otherAvatar.m_Goggles.activeSelf);
		m_DoubleGloves.SetActive(otherAvatar.m_DoubleGloves.activeSelf);
		if (m_Apron != null && otherAvatar.m_Apron != null) m_Apron.SetActive(otherAvatar.m_Apron.activeSelf);
		m_Shoes.SetActive(otherAvatar.m_Shoes.activeSelf);
		m_Boots.SetActive(otherAvatar.m_Boots.activeSelf);
		if (m_Suit != null && otherAvatar.m_Suit != null) m_Suit.SetActive(otherAvatar.m_Suit.activeSelf);
		m_Top.SetActive(otherAvatar.m_Top.activeSelf);
	
		m_Bottom.SetActive(otherAvatar.m_Bottom.activeSelf);
		m_Hair.SetActive(otherAvatar.m_Hair.activeSelf);
	}



	//pass animation event
	public void AnimationEventReceived(string eventName) {
		Debug.Log("Animation event: " + eventName);
		if (OnAnimationEventReceived != null) OnAnimationEventReceived(eventName);
	}








}
