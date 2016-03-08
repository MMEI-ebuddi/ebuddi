//Greg Lukosek 2015
//support, lukos86@gmail.com

using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public enum AnimationDirection {
	left,
	right
}


[RequireComponent(typeof(Camera))]

public class UIView : MonoBehaviour {
	
	private Camera cam;
	private Canvas[] canvases;


	public virtual void Awake() {
		cam = this.GetComponent<Camera>();
		canvases = this.GetComponentsInChildren<Canvas>();


		Application.targetFrameRate = 60;
	}


	public virtual void Show()
	{
		if (this.GetComponent<CanvasGroup>() != null) {
			this.GetComponent<CanvasGroup>().alpha = 1f;
			this.GetComponent<CanvasGroup>().blocksRaycasts = true;;
			this.GetComponent<CanvasGroup>().interactable = true;
		}

		ViewWillAppear();
		GameObject mainCanvas = this.transform.FindChild("UICanvas").gameObject;
		GameObject viewContent = mainCanvas.transform.FindChild("ViewContent").gameObject;
		viewContent.transform.localPosition = Vector3.zero;

		cam.enabled = true;
		foreach (Canvas canvas in canvases) canvas.enabled = true;

		Canvas.ForceUpdateCanvases();
	}




	public virtual void ShowAnimated(AnimationDirection sourceDirection)
	{
		if (this.GetComponent<CanvasGroup>() != null) {
			this.GetComponent<CanvasGroup>().alpha = 1f;
			this.GetComponent<CanvasGroup>().blocksRaycasts = true;;
			this.GetComponent<CanvasGroup>().interactable = true;
		}

		cam.enabled = true;
		foreach (Canvas canvas in canvases) canvas.enabled = true;

		GameObject mainCanvas = this.transform.FindChild("UICanvas").gameObject;
		GameObject viewContent = mainCanvas.transform.FindChild("ViewContent").gameObject;
		RectTransform mainCanvasRectTransform = (RectTransform)mainCanvas.transform;

		if (sourceDirection == AnimationDirection.right) {
			viewContent.transform.localPosition = new Vector3(mainCanvasRectTransform.rect.width,
			                                                 mainCanvasRectTransform.localPosition.y,
			                                                 mainCanvasRectTransform.localPosition.z);
		}
		else 
		{
			viewContent.transform.localPosition = new Vector3(-mainCanvasRectTransform.rect.width,
			                                                 mainCanvasRectTransform.localPosition.y,
			                                                 mainCanvasRectTransform.localPosition.z);
		}

		ViewWillAppear();
		iTween.MoveTo(viewContent.gameObject, iTween.Hash("x", 0, "time", 0.33f, "islocal", true));
	}




	public virtual void HideAnimated(AnimationDirection getAwayDirection)
	{
		cam.enabled = true;
		foreach (Canvas canvas in canvases) canvas.enabled = true;
		
		Canvas.ForceUpdateCanvases();
		
		GameObject mainCanvas = this.transform.FindChild("UICanvas").gameObject;
		GameObject viewContent = mainCanvas.transform.FindChild("ViewContent").gameObject;
		RectTransform mainCanvasRectTransform = (RectTransform)mainCanvas.transform;

		mainCanvas.transform.localPosition = new Vector3(0,
		                                                 mainCanvasRectTransform.localPosition.y,
		                                                 mainCanvasRectTransform.localPosition.z);
		
		if (getAwayDirection == AnimationDirection.right) {
			iTween.MoveTo(viewContent.gameObject, iTween.Hash("x", -mainCanvasRectTransform.rect.width, "time", 0.33f, "islocal", true));

		}
		else 
		{
			iTween.MoveTo(viewContent.gameObject, iTween.Hash("x", mainCanvasRectTransform.rect.width, "time", 0.33f, "islocal", true));
		}

		ViewWillDisappear();
		Invoke("Hide", 0.33f);
	}



	#region Fading

	public virtual void FadeIn(float fadeTime) {

		if (GetComponent<CanvasGroup>() == null) {
			Debug.LogError("Canvas Group component is missing, add it to the view to use fade");
			return;
		}

		GetComponent<CanvasGroup>().alpha = 0;

		ViewWillAppear();
		GameObject mainCanvas = this.transform.FindChild("UICanvas").gameObject;
		GameObject viewContent = mainCanvas.transform.FindChild("ViewContent").gameObject;
		viewContent.transform.localPosition = Vector3.zero;
		
		cam.enabled = true;
		foreach (Canvas canvas in canvases) canvas.enabled = true;
		
		Canvas.ForceUpdateCanvases();

		iTween.ValueTo(this.gameObject, iTween.Hash("from", GetComponent<CanvasGroup>().alpha, "to", 1f, "time", fadeTime, "onupdate", "AnimateAlpha"));
	}



	public virtual void FadeOut(float fadeTime) {

		if (GetComponent<CanvasGroup>() == null) {
//			Debug.LogError(this.name +  " Canvas Group component is missing, add it to the view to use fade");
			return;
		}


		ViewWillDisappear();

		iTween.ValueTo(this.gameObject, iTween.Hash("from", GetComponent<CanvasGroup>().alpha, "to", 0, "time", fadeTime, "onupdate", "AnimateAlpha"));
	}



	void AnimateAlpha(float newValue) {

		CanvasGroup cg = GetComponent<CanvasGroup>();

		cg.alpha = newValue;

		cg.interactable = (newValue == 1f);
		cg.blocksRaycasts = (newValue == 1f);

		//hide 
		if (newValue == 0) {
			cam.enabled = false;
			foreach (Canvas canvas in canvases) canvas.enabled = false;
		}
		else {
			cam.enabled = true;
			foreach (Canvas canvas in canvases) canvas.enabled = true;
		}

		Canvas.ForceUpdateCanvases();
	}


	#endregion





	public virtual void Hide()
	{
		ViewWillDisappear();

		cam.enabled = false;
		foreach (Canvas canvas in canvases) canvas.enabled = false;

		Canvas.ForceUpdateCanvases();
//		if (this.GetComponent<CanvasGroup>() != null) {
//			Canvas canvas = ((Canvas)this.GetComponent<CanvasGroup>());
//			canvas.enabled = false;
//		}
	}





	public virtual void ViewWillAppear() {

	}

	public virtual void ViewWillDisappear() {
		
	}
	
}











