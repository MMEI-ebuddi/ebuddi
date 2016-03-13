using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ViewWelcome : UIView 
{

	public Toggle englishLang;
	public Toggle liberianLang;



	public override void ViewWillAppear() 
	{
		base.ViewWillAppear();
	}
	

	
	public override void ViewWillDisappear() 
	{
		base.ViewWillDisappear();
	}





	public void SettingsButton()
	{
		TitleViewController.instance.ToConfigView();
	}



	public void StartButton()
	{
//		if(englishLang.isOn)  Debug.Log("English language chosen");
//		else if(liberianLang.isOn) Debug.Log("Liberian language chosen");

		//normal english not surrpoted now

		PlayerPrefs.SetInt("language", (int)eLanguage.LiberianEnglish);

		TitleScene titleScene = (TitleScene)GameManager.Instance.Scene;
		StartCoroutine(titleScene.StartTitleScene());
		TitleViewController.instance.ToViewTitleScene();

	}




}
