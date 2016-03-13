using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMSupport;

public class UITitle : MonoBehaviour
{
	static UITitle			sInstance;
	public static UITitle	Instance { get { return sInstance; } }
	
	public GameObject		m_InfectionGame;
    public GameObject       m_Introduction;
	public GameObject		m_Options;
	public GameObject		m_Exit;

	public GameObject		m_NewUser;
	public GameObject		m_LoadUser;
	public GameObject[]		m_Sections;

	List<GameObject>		m_UIObjects;

    public bool             DefaultUnlocked = true;

    public string           InfectionSceneName = "infection_room";
    public string           IntroductionSceneName = "introduction_to_ebola";

    public float SelectedPosterScale = 1.2f;

    private Vector3 m_OrigPosterScale;
    private Vector3 m_ScaledPosterScale;

	enum eState
	{
		Inactive = 0,
		BaseMenu,
		Start,
		Practice,
		Options,
		MAX
	}
	eState					m_CurrentState;

	int						m_SelectableLayer;
	int						m_HighlightLayer;
	int						m_LayerMask;

	float[]					m_SelectionTargets;

	bool					m_LoadUserHighlighted;
	bool					m_NewUserHighlighted;
	bool[]					m_PosterHighlighted;

	bool					m_Enabled;

	void OnDestroy()
	{
		TMMessenger.Instance.RemoveFromMessageQueue(this);
	}

	void Awake()
	{
        m_OrigPosterScale = m_Sections[0].transform.localScale;
        m_ScaledPosterScale = m_OrigPosterScale * SelectedPosterScale;

		sInstance = this;
		TMMessenger.Instance.AddToMessageQueue(new MessageCallback(this.MessageCallback));

		m_SelectableLayer = LayerMask.NameToLayer("UISelectable");
		m_HighlightLayer = LayerMask.NameToLayer("UISelectableHighlight");
		m_LayerMask = (1 << m_SelectableLayer) | (1 << m_HighlightLayer);

		m_UIObjects = new List<GameObject>(20);
		m_UIObjects.Add(m_NewUser);
		m_UIObjects.Add(m_LoadUser);
//		m_UIObjects.Add(m_Cheat);
//		m_UIObjects.Add(m_Options);
//		m_UIObjects.Add(m_Exit);

		m_PosterHighlighted = new bool[4];
		for (int loop = 0; loop < m_PosterHighlighted.Length; loop++)
			m_PosterHighlighted[loop] = false;

		m_LoadUserHighlighted = false;
		m_NewUserHighlighted = false;

		for(int loop = 0; loop < m_Sections.Length; loop++)
		{
			m_UIObjects.Add(m_Sections[loop]);
		}
		SetupForNewUser();
		m_CurrentState = eState.Inactive;

		m_SelectionTargets = new float[m_UIObjects.Count];
		for (int loop = 0; loop < m_UIObjects.Count; loop++)
		{
			m_SelectionTargets[loop] = 0;
		}
	}

	void MessageCallback(TMMessageNode Message)
	{
		if (Message.Message == "ENTRYEXITED".GetHashCode())
			m_CurrentState = eState.BaseMenu;
		else if (Message.Message == "OPTIONSEXITED".GetHashCode())
			m_CurrentState = eState.BaseMenu;
		else if (Message.Message == "LOAD_USER_COMPLETE".GetHashCode())
		{
			SetupForNewUser();
			m_CurrentState = eState.BaseMenu;
		}
	}

	Color		sPosterActve = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	Color		sPosterInactive = new Color(1.0f, 1.0f, 1.0f, 0.5f);

	private void SetupForNewUser()
	{
		for (int loop = 0; loop < m_Sections.Length; loop++)
		{
            if (UserProfile.sCurrent.Name.Equals(""))
                m_Sections[loop].gameObject.GetComponent<Renderer>().material.color = sPosterInactive;
            else
                m_Sections[loop].gameObject.GetComponent<Renderer>().material.color = sPosterActve;		
		}
	}

	void Update()
	{
		if(m_CurrentState == eState.BaseMenu)
		{
            // Skip all this if we're hovering the mouse over a UI element so we can't click things behind UI
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

			Ray			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit	hit;
			GameObject	HighlightObject = null;

			// Casts the ray and get the first game object hit
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_LayerMask))
			{
//				Debug.Log("raycast");
				HighlightObject = hit.collider.gameObject;

				int		PosterIndex = Array.IndexOf(m_Sections, HighlightObject);

                if (PosterIndex >= 0)
                {                    
                    // Are we flagged to unlock all scenes from the start?
                    if (DefaultUnlocked)
                    {
                        // If the current user profile doesn't have a name set, we've not logged in/registered yet, so disabled scene selection
                        if (UserProfile.sCurrent.Name.Equals(""))
                            HighlightObject = null;
                    }
                    else
                    {                    
                        if (UserProfile.sCurrent.GetSceneCompleteCount(PosterIndex) == -1)
                            HighlightObject = null;                    
                    }

                    // Don't allow the last section to be selected as it doesn't exist yet.
                    if ( PosterIndex == (m_Sections.Length))
                        HighlightObject = null;
                }          
				else {
					//introduction scene
					//refactor this when have time to use some button component
					if (HighlightObject.name == "PosterIntroduction" || HighlightObject.name == "PosterInfectionGame") {
						//if not logged in yet preventing from going inside introdution scene
						if (UserProfile.sCurrent.Name.Equals("")) HighlightObject = null; 
					}

				}
			}

			if (Input.GetMouseButtonDown(0))
			{
				if (m_InfectionGame == HighlightObject)
				{
					if (UILoading.instance != null) UILoading.instance.LoadScene(InfectionSceneName);
					else Application.LoadLevel(InfectionSceneName);
				}
                else if (m_Introduction == HighlightObject)
                {
					if (UILoading.instance != null) UILoading.instance.LoadScene(IntroductionSceneName);
					else Application.LoadLevel(IntroductionSceneName);
                }
				else if (m_Options == HighlightObject)
				{
					m_CurrentState = eState.Options;
					TMMessenger.Send("UIOptions".GetHashCode());
				}
			}

			for (int loop = 0; loop < m_UIObjects.Count; loop++)
			{
				if (m_UIObjects[loop] == HighlightObject)
				{
					m_SelectionTargets[loop] = Mathf.Lerp(m_SelectionTargets[loop], 1, 0.15f);

					if (Input.GetMouseButtonDown(0))
					{
						if (m_NewUser == HighlightObject)
						{
							m_CurrentState = eState.Start;
							TMMessenger.Send("NEW_USER".GetHashCode());
						}
						else if (m_LoadUser == HighlightObject)
						{
							m_CurrentState = eState.Start;
							TMMessenger.Send("LOAD_USER".GetHashCode());
						}
						else
						{
							int			SceneIndex = Array.IndexOf(m_Sections, HighlightObject);

							if(SceneIndex >= 0)
								TMMessenger.Send("LOADSCENE".GetHashCode(), 0, SceneIndex + 2);
//							HighlightObject = null;
						}
					}
				}
				else
					m_SelectionTargets[loop] = Mathf.Lerp(m_SelectionTargets[loop], 0, 0.15f);

				m_UIObjects[loop].transform.localScale = Vector3.Lerp(m_OrigPosterScale, m_ScaledPosterScale, m_SelectionTargets[loop]);
			}
		}

		if (m_CurrentState == eState.Inactive)
		{
			float		SineTime = (Mathf.Sin(Time.time * 3) + 1) * 0.5f;

			if(m_LoadUserHighlighted)
                m_LoadUser.transform.localScale = Vector3.Lerp(m_OrigPosterScale, m_ScaledPosterScale, SineTime);
			else if(m_NewUserHighlighted)
                m_NewUser.transform.localScale = Vector3.Lerp(m_OrigPosterScale, m_ScaledPosterScale, SineTime);
			else
			{
				for (int loop = 0; loop < m_PosterHighlighted.Length; loop++)
				{
					if(m_PosterHighlighted[loop])
                        m_Sections[loop].transform.localScale = Vector3.Lerp(m_OrigPosterScale, m_ScaledPosterScale, SineTime);
				}
			}
		}
	}

	internal void Activate()
	{
		m_CurrentState = eState.BaseMenu;
	}


	internal void Highlight(GameObject Poster, bool Enable = true)
	{

		if(Poster == m_LoadUser)
		{
			m_LoadUserHighlighted = Enable;
			if(Enable == false)
                m_LoadUser.transform.localScale = m_OrigPosterScale;
		}
		else if (Poster == m_NewUser)
		{
			m_NewUserHighlighted = Enable;
			if (Enable == false)
                m_NewUser.transform.localScale = m_OrigPosterScale;
		}
		else
		{
			for (int loop = 0; loop < m_PosterHighlighted.Length; loop++)
			{
				if (Poster == m_Sections[loop])
				{
					m_PosterHighlighted[loop] = Enable;
					if (Enable == false)
                        m_Sections[loop].transform.localScale = m_OrigPosterScale;
				}
			}
		}
	}
}
