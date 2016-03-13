using UnityEngine;
using System.Collections;

public class SkipIntroductionButton : MonoBehaviour {

    public string SceneToLoad;

	public void SkipButtonClicked()
    {
		if (UILoading.instance != null) UILoading.instance.LoadScene(SceneToLoad);
		else Application.LoadLevel(SceneToLoad);
    }
}
