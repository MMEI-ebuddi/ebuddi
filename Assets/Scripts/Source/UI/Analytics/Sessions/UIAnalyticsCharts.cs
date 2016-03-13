using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class UIAnalyticsCharts : MonoBehaviour {

    public GameObject Next;

    public UIBarChart ChartActions, ChartQuizes;

    private AnalyticsAnalyser m_Analyser;

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

    public void NextClicked()
    {        
        Next.SetActive(true);
        Next.GetComponent<UIAnalyticsCharts>().BuildCharts("Scene3", m_Analyser);
    }

    public void BuildCharts(string strSessionType, AnalyticsAnalyser analyser )
    {
        m_Analyser = analyser;

        BuildActionsChart(strSessionType, analyser);
        BuildQuizesChart(strSessionType, analyser);
    }
     

    private void BuildActionsChart(string strSessionType, AnalyticsAnalyser analyser)
    {
        Dictionary<string, int> entries = new Dictionary<string, int>();

        ChartData actionsData = new ChartData();

        foreach (string strUser in m_Analyser.GetUserNames())
        {
            foreach (AnalyticsSession session in analyser.GetSessionsForUser(strUser, strSessionType))
            {              
                foreach (string wrongAction in analyser.GetFailedItemsForSession(session))
                {
                    if (entries.ContainsKey(wrongAction))
                        entries[wrongAction]++;
                    else
                        entries.Add(wrongAction, 1);
                }                            
            }
        }

        actionsData.ChartTile = "Failed Actions";
        foreach (string key in entries.Keys)
        {
            actionsData.AddEntry(new ChartEntry(key, entries[key]));
        }

        ChartActions.BuildChart(actionsData);        
    }

    private void BuildQuizesChart(string strSessionType, AnalyticsAnalyser analyser)
    {
        Dictionary<string, int> entries = new Dictionary<string, int>();

        ChartData quizesData = new ChartData();

        foreach (string strUser in m_Analyser.GetUserNames())
        {
            foreach (AnalyticsSession session in analyser.GetSessionsForUser(strUser, strSessionType))
            {
                foreach (string wrongQuiz in analyser.GetFailedQuizesForSession(session))
                {
                    if (entries.ContainsKey(wrongQuiz))
                        entries[wrongQuiz]++;
                    else
                        entries.Add(wrongQuiz, 1);
                }
            }
        }

        quizesData.ChartTile = "Incorrectly Answered Questions";
        foreach (string key in entries.Keys)
        {
            quizesData.AddEntry(new ChartEntry(key, entries[key]));
        }

        ChartQuizes.BuildChart(quizesData);        
    }
}


  

