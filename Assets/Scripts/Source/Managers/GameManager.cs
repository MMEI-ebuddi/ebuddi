
#define NO_LOCAL_FILES

using System;
using UnityEngine;
using TMSupport;

using System.Collections;

public class GameManager : MonoBehaviour
{
	private static GameManager			sInstance;
	public static GameManager			Instance { get { return sInstance; } }

	public static readonly string		sBaseContentDir = "Assets/Resources/";
	public static int					sBuildNumber = 2;
	static bool							sInitialised = false;
	public static byte[]				sBuildVersion = new byte[] { 2, 2, 1, 0 };

	public enum eGameMode
	{
		menu = 0,
		standard = 1,
	}

	public eGameMode					m_GameMode;

	static public bool			sCalledFromTitleScreen = false;

	bool						m_DisplayFPS;

	int							sVSyncSetting;
	int							m_ScreenshotTimer;

	public Buddy				Buddy { get { return m_Buddy; } }
	Buddy						m_Buddy;
	public Scene				Scene { get { return m_Scene; } }
	Scene						m_Scene;

	UserProfileManager			m_UserProfileManager;

	public static void SetupTMSystem()
	{
		OurSaveState.Create();
	}

	void Awake()
	{
		Application.targetFrameRate = 60;

		sInstance = this;
		if (sInitialised == false)
		{
			SetupTMSystem();

			sInitialised = true;

			MediaNodeManager.LoadCSV("TextSpeechDatabase");
		}
//UI	
		if ( UIManager.Instance == null )
			UIManager.Create();

#if DEMO_BUILD || EARLY_ACCESS
		TMSystem.SaveState.SetInt("playedtutorial_00", 1);
#endif

		m_ScreenshotTimer = 0;
		m_DisplayFPS = false;

		m_Buddy = GameObject.FindObjectOfType<Buddy>();
		m_Scene = GameObject.FindObjectOfType<Scene>();
		m_UserProfileManager = UserProfileManager.Get();
	}

	void Start()
	{
//		EmailManager.SendEmail("masangahospitalppe@gmail.com", "masangahospitalppe@gmail.com", "Email Test", strEmailXML, "4769438fhdjsksh78839574");
//		EmailManager.SendEmail("itsbod@hotmail.com", "weeksy@gmail.com", "Email Test", "Email Test Body", "san6fransix");
	}	
	
	void Update ()
	{		
#if ENABLE_DEBUGTOOLS
		OurDebugMenu.Instance.Update();
		OurLogger.Instance.DisplayLog();
#endif
		if (Input.GetKeyDown(KeyCode.F1))
		{
			TakeScreenshot(1);
		}
		if (Input.GetKeyDown(KeyCode.F2))
		{
			TakeScreenshot(4);
		}

		if (m_ScreenshotTimer > 0)
		{
			m_ScreenshotTimer--;
			if (m_ScreenshotTimer == 0)
			{
//				TMSystem.UI.SetUIValue(TMUI.eAnimType.Alpha, 0, "DebugScreen", "Watermark");
			}
		}
	}

	void TakeScreenshot(int SuperSize)
	{
		m_ScreenshotTimer = 2;

		int		ScreenshotIndex = OurSaveState.Instance.GetInt("screenshot");

		Application.CaptureScreenshot("Screenshot" + ScreenshotIndex.ToString() + ".png", SuperSize);
		OurSaveState.Instance.AddInt("screenshot", 1);
	}

// 	void OnGUI()
// 	{
// 		if(Event.current.type.Equals(EventType.Repaint))
// 		{
// 	}

	void OnDestroy()
	{
	}
}


