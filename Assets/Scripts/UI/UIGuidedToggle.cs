using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIGuidedToggle : MonoBehaviour 
{


	public Image activeImage;
	public ToggleGroup sexToggles;




	public void UpdateToggle(bool isActive)
	{
		if(isActive)
		{
			activeImage.gameObject.SetActive(true);
			MightBlink();
		}
		else
		{
			activeImage.gameObject.SetActive(false);
			AnimateAlpha(0);
		}
	}



	public bool IsSet()
	{
		if(!sexToggles.AnyTogglesOn()) return false;
		else return true;
	}
	
	
	IEnumerator Blink() 
	{
		iTween.Stop(this.gameObject);
		iTween.ValueTo(this.gameObject, iTween.Hash("from", 0, "to", 1f, "time", 0.4f, "onupdate", "AnimateAlpha"));
		yield return new WaitForSeconds(0.4f);
		iTween.ValueTo(this.gameObject, iTween.Hash("from", 1f, "to", 0, "time", 0.4f, "onupdate", "AnimateAlpha", "oncomplete", "MightBlink"));
	}
	
	
	void AnimateAlpha(float alpha) 
	{
		activeImage.color = new Color(activeImage.color.r, activeImage.color.g, activeImage.color.b, alpha);
	}
	
	
	
	void MightBlink()
	{
		if(!IsSet()) 
		{
			StartCoroutine(Blink());
		}
	}







}
