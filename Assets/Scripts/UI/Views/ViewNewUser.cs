using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMSupport;

public class ViewNewUser : UIView 
{

	public UIGuidedInputField nameInput;
	public UIGuidedInputField numberInput;
	public UIGuidedInputField ageInput;
	public UIGuidedToggle sexToggle;
	public Toggle maleToggle;

	public GameObject newUserObject;
	private Vector3 middlePosition = new Vector3(0,0,0);
	private Vector3 topPosition = new Vector3(0,200f,0);

	public override void ViewWillAppear() {
		nameInput.SetFrame(true);
		numberInput.SetFrame(false);
		sexToggle.UpdateToggle(false);
		ageInput.SetFrame(false);
		base.ViewWillAppear();
	}


	#if UNITY_ANDROID
	void Update()
	{
		if(nameInput.input.isFocused || numberInput.input.isFocused || ageInput.input.isFocused)
		{
			if(TouchScreenKeyboard.visible)
			{
				if(newUserObject.transform.localPosition != topPosition) 
				{
					newUserObject.transform.localPosition = topPosition;
				}
			}
			else
			{
				if(newUserObject.transform.position != middlePosition) 
				{
					newUserObject.transform.localPosition = middlePosition;
				}
			}
		}
	}
	#endif





	public void BackButton()
	{
		TMMessenger.Send("ENTRYEXITED".GetHashCode());
		Hide ();
	}


	public void StartButton()
	{
		if(VerifyCreatedUser()) {

			UserProfile.eSex Sex = maleToggle.isOn ? UserProfile.eSex.Male : UserProfile.eSex.Female;
			
			UserProfile.sCurrent.SetNameSexAge(nameInput.input.text, Sex, int.Parse(ageInput.input.text));
			UserProfile.sCurrent.SceneComplete(0);

			TMMessenger.Send("NEW_USER_COMPLETE".GetHashCode());
			Hide();
		}
	}



	public bool VerifyCreatedUser()
	{
		if(!nameInput.IsFilled())
		{
			Debug.LogError("Name input empty");
			return false;
		}
		else if(!ageInput.IsFilled())
		{
			Debug.LogError("Age field empty");
			return false;
		}
		else if(!sexToggle.IsSet())
		{
			Debug.LogError("Sex not chosen");
			return false;
		}
		else
		{
			return true;
		}
	}



	public void RefreshInputs()
	{
		if(!nameInput.IsFilled())
		{
			nameInput.SetFrame(true);
			numberInput.SetFrame(false);
			sexToggle.UpdateToggle(false);
			ageInput.SetFrame(false);
		}
		else if(!sexToggle.IsSet())
		{
			nameInput.SetFrame(false);
			numberInput.SetFrame(false);
			sexToggle.UpdateToggle(true);
			ageInput.SetFrame(false);
		}
		else if(!ageInput.IsFilled())
		{
			nameInput.SetFrame(false);
			numberInput.SetFrame(false);
			sexToggle.UpdateToggle(false);
			ageInput.SetFrame(true);
		}
	}



}
