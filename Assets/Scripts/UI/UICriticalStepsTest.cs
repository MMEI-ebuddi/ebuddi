using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UICriticalStepsTest : MonoBehaviour {

	public UICriticalSteps uiCriticalSteps;
	public List<Texture2D> testList = new List<Texture2D>();


	void Start() {

		uiCriticalSteps.Init(testList, true);
		uiCriticalSteps.ShowNext();
	}

	


}






