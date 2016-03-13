using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;

public class Survey : MonoBehaviour {

    public TextAsset SurveyXMLFile;
    public UISurvey SurveyUI;
    
	// Use this for initialization
	void Start () {
        SurveyUI.SurveyToUse = this;

        LoadSurveyXML();
	}

    /// <summary>
    /// Parse a survey from an XML file. Each question in the survey can have a different response type with different data and varying number of response items.
    /// </summary>
    private void LoadSurveyXML()
    {        
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(SurveyXMLFile.text);
        XmlNodeList questionList = xmlDoc.GetElementsByTagName("question");

        // Loop through each question in the survey
        foreach (XmlNode question in questionList)
        {                        
            ParseQuestion(question);                       
        }

        // Set the first question as active
        SurveyUI.SetSelectedQuestionIndex(0);
    }  

    /// <summary>
    /// Parse an individual question and create and setup a UI panel for it
    /// </summary>
    /// <param name="questionNode"></param>
    private void ParseQuestion(XmlNode questionNode)
    {
        // Create a neq question panel
        GameObject questionPanel = SurveyUI.AddQuestionPanel();

        // Get the question panel script on the new panel
        UISurveyQuestionPanel qp = questionPanel.GetComponent<UISurveyQuestionPanel>();

        // Find the question text, TODO: Get rid of this loop.
        foreach (XmlNode contentItem in questionNode)
        {            
            // Question text
            if (contentItem.Name == "text")
            {
                qp.TextQuestion.text = contentItem.InnerText;                
            }                      
        }

        // Grab all the answer nodes
        XmlNodeList answerNodes = questionNode.SelectSingleNode("answers").SelectNodes("answer");
        
        // Parse each answer node
        foreach (XmlNode answerNode in answerNodes)
        {
            ParseAnswer(answerNode, qp);
        }        
    }

    /// <summary>
    /// Parse an individual answer, determining the input type and any child inputs it may have
    /// </summary>
    /// <param name="answerNode"></param>
    /// <param name="questionPanel"></param>
    private void ParseAnswer(XmlNode answerNode, UISurveyQuestionPanel questionPanel)
    {
        // Make sure we have a type 
        if (answerNode.Attributes["type"] !=null)        
        {
            string strAnswerType = answerNode.Attributes["type"].InnerText;

            UISurveyQuestionPanel.ANSWER_TYPE answerType = GetAnswerTypeFromString(strAnswerType);
            
            // Try and select the answer text node
            XmlNode answerTextNode = answerNode.SelectSingleNode("text");

            string strAnswerText = (answerTextNode != null) ? answerNode.InnerText : "UNDEFINED";          

            // Add a new answer input panel to questionPanel
            UISurveyElement answerElement = questionPanel.AddAnswer(answerType, strAnswerText);
            
            // See if this answer has a skip attribute which determines how many questions we need to skip if this answer is chosen
            if (answerNode.Attributes["skip"] != null)
            {
                if (answerElement!=null)
                {
                    answerElement.QuestionsToSkip = Convert.ToInt32(answerNode.Attributes["skip"].InnerText);
                }                                
            }
        }
    }

    private UISurveyQuestionPanel.ANSWER_TYPE GetAnswerTypeFromString(string strText)
    {
        if (strText.ToUpper() == "TOGGLE")
            return UISurveyQuestionPanel.ANSWER_TYPE.TOGGLE;
        else if (strText.ToUpper() == "TEXT")
            return UISurveyQuestionPanel.ANSWER_TYPE.TEXT;
        else if (strText.ToUpper() == "TOGGLE_TEXT")
            return UISurveyQuestionPanel.ANSWER_TYPE.TOGGLE_TEXT;

        return UISurveyQuestionPanel.ANSWER_TYPE.TOGGLE;
    }

	// Update is called once per frame
	void Update () {
	
	}
}
