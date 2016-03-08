using UnityEngine;
using System.Collections;

public class UIActionBarButton : MonoBehaviour {

	public UIActionBar ActionBar;
	public UIActionBar.CLICK_ACTION Action;
	
	public void ButtonClicked()
	{
		ActionBar.SetCurrentAction(Action);
	}
}
