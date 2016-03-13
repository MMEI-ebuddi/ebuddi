using UnityEngine;
using System.Collections;
using UnityEngine.UI;



public class UIProgressBarSlider : MonoBehaviour 
{

	public Image fill;

	public Image backgroundFill;
	public Image icon;
	public Sprite greyIcon;
	public Sprite yellowIcon;
	public Sprite greenIcon;
	public ModuleType moduleType;




	public void SetValue(float value, bool animated)
	{
		if (animated) {
			AnimateValue(value);
		}
		else {
			//not animated
			UpdateSliderValue(value);
		}

		if(value == 0) backgroundFill.color = new Color(147f/255f, 149f/255f, 152f/255f);
		else if(value > 0) backgroundFill.color = new Color(254f/255f, 192f/255f, 15f/255f);
	}



	void AnimateValue(float value)
	{
		iTween.ValueTo(this.gameObject, iTween.Hash("from", fill.fillAmount, "to", value, "time", 0.35f, "onupdate", "UpdateSliderValue"));
	}



	void UpdateSliderValue(float newValue)
	{
		fill.fillAmount = newValue;

		if (newValue == 0) icon.sprite = greyIcon;
		else if (newValue > 0 && newValue < 1f) icon.sprite = yellowIcon;
		else {
			icon.sprite = greenIcon;
		} 
	}

}
