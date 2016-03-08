using UnityEngine;

public class InfectionToTitle : MonoBehaviour
{
	public void OnClicked()
	{
		if (UILoading.instance != null) UILoading.instance.LoadScene("Title");
		else Application.LoadLevel("Title");
	}
}
