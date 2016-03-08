using System;
using UnityEngine;
using UnityEngine.UI;
using TMSupport;

public class UIProgressBar : MonoBehaviour
{
	static float				sProgressPercentage;
	
    public Image                BarFill;
		
	void Update()
	{
        BarFill.fillAmount = sProgressPercentage;
	}

	internal static void SetProgressPercentage(float ProgressPercentage)
	{        
        sProgressPercentage = ProgressPercentage / 100f;
	}
}
