using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ViewConfiguration : UIView
{

	public InputField hospitalInput;
	public InputField trainerInput;

	public Toggle basicToggle;
	public Toggle enhancedToggle;
	public Toggle powerSavingToggle;

	public GameObject configurationObject;
	private Vector3 middlePosition = new Vector3(0,0,0);
	private Vector3 topPosition = new Vector3(0,200f,0);



	public override void ViewWillAppear() 
	{
		SetValues();
		base.ViewWillAppear();
	}
	
	

	public override void ViewWillDisappear() 
	{
		base.ViewWillDisappear();
	}


#if UNITY_ANDROID
	void Update()
	{
		if(hospitalInput.isFocused || trainerInput.isFocused)
		{
			if(TouchScreenKeyboard.visible)
			{
				if(configurationObject.transform.localPosition != topPosition) 
				{
					configurationObject.transform.localPosition = topPosition;
				}
			}
			else
			{
				if(configurationObject.transform.position != middlePosition) 
				{
					configurationObject.transform.localPosition = middlePosition;
				}
			}
		}
	}
#endif



	//Check if hospital and trainer saved in player prefs and set inputs texts
	void SetValues()
	{
		hospitalInput.text = PlayerPrefs.GetString("hospitalName", "");
		trainerInput.text = PlayerPrefs.GetString("trainerName", "");
		basicToggle.isOn = PlayerPrefs.GetInt("basicPPESupported", 1) == 1;
		enhancedToggle.isOn = PlayerPrefs.GetInt("enhancedPPESupported", 1) == 1;
		powerSavingToggle.isOn = PlayerPrefs.GetInt("powerSavingOn", 0) == 1;
	}


	//saving persistent data
	public void StoreConfig() {
		PlayerPrefs.SetString("hospitalName", hospitalInput.text);
		PlayerPrefs.SetString("trainerName", trainerInput.text);
		PlayerPrefs.SetInt("basicPPESupported", basicToggle.isOn ? 1 : 0);
		PlayerPrefs.SetInt("enhancedPPESupported", enhancedToggle.isOn ? 1 : 0);
		PlayerPrefs.SetInt("powerSavingOn", powerSavingToggle.isOn ? 1 : 0);
	}




	public void ClearInput(Button button)
	{
		if(button.name == "hospital") hospitalInput.text = "";
		else if(button.name == "trainer") trainerInput.text = "";	
		StoreConfig();
	}




	public void StartButton()
	{
		StoreConfig();
		bool powerSavingMode = PowerSavingModeOn();
		int chosenPPE = ReturnChosenToggle();

		TitleViewController.instance.ToWelcomeView();
	}



	int ReturnChosenToggle()
	{
		if(basicToggle.isOn && enhancedToggle.isOn)
		{
			return 3;
		}
		else if(basicToggle.isOn && !enhancedToggle.isOn)
		{
			return 1;
		}
		else if(enhancedToggle.isOn && !basicToggle.isOn)
		{
			return 2;
		}
		else return 0;

	}



	bool PowerSavingModeOn()
	{
		if(powerSavingToggle.isOn) return true;
		else return false;
	}


	public void PPESwitchChanged(Toggle toggle) {
		//force at least one toggle on
		if (!basicToggle.isOn && !enhancedToggle.isOn) {
			if (toggle == basicToggle) enhancedToggle.isOn = true;
			else basicToggle.isOn = true;
		}
	}




}








