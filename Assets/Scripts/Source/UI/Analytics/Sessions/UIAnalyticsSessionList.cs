using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIAnalyticsSessionList : MonoBehaviour {

    public Text TextUserName;

    public Transform PrefabHeader;
    public Transform PrefabEntry;

    public GameObject EntryTable;

    public GameObject SessionSummaryPanel;

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

        //EntrySummaryPanel.GetComponent<Animator>().SetTrigger("SlideOn");
        //GetComponent<Animator>().SetTrigger("SlideOff");
    }

    private void ClearList()
    {
        foreach (Transform child in EntryTable.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void DisplayUser(string strUser, AnalyticsAnalyser analyser)
    {
        ClearList();

        m_Analyser = analyser;

        TextUserName.text = strUser + "'s Sessions";

        Transform header = Instantiate(PrefabHeader) as Transform;
        header.SetParent(EntryTable.transform, false);       
        
        Dictionary<float, Transform> entries = new Dictionary<float, Transform>();

        List<AnalyticsSession> sessions = analyser.GetSessionsForUser(strUser);

        foreach (AnalyticsSession session in sessions)
        {
            Transform sessionEntry = Instantiate(PrefabEntry) as Transform;

            UIAnalyticsSessionEntry uiEntry = sessionEntry.GetComponent<UIAnalyticsSessionEntry>();

            uiEntry.Table = this;
            uiEntry.Session = session;

            uiEntry.TextName.text = session.GetSessionType() == "Scene1" ? "Donning" : "Doffing";
            
            int iTotal = analyser.GetNumberOfRunsForSession(session);
            int iFailures = analyser.GetNumberOfFailedRunsForSession(session);
            int iSuccesses = (iTotal - iFailures);
            
            uiEntry.TextRuns.text = iTotal.ToString();
            uiEntry.TextFailures.text = iFailures.ToString();
            uiEntry.TextSuccesses.text = iSuccesses.ToString();

            float fPerc = ((float)iSuccesses / (float)iTotal * 100f);

            if (iTotal > 0)
                uiEntry.TextPassRate.text = fPerc.ToString() + "%";
            else
                uiEntry.TextPassRate.text = "-";

            uiEntry.TextName.color = fPerc > 50.0f ? Color.white : Color.red;
            uiEntry.TextRuns.color = fPerc > 50.0f ? Color.white : Color.red;
            uiEntry.TextFailures.color = fPerc > 50.0f ? Color.white : Color.red;
            uiEntry.TextPassRate.color = fPerc > 50.0f ? Color.white : Color.red;
            uiEntry.TextSuccesses.color = fPerc > 50.0f ? Color.white : Color.red;

            entries.Add(fPerc, sessionEntry);

            sessionEntry.SetParent(EntryTable.transform, false);
        }


        //// Sort the dictionary keys (passed %)
        //var list = entries.Keys.ToList();
        //list.Sort();

        //// Now add the items in sorted order to the table
        //foreach (var key in list)
        //{
        //    entries[key].SetParent(EntryTable.transform, false);
        //}        
    }

    public void SessionEntryClicked(AnalyticsSession session)
    {
        SessionSummaryPanel.SetActive(true);
        SessionSummaryPanel.GetComponent<UIAnalyticsSessionSummary>().DisplaySession(session, m_Analyser);

        //SessionSummaryPanel.GetComponent<Animator>().SetTrigger("SlideOn");
        //GetComponent<Animator>().SetTrigger("SlideOff");
    }
}
