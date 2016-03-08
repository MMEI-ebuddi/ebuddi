using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISurveyElementToggleTextInputAnswer : UISurveyElement {

    public Toggle ToggleAnswer;
    public Text TextAnswer;
    public InputField TextInputField;

	public override string GetData()
    {
        return TextInputField.text;
    }

    public override bool Answered()
    {
        return TextInputField.text.Length != 0 && ToggleAnswer.isOn;
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

    void Awake()
    {
        TextInputField.interactable = false;
    }

    public override bool Selected()
    {
        return ToggleAnswer.isOn;
    }

    void Update()
    {
        TextInputField.interactable = ToggleAnswer.isOn;       
    }
}
