using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIAnalyticsTable : MonoBehaviour {

    public Transform PrefabHeader;
    public Transform PrefabEntry;

    public GameObject EntryTable;

    public GameObject Root;

    public GameObject EntrySummaryPanel;

    public GameObject ChartsScreen;

    private AnalyticsAnalyser m_Analyser;

	// Use this for initialization
	void Start () {
        Transform header = Instantiate(PrefabHeader) as Transform;
        header.SetParent(EntryTable.transform, false);

        m_Analyser = new AnalyticsAnalyser();

//        Dictionary<float, Transform> entries = new Dictionary<float, Transform>();
        
        // Create and setup all the table entries into a dictionary
        foreach (string strUser in m_Analyser.GetUserNames())
        {
            Transform entry = Instantiate(PrefabEntry) as Transform;         

            UIAnalyticsTableEntry uiEntry = entry.GetComponent<UIAnalyticsTableEntry>();

            uiEntry.Table = this;

            int iTotal = (m_Analyser.GetTotalRunsForUser(strUser, "Scene1") + m_Analyser.GetTotalRunsForUser(strUser, "Scene3"));
            int iFailures = (m_Analyser.GetTotalNumberOfFailedRunsForUser(strUser, "Scene1") + m_Analyser.GetTotalNumberOfFailedRunsForUser(strUser, "Scene3"));
            int iSuccesses = (iTotal - iFailures);

            uiEntry.TextName.text = strUser;
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

            entry.SetParent(EntryTable.transform, false);
          //  entries.Add(fPerc, entry);
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
	
	public void ExitButtonClicked()
    {
        Destroy(Root);
    }

    public void UserEntryClicked(string strUser)
    {
        EntrySummaryPanel.SetActive(true);
        EntrySummaryPanel.GetComponent<UIAnalyticsSessionList>().DisplayUser(strUser, m_Analyser);

        //EntrySummaryPanel.GetComponent<Animator>().SetTrigger("SlideOn");
        //GetComponent<Animator>().SetTrigger("SlideOff");
    }

    public void ChartsClicked()
    {
        ChartsScreen.SetActive(true);
        ChartsScreen.GetComponent<UIAnalyticsCharts>().BuildCharts("Scene1", m_Analyser);
    }
}
