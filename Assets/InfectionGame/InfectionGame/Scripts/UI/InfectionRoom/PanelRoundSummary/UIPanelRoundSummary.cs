using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPanelRoundSummary : MonoBehaviour {

    public RectTransform Panel;

    public GameObject ButtonNext, ButtonRetry, ButtonExit;

    public Text TextInfectionCount;

    public string SceneToExitTo;

    void Start()
    {
       
    }

    public void NextRoundButtonClicked()
    {
        RoundManager.Instance.NextRound();
        Hide();
    }

    public void RetryButtonClicked()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void ExitButtonClicked()
    {
		if (UILoading.instance != null) UILoading.instance.LoadScene(SceneToExitTo);
		else Application.LoadLevel(SceneToExitTo);
    }

	public void Show(int iRound)
    {       
        Panel.gameObject.SetActive(true);

        ButtonNext.SetActive(iRound == 0 ? true : false);
        ButtonRetry.SetActive(iRound == 1 ? true : false);
        ButtonExit.SetActive(iRound == 1 ? true : false);

        int iInfectionCount = RoundManager.Instance.GetInfectedCount();

        if (iInfectionCount==1)
            TextInfectionCount.text = RoundManager.Instance.GetInfectedCount().ToString() + " person was infected with Ebola!";
        else
            TextInfectionCount.text = RoundManager.Instance.GetInfectedCount().ToString() + " people were infected with Ebola!";
    }

    public void Hide()
    {
        Panel.gameObject.SetActive(false);
    }
}
