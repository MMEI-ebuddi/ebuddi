using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISplash : MonoBehaviour
{
	public Text			m_BuildText;
	public Button		m_EnglishButton;
	public Button		m_LiberianEnglishButton;
	public Button		m_PowerSaverButton;
	public Text			m_PowerSaveText;

	public GameObject	m_MainPanel;
	public GameObject	m_DisclaimerPanel;
	public Button		m_DisclaimerContinueButton;

	void Start()
	{
		SetLanguageUI();

		m_BuildText.text = string.Format("Alpha Build {0}.{1}.{2}", GameManager.sBuildVersion[0].ToString(), GameManager.sBuildVersion[1].ToString(), GameManager.sBuildVersion[2].ToString());
		m_MainPanel.SetActive(true);
		m_DisclaimerPanel.SetActive(false);
		m_DisclaimerContinueButton.gameObject.SetActive(false);

		SetPowerSaverUI();
	}

	void SetPowerSaverUI()
	{
		if (PlayerPrefs.GetInt("powersave") > 0)
		{
			m_PowerSaveText.text = "Power Saver ON";
			//			m_PowerSaverButton.Select();
			QualitySettings.vSyncCount = 2;
		}
		else
		{
			m_PowerSaveText.text = "Power Saver OFF";
			QualitySettings.vSyncCount = 1;
		}
	}

	void SetLanguageUI()
	{
		int		Language = PlayerPrefs.GetInt("language");

		if (Language == (int)eLanguage.English)
			m_EnglishButton.Select();
		else if (Language == (int)eLanguage.LiberianEnglish)
			m_LiberianEnglishButton.Select();
	}

	public void StartClicked()
	{
		m_MainPanel.SetActive(false);
		m_DisclaimerPanel.SetActive(true);
		StartCoroutine(ContinueCountdown());
	}

	public void PowerSaveClicked()
	{
		if (PlayerPrefs.GetInt("powersave") > 0)
			PlayerPrefs.SetInt("powersave", 0);
		else
			PlayerPrefs.SetInt("powersave", 1);

		SetPowerSaverUI();
		SetLanguageUI();
	}



	public void ContinueClicked()
	{
		if (UILoading.instance != null) UILoading.instance.LoadScene("Title");
		else Application.LoadLevel("Title");
	}


	IEnumerator ContinueCountdown()
	{
		if (!Application.isEditor) yield return new WaitForSeconds(5);
		m_DisclaimerContinueButton.gameObject.SetActive(true);
	}

	public void EnglishClicked()
	{
		PlayerPrefs.SetInt("language", (int)eLanguage.English);
	}

	public void LiberianClicked()
	{
		PlayerPrefs.SetInt("language", (int)eLanguage.LiberianEnglish);
	}
}
