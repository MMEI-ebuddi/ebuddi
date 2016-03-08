using System;
using UnityEngine;
using UnityEngine.UI;
using TMSupport;
using System.Collections.Generic;

public class UIClipboard : MonoBehaviour
{
	static float[]				sTimers = new float[] {0, 1, 10, 1};

	public RectTransform		m_ClipBoardTransform;

	public Sprite				m_Jewelry;
	public Sprite				m_WashHands;
	public Sprite				m_FaceMask;
	public Sprite				m_Gloves;
	public Sprite				m_Gown;
	public Sprite				m_Hood;
	public Sprite				m_FaceShield;
	public Sprite				m_Goggles;
	public Sprite				m_Apron;
	public Sprite				m_Boots;

	public GameObject[]			m_Displays;
	public Image[]				m_DisplayImages;
	public Image[]				m_HandwashImages;

	enum eState
	{
		Inactive = 0,
		BecommingActive,
		Active,
		BecommingInactive
	}
	eState		m_State;
	float		m_Timer;
	Vector3		m_Position;
	Vector3		m_OffscreenPosition;
	Vector3		m_OnscreenPosition;

	Func<float, float, float, float, float>		m_EaseFunction;

	public void Init()
	{
		m_Position = m_ClipBoardTransform.localPosition;
		m_OffscreenPosition = m_OnscreenPosition = m_Position;
		m_OffscreenPosition.x = -1000;
		m_EaseFunction = GoTweenUtils.easeFunctionForType(GoEaseType.QuadInOut);

        m_ClipBoardTransform.gameObject.SetActive(false);
	}

	void Awake()
	{
		Init();      
	}


    // REFACTOR new function
    public void Setup(List<BaseActionScriptableObject> actionList)
    {
        HideAllIcons();

        int iIndex = 0;
       
        foreach (BaseActionScriptableObject action in actionList)
        {
            if (action.Automatic)
                continue;

            if (action.WashHandsAter)
                m_HandwashImages[iIndex].gameObject.SetActive(true);

            // TODO: Check if this item is equipped
            m_DisplayImages[iIndex].sprite = action.Image;
            m_Displays[iIndex].SetActive(true);
            iIndex++;            
        }

        SlideIn();
    }
  
    private void HideAllIcons()
    {
        HideHandWashIcons();

        for (int i = 0; i < m_DisplayImages.Length; i++)
        {
            m_Displays[i].SetActive(false);
        }
    }
    private void HideHandWashIcons()
    {
        for (int loop = 0; loop < m_HandwashImages.Length; loop++)
        {
            m_HandwashImages[loop].gameObject.SetActive(false);
        }
    }

    private void SlideIn()
    {
        m_State = eState.BecommingActive;
        m_ClipBoardTransform.gameObject.SetActive(true);
        m_ClipBoardTransform.localPosition = m_Position = m_OffscreenPosition;
        m_Timer = 0;
    }
    
	void Update()
	{
		if(m_State != eState.Inactive)
		{
			float		Interp = m_EaseFunction(m_Timer / sTimers[(int)m_State], 0, 1, 1);

			if (m_State == eState.BecommingActive)
				m_Position = Vector3.Lerp(m_OffscreenPosition, m_OnscreenPosition, Interp);
			else if (m_State == eState.Active)
				m_Position = m_OnscreenPosition;
			else if (m_State == eState.BecommingInactive)
				m_Position = Vector3.Lerp(m_OnscreenPosition, m_OffscreenPosition, Interp);

			m_ClipBoardTransform.localPosition = m_Position;

			m_Timer += Time.deltaTime;
			if (m_Timer >= sTimers[(int)m_State])
			{
				if (m_State == eState.BecommingInactive)
				{
					m_State = eState.Inactive;
					TMSupport.TMMessenger.Send("ClipboardDone".GetHashCode());
				}
				else
					m_State++;

				m_Timer = 0;
			}

			if (Application.isEditor) {
				if (Input.GetKeyDown(KeyCode.Space)) {
					if (m_State == eState.Active) m_Timer = 11f;
				}
			}
		}
	}
}
