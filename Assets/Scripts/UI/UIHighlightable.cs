using UnityEngine;
using System.Collections;

public class UIHighlightable : MonoBehaviour {

	public bool isHighlighted = false;



	public virtual void Awake() {
		UnHighlight();
	}

	public virtual void Highlight() {
		isHighlighted = true;
	}

	public virtual void UnHighlight() {
		isHighlighted = false;
	}



	public virtual void Update() {



	}


}
