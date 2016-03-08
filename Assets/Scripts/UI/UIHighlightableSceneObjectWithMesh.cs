using UnityEngine;
using System.Collections;

public class UIHighlightableSceneObjectWithMesh : UIHighlightable {

	public GameObject highlightMesh;



	public override void Highlight() {

		highlightMesh.SetActive(true);

		base.Highlight();
	}


	public override void UnHighlight() {

		highlightMesh.SetActive(false);
		
		base.UnHighlight();
	}

}
