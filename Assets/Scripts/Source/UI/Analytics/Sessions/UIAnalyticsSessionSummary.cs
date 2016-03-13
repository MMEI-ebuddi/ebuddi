using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class UIAnalyticsSessionSummary : MonoBehaviour {

    public UIBarChart ChartActions, ChartQuizes;

    public Text TextName;
    public Text TextRuns;
    public Text TextFailures;
    public Text TextSuccesses;
    public Text TextPassRate;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ExitClicked()
    {
        gameObject.SetActive(false);
    }

    public void DisplaySession(AnalyticsSession session, AnalyticsAnalyser analyser)
    {
        BuildActionsChart(session, analyser);
        BuildQuizChart(session, analyser);
        
        SetupHeader(session, analyser);
    }

    private void BuildQuizChart(AnalyticsSession session, AnalyticsAnalyser analyser)
    {
        Dictionary<string, int> entries = new Dictionary<string, int>();

        foreach (string wrongQuiz in analyser.GetFailedQuizesForSession(session)) 
        {
            if (entries.ContainsKey(wrongQuiz))
                entries[wrongQuiz]++;
            else
                entries.Add(wrongQuiz, 1);
        }

        ChartData actionsData = new ChartData();
        actionsData.ChartTile = "Incorrectly Answered Questions";
        foreach (string key in entries.Keys)
        {
            actionsData.AddEntry(new ChartEntry(key, entries[key]));
        }

        ChartQuizes.BuildChart(actionsData);
    }

    private void BuildActionsChart(AnalyticsSession session, AnalyticsAnalyser analyser)
    {
        Dictionary<string, int> entries = new Dictionary<string, int>();

        foreach (string wrongAction in analyser.GetFailedItemsForSession(session))
        {
            if (entries.ContainsKey(wrongAction))
                entries[wrongAction]++;
            else
                entries.Add(wrongAction, 1);
        }

        ChartData actionsData = new ChartData();
        actionsData.ChartTile = "Failed Actions";
        foreach (string key in entries.Keys)
        {
            actionsData.AddEntry(new ChartEntry(key, entries[key]));
        }

        ChartActions.BuildChart(actionsData);
    }

    private void SetupHeader(AnalyticsSession session, AnalyticsAnalyser analyser)
    {
        TextName.text = session.GetSessionType() == "Scene1" ? "Donning" : "Doffing";

        int iTotal = analyser.GetNumberOfRunsForSession(session);
        int iFailures = analyser.GetNumberOfFailedRunsForSession(session);
        int iSuccesses = (iTotal - iFailures);

        TextRuns.text = iTotal.ToString();
        TextFailures.text = iFailures.ToString();
        TextSuccesses.text = iSuccesses.ToString();

        float fPerc = ((float)iSuccesses / (float)iTotal * 100f);

        if (iTotal > 0)
            TextPassRate.text = fPerc.ToString() + "%";
        else
            TextPassRate.text = "-";   
    }
}
