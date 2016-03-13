using UnityEngine;
using System.Collections;

public class UIActionBar : MonoBehaviour {

	public enum CLICK_ACTION
	{
		NONE,
		KEEP_DISTANCE,
		ISOLATE,
		PPE,
	};
	
	private CLICK_ACTION m_CurrentAction;
	
	public void SetCurrentAction(CLICK_ACTION action)
	{
		if (action == m_CurrentAction)
			m_CurrentAction = CLICK_ACTION.NONE;
		else
			m_CurrentAction = action;
	}
	
	public CLICK_ACTION GetCurrentAction()
	{
		return m_CurrentAction;
	}
}
