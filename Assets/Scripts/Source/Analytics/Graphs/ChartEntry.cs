using UnityEngine;
using System.Collections;

public class ChartEntry 
{
    public string Header;
    public int Value;

    public ChartEntry(string strHeader, int iValue)
    {
        Header = strHeader;
        Value = iValue;        
    }
}
