using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ViewOptionsUser : UIView
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


}
