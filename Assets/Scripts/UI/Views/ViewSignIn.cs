using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMSupport;

public class ViewSignIn : UIView 
{

	public UIGuidedInputField nameInput;

	public GameObject signInObject;
	private Vector3 middlePosition = new Vector3(0,0,0);
	private Vector3 topPosition = new Vector3(0,250f,0);
	
	public override void ViewWillAppear() {
		nameInput.SetFrame(true);
		base.ViewWillAppear();
	}


	public void BackButton()
	{
		Hide ();
	}
	
	
	public void StartButton()
	{
		if(VerifySignedUser()) {

			if(nameInput.input.text.Length > 0)
			{
				if(UserProfileManager.Instance.DoesUserProfileExist(nameInput.input.text))
				{
					if (UserProfile.Load(nameInput.input.text + ".xml"))
					{
						TMMessenger.Send("LOAD_USER_COMPLETE".GetHashCode());
						Hide();
					}
				}
				else {
					Debug.LogError("User not found, modal needed here");
				}
				
			}


		}
	}

#if UNITY_ANDROID
	void Update()
	{
		if(nameInput.input.isFocused)
		{
			if(TouchScreenKeyboard.visible)
			{
				if(signInObject.transform.localPosition != topPosition) 
				{
					signInObject.transform.localPosition = topPosition;
				}
			}
			else
			{
				if(signInObject.transform.position != middlePosition) 
				{
					signInObject.transform.localPosition = middlePosition;
				}
			}
		}
	}
#endif

	
	public bool VerifySignedUser()
	{
		if(!nameInput.IsFilled())
		{
			Debug.LogError("Name input empty");
			return false;
		}
		else
		{
			return true;
		}
	}


}
