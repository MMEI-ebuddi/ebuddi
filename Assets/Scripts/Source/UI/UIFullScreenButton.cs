using UnityEngine;
using System.Collections;
using TMSupport;

public class UIFullScreenButton : MonoBehaviour {




	public void OnClicked()
	{
		Debug.Log("FullScreenButtonTapped");
		TMMessenger.Send("FullScreenButtonTapped".GetHashCode());


	}
}
