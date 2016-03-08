using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnalyticsAction
{
    public string Expected, Guessed;
    public float Time;
    public int Run;
	public Dictionary<string, string> additionalAttributes = new Dictionary<string, string>();

    public AnalyticsAction(string strExpected, string strGuessed, float fTime, int iRun, Dictionary<string, string> pAdditionalAttributes = null)
    {
        Expected = strExpected;
        Guessed = strGuessed;
        Time = fTime;
        Run = iRun;
		if (pAdditionalAttributes != null) additionalAttributes = pAdditionalAttributes;
    }
}