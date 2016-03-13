using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIAnalyticsSessionEntry : MonoBehaviour {

    public Text TextName, TextRuns, TextSuccesses, TextFailures, TextPassRate;
    public UIAnalyticsSessionList Table;
    public AnalyticsSession Session;

    public void EntryClicked()
    {
        Table.SessionEntryClicked(Session);
    }
    
}
