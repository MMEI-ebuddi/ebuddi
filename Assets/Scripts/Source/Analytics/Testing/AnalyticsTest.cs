using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class AnalyticsTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        AnalyticsAnalyser analyser = new AnalyticsAnalyser();

        foreach (string strUser in analyser.GetUserNames())
        {
            Debug.Log("Runs for " + strUser + ": " + analyser.GetTotalRunsForUser(strUser,"Scene1").ToString());
            Debug.Log("Failed runs for " + strUser + ": " + analyser.GetTotalNumberOfFailedRunsForUser(strUser, "Scene1").ToString());
        }
	}		
}
