using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ViewError : UIView 
{

	public Text errorTitle;
	public Text errorMessage;
	


	public void ShowErrorView(string title, string message)
	{
		errorTitle.text = title;
		errorMessage.text = message;
		Show();
	}
	



	public void OkButton()
	{
		Hide();
	}




}
