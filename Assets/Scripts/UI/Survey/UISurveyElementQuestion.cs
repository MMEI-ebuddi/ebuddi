using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISurveyElementQuestion : UISurveyElement {

    public Text TextQuestion;

	public override string GetData()
    {
        return TextQuestion.text;
    }
}
