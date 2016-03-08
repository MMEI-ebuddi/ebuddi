using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ViewOptionsAdmin : UIView
{
	public Toggle subtitlesToggle;
	public Toggle heatmapToggle;
	
	
	public void OKButton()
	{
		Hide();
		if(subtitlesToggle.isOn) Debug.Log("Subtitles On");
		else Debug.Log("Subtitles Off");
		
		if(heatmapToggle.isOn) Debug.Log("Heatmap On");
		else Debug.Log("Heatmap Off");
	}
	



	public void PostAnalyticsButton()
	{
		Debug.Log("Post Analytics button");
	}


	public void ViewAnalyticsButton()
	{
		Debug.Log("View Analytics button");
	}


}
