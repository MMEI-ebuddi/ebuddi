using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UILoading : UIView {

	public static UILoading instance;



	public override void Awake() {
		instance = this;
		DontDestroyOnLoad(this.gameObject.transform);

		base.Awake();

		Hide();
	}

	public void LoadScene (string sceneName) {
		StartCoroutine(LoadSceneAsync(sceneName));
	}
	


	IEnumerator LoadSceneAsync(string sceneName) {

		Show();

		AsyncOperation loading = Application.LoadLevelAsync(sceneName);
		yield return loading;

		Hide();
	}


}
