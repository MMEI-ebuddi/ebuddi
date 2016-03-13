using UnityEngine;
using System.Collections;

public class UIHighlightableSceneObject : UIHighlightable {

	public string initialLayerName;


	void Awake() {
		initialLayerName = LayerMask.LayerToName(gameObject.layer);
	}


	public override void Highlight() {

		this.gameObject.layer = LayerMask.NameToLayer("3DHighlighted");

		base.Highlight();
	}


	public override void UnHighlight() {

		this.gameObject.layer = LayerMask.NameToLayer(initialLayerName);
		
		base.UnHighlight();
	}

}
