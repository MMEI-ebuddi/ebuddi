using UnityEngine;
using TMSupport;
using System.Collections;

public class UIClickHandler : MonoBehaviour 
{
	public string							m_Message;
	public UIManager.eUIReferenceName		m_ThisCanvas;

	public void Restart() 
	{	
		Debug.Log("restartlevel : Msg");
		TMMessenger.Send("restartlevel".GetHashCode());
	}

	public void CustomMessage()
	{
		if (m_Message != null && m_Message.Length > 0)
		{ 
			TMMessenger.Send(m_Message.GetHashCode());
		}
	}

	public void CustomMessageAfterT(float Param)
	{
		if (m_Message != null && m_Message.Length > 0)
		{ 
			TMMessenger.Send(m_Message.GetHashCode(),0, Param);
		}
	}

	public void CustomMessage(string Message)
	{
		if (Message != null && Message.Length > 0)
		{
			TMMessenger.Send(Message.GetHashCode());
		}
	}

	public void CloseMe() 
	{
		TMMessenger.Send("CLOSEME".GetHashCode(),(int)m_ThisCanvas);
	}

	public void CloseMeAfterT(float Param) 
	{
		StartCoroutine( SetAfterDelay( Param, "CLOSEME", (int)m_ThisCanvas)  );
	}

	IEnumerator SetAfterDelay(float Delay, string Message, int CanvasID ) 
	{		
		yield return new WaitForSeconds(Delay);
		TMMessenger.Send(Message.GetHashCode(),CanvasID );
	}

	public void MoveCamera(GameObject Target) 
	{
		TMMessenger.Send("MoveCamera".GetHashCode(),(int)m_ThisCanvas , Target );
	}

	public void ReturnCamera()
	{
		TMMessenger.Send("CameraReturn".GetHashCode(),(int)m_ThisCanvas , null );
	}



	public void QuitToTitle() 
	{
		StartCoroutine(UIModalManager.instance.ShowAlert("Quit?", "Are you sure you want to quit the module?", new string[]{"Cancel", "Quit"}, delegate(string buttonClicked) {
		
			if (buttonClicked == "Quit") {
				TMMessenger.Send("UserQuit".GetHashCode());
				TMMessenger.Send("RETURNTOTITLE".GetHashCode());
			}
		}));

	}



	public void LanguageSwitch(int choice)
	{
		Debug.Log("Language Change");
		MediaNodeManager.ChangeLanguage( (eLanguage)choice );
	}
	 
	public void LoadScene(int SceneIndex)
	{
		Debug.Log("loadscene");
		TMMessenger.Send("LOADSCENE".GetHashCode(),(int)m_ThisCanvas, SceneIndex );
	}


}
