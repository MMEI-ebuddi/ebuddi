using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIAnalyticsTableEntry : MonoBehaviour {

    public Text TextName, TextRuns, TextSuccesses, TextFailures, TextPassRate;
    public UIAnalyticsTable Table;
    
    public void EntryClicked()
    {
        Table.UserEntryClicked(TextName.text);
    }
}
