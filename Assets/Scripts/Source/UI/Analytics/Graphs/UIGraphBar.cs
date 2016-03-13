using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIGraphBar : MonoBehaviour {

    public Text TextHeader;
    public Text TextValue;
    public Image ImageBar;
    
	// Use this for initialization
	void Start () {
           
	}
	

	public void Init(string strHeader, string strValue, float fPercent)
    {
        ImageBar.fillAmount = fPercent;

        TextHeader.text = strHeader;
        TextValue.text = strValue;     
    }
}
