using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISurveyElementToggleAnswer : UISurveyElement {

    public Toggle ToggleAnswer;
    public Text TextAnswer;

	public override string GetData()
    {
        return TextAnswer.text;
    }

    public override bool Answered()
    {
        return ToggleAnswer.isOn;
    }

    public override void InitData(string strData)
    {
        // If our parent has a toggle group component, make that our toggle group
        if (transform.parent!=null)
        {
            ToggleGroup tg = transform.parent.GetComponent<ToggleGroup>();

            if (tg!=null)
            {
                ToggleAnswer.group = tg;
            }
        }

        TextAnswer.text = strData;
    }

    public override bool Selected()
    {
        return ToggleAnswer.isOn;
    }  
}
