using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISurveyElementTextInputAnswer : UISurveyElement {

    public Text TextAnswerHeader;
    public Text TextInput;

	public override string GetData()
    {
        return TextInput.text;
    }

    public override void InitData(string strData)
    {
        TextAnswerHeader.text = strData;
    }

    public override bool Answered()
    {
        return TextInput.text.Length != 0;
    }
}
