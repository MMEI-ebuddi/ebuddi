using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIBarChart : MonoBehaviour {

    public Text TextTitle;
    public Transform PrefabBar;
    public Transform ItemsPanel;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void Clear()
    {
        foreach (Transform child in ItemsPanel)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void BuildChart(ChartData chart)
    {
        Clear();

        TextTitle.text = chart.ChartTile;

        Dictionary<int, string> entries = new Dictionary<int, string>();

        // Find the largest value entry in the chart
        int iLargest = 0;      
  
        foreach (ChartEntry entry in chart.GetEntries())
        {
            if (entry.Value > iLargest)
                iLargest = entry.Value;

            Debug.Log("Adding " + entry.Header);
          //  entries.Add(entry.Value, entry.Header);
        }
      
        // Now sort the entires based on value (then reverse it to give us highest first)
        //var list = entries.Keys.ToList();
        //list.Sort();
        //list.Reverse();

        //// ...and add the items in sorted order to the table
        //foreach (var key in list)
        //{
        //    Transform bar = Instantiate(PrefabBar) as Transform;
        //    UIGraphBar graphBar = bar.GetComponent<UIGraphBar>();

        //    graphBar.Init(entries[key], key.ToString(), (float)key / (float)iLargest);
        //    bar.SetParent(ItemsPanel, false);
        //}

        foreach (ChartEntry entry in chart.GetEntries())
        {
            Transform bar = Instantiate(PrefabBar) as Transform;
            UIGraphBar graphBar = bar.GetComponent<UIGraphBar>();

            graphBar.Init(entry.Header, entry.Value.ToString(), (float)entry.Value / (float)iLargest);
            bar.SetParent(ItemsPanel, false);
        }
    }
}
