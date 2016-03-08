using UnityEngine;
using System.Collections;

public class ExitIntroduction : MonoBehaviour {

    public string NextSceneName;

    void OnEnable()
    {
		if (UILoading.instance != null) UILoading.instance.LoadScene(NextSceneName);
		else Application.LoadLevel(NextSceneName);
    }
}
