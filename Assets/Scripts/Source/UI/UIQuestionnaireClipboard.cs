using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIQuestionnaireClipboard : MonoBehaviour {


	public Image tint;


	public bool isClipboardVisible {
		get {
			return (this.gameObject.transform.localPosition.y == 0);
		}
	}



	void Start() {
		Hide(false);
//		Show(true);
		UpdateTintAlpha(0);
	}




	public void Show(bool animated) {

		iTween.Stop(this.gameObject);
		iTween.MoveTo(this.gameObject, iTween.Hash("y", 0, "time", 0.33f, "islocal", true));
		iTween.ValueTo(this.gameObject, iTween.Hash("from", 0, "to", 0.45f, "time", 0.33f, "onupdate", "UpdateTintAlpha"));
	}


	public void Hide(bool animated) {

		if (animated) {
			iTween.MoveTo(this.gameObject, iTween.Hash("y", -800f, "time", 0.33, "islocal", true));
			iTween.ValueTo(this.gameObject, iTween.Hash("from", tint.color.a, "to", 0f, "time", 0.33f, "onupdate", "UpdateTintAlpha"));
		}
		else {
			this.gameObject.transform.localPosition = new Vector3(this.gameObject.transform.localPosition.x,
			                                                      -800f,
			                                                      this.gameObject.transform.localPosition.z);
			UpdateTintAlpha(0);
		}

	}


	void UpdateTintAlpha(float newValue) {
		tint.color = new Color(tint.color.r, tint.color.g, tint.color.b, newValue);
		tint.raycastTarget = newValue > 0;
	}

}
