using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UIGuidedInputField : MonoBehaviour 
{
	public Text inputLabel;
	public Image activeImage;
	public InputField input;
	public Color inactiveColor = new Color32(167, 169, 172, 255);
	public Color activeColor = Color.white;
	public bool isGuidedField = false;

	public void SetFrame(bool isActive)
	{
		if (!isGuidedField) return;

		if(isActive)
		{
			activeImage.gameObject.SetActive(true);
			input.Select();
			input.ActivateInputField();
			MightBlink();
		}
		else
		{
			activeImage.gameObject.SetActive(false);
			AnimateAlpha(0);
		}
	}

	void Update () {
		if (input.isFocused) {
			if (inputLabel.color != activeColor) inputLabel.color = activeColor;
		} else {
			if (inputLabel.color != inactiveColor) inputLabel.color = inactiveColor;
		}
	}





	public bool IsFilled()
	{
		return (!string.IsNullOrEmpty(input.text));
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
		if(!input.isFocused || !IsFilled()) 
		{
			StartCoroutine(Blink());
		}
	}


}
