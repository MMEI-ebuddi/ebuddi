using UnityEngine;
using System.Collections;
using TMSupport;
using System.Text;
using System;
using UnityEngine.UI;


public class UIManager : MonoBehaviour 
{
	private static UIManager		sInstance;
	public static UIManager			Instance { get { return sInstance; } }

	const string					cUIpath = "Prefabs/UI/";
	const string					cUISuffix = " Canvas";

	private ViewNewUser viewNewUser;
	public ViewNewUser ViewNewUser {
		get {
			if (viewNewUser == null) {
				viewNewUser = FindObjectOfType<ViewNewUser>();
			}
			return viewNewUser;

		}
	}

	static string[]					sUIPrefabFilenames = null;
	public static GameObject[]		sUIPrefabs = null;
	
	public enum eUIReferenceName
	{
		MainMenu = 0,
		Options,
		BasicEntry,
		Practice,
		Form,
		MainDialog,
		HeaderDialog,
		DonningItemSelection,
		BuddyDialog,
		ExitToTitle,
		SceneDescription1,
		SceneDescription2,
		SceneDescription3,
		SceneDescription4,
		BasicEnhancedPPE,
		DoffingItemSelection,
		RightWrongSelection,
		MoviePlay,
		HazardRoundup,
		LoadUser,
		EndOfDonning,
		ProgressBar,
		MAX
	}

	StringBuilder			m_TempString = new StringBuilder();
	Vector3					m_StoredPos;
	Vector3					m_StoredRot;

	[SerializeField]
	private eUIReferenceName m_LastCanvas;
	[SerializeField]
	private eUIReferenceName m_ActiveCanvas;

	static public void Create() 
	{
		if ( sInstance == null )
		{ 
			GameObject	NewGameObject = new GameObject("UIManager");

			sInstance = NewGameObject.AddComponent<UIManager>();
			sInstance.Initialise();
			DontDestroyOnLoad(NewGameObject);
		}
	}

	void OnDestroy()
	{
		TMMessenger.Instance.RemoveFromMessageQueue(this);
	}

	void Awake() 
	{
		TMMessenger.Instance.AddToMessageQueue(new MessageCallback(this.MessageCallback));
		if (sInstance == null)
		{ 
			Initialise();
		}
	}

	public void Initialise() 
	{
	//	if(sInstance != null)
	//		return;

		sInstance = this;

		int		NumUISets = System.Enum.GetValues(typeof(eUIReferenceName)).Length;

		if(sUIPrefabFilenames == null)
			sUIPrefabFilenames = new string[NumUISets];
		if(sUIPrefabs == null)
			sUIPrefabs = new GameObject[NumUISets];

		for (int loop = 0; loop < NumUISets; loop++)
		{ 
			//skip refactored UI
			if (Enum.GetName(typeof(eUIReferenceName), loop)== "ExitToTitle" ||
			    Enum.GetName(typeof(eUIReferenceName), loop)== "BuddyDialog" ||
			    Enum.GetName(typeof(eUIReferenceName), loop)== "UIOptions" 	||
			    Enum.GetName(typeof(eUIReferenceName), loop)== "ProgressBar" 
			    ) {
//				Debug.Log("Skip UI creation " + Enum.GetName ( typeof(eUIReferenceName), loop) );
				continue;
			}



			m_TempString.Length = 0;
			m_TempString.Append(cUIpath);
			//m_TempString.Append(UIReferenceName.MainMenu.ToString());
			m_TempString.Append(Enum.GetName ( typeof(eUIReferenceName), loop) );
			m_TempString.Append(cUISuffix);
			sUIPrefabFilenames[loop] = m_TempString.ToString();
//			Debug.Log( sUIPrefabFilenames[loop] );
		}
	}
	
	public void OpenUI( eUIReferenceName UICanvas, float Delay = 0.0f)
	{
		m_LastCanvas = m_ActiveCanvas;
		m_ActiveCanvas = UICanvas;
		if (sUIPrefabs == null)
		{ 
			Debug.Log("UI Prefab Error");
		}

		if (sUIPrefabs[(int)UICanvas] == null)
		{
			//create
//			Debug.Log( UICanvas +" Loading "+  Delay +" @" + Time.timeSinceLevelLoad) ;
			sUIPrefabs[(int)UICanvas] =  Instantiate( Resources.Load<GameObject>( sUIPrefabFilenames[(int) UICanvas ] ) as GameObject, Vector3.zero, Quaternion.identity) as GameObject;
			//DontDestroyOnLoad(sUIPrefabs[(int)UICanvas]);
			//sUIPrefabs[(int)UICanvas].transform.SetParent(this.transform,false); 	
		//	StartCoroutine( SetActiveDelay(sUIPrefabs[(int)UICanvas], Delay ) );
			if (sUIPrefabs[(int)UICanvas] != null)
			{
				StartCoroutine(SetUIAtEndOfFrame(sUIPrefabs[(int)UICanvas]));
			}
		}
		else
		{ 
			//StartCoroutine( SetActiveDelay(sUIPrefabs[(int)UICanvas], Delay ) );
			StartCoroutine( SetUIAtEndOfFrame(sUIPrefabs[(int)UICanvas]) );
		//	sUIPrefabs[(int)UICanvas].SetActive(true);
		}
	
	}

	public void CloseUI(eUIReferenceName UICanvas, bool KeepOpen = false)
	{
		if (sUIPrefabs[(int)UICanvas] == null && KeepOpen == false)
			return;

		if (!KeepOpen) 
		{
			sUIPrefabs[(int)UICanvas].SetActive(false);
			return;
		}
		sUIPrefabs[(int)UICanvas].SetActive(true);
	}
	
	void CreateUI(string Canvas)
	{
		m_TempString.Length = 0;
		m_TempString.Append(cUIpath);
		m_TempString.Append(Canvas);
		m_TempString.Append(cUISuffix);
		Debug.Log( m_TempString.ToString() );
		GameObject MMenu = Resources.Load<GameObject>( m_TempString.ToString() ) as GameObject;
		sUIPrefabs[0] = Instantiate( MMenu, Vector3.zero, Quaternion.identity) as GameObject;
		sUIPrefabs[0].transform.SetParent(this.transform,false); 
		sUIPrefabs[0].SetActive(false);
		StartCoroutine( SetUIAtEndOfFrame( sUIPrefabs[0] ) );
	}

	IEnumerator SetUIAtEndOfFrame(GameObject Target) 
	{	
		// may fail for some canvus types??
		Target.GetComponent<Canvas>().worldCamera = Camera.main;
		yield return new WaitForEndOfFrame();
		Target.SetActive(true);
//		Debug.Log( Target.name +" ON  @" + Time.timeSinceLevelLoad ) ;
	}

	IEnumerator SetActiveDelay(GameObject Target, float Delay ) 
	{		
		yield return new WaitForSeconds(Delay);
		StartCoroutine( SetUIAtEndOfFrame( Target ) );		
	}

	protected void MessageCallback(TMMessageNode Message)
	{
	

		if (Message.Message == "NEW_USER".GetHashCode())
		{
			CloseUI( m_ActiveCanvas );
//			OpenUI( eUIReferenceName.BasicEntry );	
			ViewNewUser.Show();
		}
		else if (Message.Message == "LOAD_USER".GetHashCode())
		{
			CloseUI(m_ActiveCanvas);
			OpenUI(eUIReferenceName.LoadUser);
		}
		else if (Message.Message == "NEW_USER_COMPLETE".GetHashCode())
		{ 
			CloseUI( m_ActiveCanvas );
			// Load ebola introduction
//			Application.LoadLevel("introduction_to_ebola");		// Ebola introduction
//			Application.LoadLevel("Scene1");							// Donning Scene
		}

		else if (Message.Message == "UIPractice".GetHashCode())
		{
			CloseUI(eUIReferenceName.MainMenu);
			OpenUI(eUIReferenceName.Practice);
		}
		else if (Message.Message == "UIOptions".GetHashCode())
		{
			CloseUI(eUIReferenceName.MainMenu);
			OpenUI(eUIReferenceName.Options);
		}
		else if (Message.Message == "UIExit".GetHashCode())
		{
			//app cleanup
			//try to send data?
			Application.Quit();
		}
		else if (Message.Message == "BACK".GetHashCode())
		{
			//Close Current
			CloseUI(m_ActiveCanvas);
			//Open Last
			OpenUI(m_LastCanvas);
		}
		else if (Message.Message == "MAINMENU".GetHashCode())
		{
			//CloseUI( eUIReferenceName.MainMenu, true ); // close all but main menu
			OpenUI(eUIReferenceName.MainMenu);
		}
		else if (Message.Message == "OPENITEMSELECTION".GetHashCode())
		{
			OpenUI(eUIReferenceName.DonningItemSelection);
		}

		//one removed YEY!
//		else if (Message.Message == "OPENEXITTOTITLE".GetHashCode())
//		{
////			if (Application.loadedLevelName != "Scene1") {
//				OpenUI(eUIReferenceName.ExitToTitle);
////			}
//		}
		else if (Message.Message == "CLOSEITEMSELECTION".GetHashCode())
		{
			CloseUI(eUIReferenceName.DonningItemSelection);
		}

		else if (Message.Message == "ShowHeaderDialog".GetHashCode())
		{
			OpenUI(eUIReferenceName.HeaderDialog);
		}

		else if (Message.Message == "ShowMainDialog".GetHashCode())
		{
			CloseUI(eUIReferenceName.MainDialog);
		}

		else if (Message.Message == "CLOSEME".GetHashCode())
		{
			CloseUI((eUIReferenceName)Message.Sender);
		}

		else if (Message.Message == "STARTSECTION".GetHashCode())
		{
			Debug.Log("STARTING " + Message.Sender + " " + Message.Custom);
			//CloseUI((UIReferenceName)Message.Sender);
		}
		else if (Message.Message == "RETURNTOTITLE".GetHashCode())
		{
			MediaClipManager.Instance.Exit();
			Debug.Log("RETURNTOTITLE");
			if (UILoading.instance != null) UILoading.instance.LoadScene("Title");
			else Application.LoadLevel("Title");
		}
		else if (Message.Message == "LOADSCENE".GetHashCode())
		{
			MediaClipManager.Instance.Exit();
			//CloseUI(m_ActiveCanvas);
			string sceneName = SceneManager.GetSceneNameFromIndex( (SceneManager.SceneId)Message.Custom );
			if (UILoading.instance != null) UILoading.instance.LoadScene(sceneName);
			else Application.LoadLevel(sceneName);
		}
		else if (Message.Message == "OPENSCENEDESCRIPTION".GetHashCode())
		{
			//CloseUI(m_ActiveCanvas);
			switch((int)Message.Sender)
			{
				case 1: OpenUI(eUIReferenceName.SceneDescription1); break;
				case 2: OpenUI(eUIReferenceName.SceneDescription2); break;
				case 3: OpenUI(eUIReferenceName.SceneDescription3); break;
				case 4: OpenUI(eUIReferenceName.SceneDescription4); break;
			}
		}


#if false
		else if (Message.Message == "MoveCamera".GetHashCode())
		{
			//Debug.Log();
			if (Message.Custom != null)
			{
				m_StoredPos = Camera.main.transform.position;
				m_StoredRot = Camera.main.transform.rotation.eulerAngles;

				Camera.main.transform.positionTo(1.0f, ((GameObject)Message.Custom).transform.position, false);
				Camera.main.transform.rotationTo(1.0f, ((GameObject)Message.Custom).transform.rotation.eulerAngles, false);
			}
		}
		else if (Message.Message == "CameraReturn".GetHashCode())
		{
		
				Camera.main.transform.positionTo(1.0f, m_StoredPos, false);
				Camera.main.transform.rotationTo(1.0f, m_StoredRot, false);		
		}
#endif
	}

	internal UIDialog BringUpDialog(eUIReferenceName UIName, string Text = null, string Header = null, Sprite Image = null)
	{
		if(UIName == eUIReferenceName.BuddyDialog)
			CloseUI(eUIReferenceName.MainDialog);
		else
			CloseUI(eUIReferenceName.BuddyDialog);

		OpenUI(UIName);

		UIDialog Dialog = sUIPrefabs[(int)UIName].GetComponent<UIDialog>();

		if(Dialog != null) Dialog.SetTextAndHeaderAndImage(Text, Header, Image);

		return(Dialog);
	}

	internal UIDialog BringUpCanvas(eUIReferenceName UIName)
	{
		OpenUI(UIName);

		UIDialog	Dialog = sUIPrefabs[(int)UIName].GetComponent<UIDialog>();

		return (Dialog);
	}
}
