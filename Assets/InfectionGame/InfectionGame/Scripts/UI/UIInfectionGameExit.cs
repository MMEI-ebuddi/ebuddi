using UnityEngine;
using System.Collections;

public class UIInfectionGameExit : MonoBehaviour {

    public string SceneToExitTo = "Title";

	public void ExitButtonClicked()
    {
		if (UILoading.instance != null) UILoading.instance.LoadScene(SceneToExitTo);
		else Application.LoadLevel(SceneToExitTo);
    }
}
