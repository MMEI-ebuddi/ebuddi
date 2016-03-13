using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UISurveyQuestionPanel : MonoBehaviour {

    public Text TextQuestion;
    public Transform AnswersPanel;

    public Transform PrefabToggleAnswer;

    public Transform[] AnswerInputPrefabs;

    private List<UISurveyElement> m_SurveyElements;

    private int m_iPreviousQuestionIndex = 0;

    public enum ANSWER_TYPE
    {
        TOGGLE,
        TEXT,
        TOGGLE_TEXT,
    };
    
    void Awake()
    {
        m_SurveyElements = new List<UISurveyElement>();
    }

    /// <summary>
    /// Add a new answer item to this question panel. This could be a toggle, text input, etc.
    /// </summary>
    /// <param name="answerType"></param>
    /// <param name="strAnswerText"></param>
    public UISurveyElement AddAnswer(ANSWER_TYPE answerType, string strAnswerText)
    {
        Transform inputPrefab = Instantiate(AnswerInputPrefabs[(int)answerType]);
        inputPrefab.SetParent(AnswersPanel, false);

        UISurveyElement surveyElement = inputPrefab.GetComponent<UISurveyElement>();

        surveyElement.InitData(strAnswerText);

        m_SurveyElements.Add(surveyElement);

        return surveyElement;
    }    

    /// <summary>
    /// How many questions should we advance when we move on from this question? Defaults to 1 and adds on if a selectable UISurveyElement is selected and has a Skip value specified
    /// </summary>
    /// <returns></returns>
    public int QuestionsToAdvance()
    {
        // Always advance at least one question
        int iSkip = 1;

        // Check all the survey elements for this question and see if selected, if so check if they have a Skip value specified and add it on.
        // There may be multiple toggle groups etc eventually, so we need to check every element to see if it's selected independently.
        foreach (UISurveyElement surveyElement in m_SurveyElements)
        {
            if (surveyElement.IsSelectable() && surveyElement.Selected())
            {
                iSkip += surveyElement.QuestionsToSkip;
            }
        }

        return iSkip;
    }

    /// <summary>
    /// Check all the survey elements and see if anything has been input
    /// </summary>
    /// <returns></returns>
    public bool Answered()
    {
        foreach (UISurveyElement surveyElement in m_SurveyElements)
        {
            if (surveyElement.Answered())
                return true;
        }

        return false;
    }

    /// <summary>
    /// Used to skip backwards to the right question when we've skipped multiple questions out
    /// </summary>
    /// <param name="iIndex"></param>
    public void SetPreviousQuestionIndex(int iIndex)
    {
        m_iPreviousQuestionIndex = iIndex;
    }
    
    public int PreviousQuestionIndex()
    {
        return m_iPreviousQuestionIndex;
    }

    public string GetXML()
    {
        string strXML = "<Question>\n"
            + "<text>" + TextQuestion.text + "</text>\n";

        foreach (UISurveyElement surveyElement in m_SurveyElements)
        {
            if (surveyElement.IsSelectable() && surveyElement.Selected() || !surveyElement.IsSelectable())
            {
                strXML += "<answer>" + surveyElement.GetData() + "</answer>\n";
            }
        }

        strXML += "</Question>\n";

        return strXML;
    }
}
