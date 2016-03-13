using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ChartData 
{
    public string ChartTile;

    private List<ChartEntry> m_Entries;

    public ChartData()
    {
        m_Entries = new List<ChartEntry>();
    }
    
    public void AddEntry(ChartEntry entry)
    {
        m_Entries.Add(entry);
    }

    public List<ChartEntry> GetEntries()
    {
        return m_Entries;
    }
}
