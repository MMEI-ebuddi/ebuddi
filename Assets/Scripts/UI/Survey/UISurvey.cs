using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class UISurvey : MonoBehaviour {
    public Transform PanelContainer;
    public Transform PrefabQuestionPanel;
    public Text TextProgressTracker;

    public GameObject ButtonNext, ButtonPrevious;

    private List<GameObject> m_QuestionPanels;

    private int m_iActiveQuestionIndex = 0;

    public string NextSceneName;

    public Survey SurveyToUse;

    void Awake()
    {        
        m_QuestionPanels = new List<GameObject>();

        //AnalyticsData.Instance.PostAllUserData();
    }
  
    /// <summary>
    /// Add a new question panel as a child of this panel and store it in our list of question panels
    /// </summary>
    /// <returns></returns>
    public GameObject AddQuestionPanel()
    {
        Transform questionPanel = Instantiate(PrefabQuestionPanel);
        questionPanel.SetParent(PanelContainer, false);
        questionPanel.localPosition = Vector3.zero;

        m_QuestionPanels.Add(questionPanel.gameObject);

        return questionPanel.gameObject;
    }
    
    /// <summary>
    /// Change the selected panel index from the current one by iVal, typically -1 or 1 to cycle through the questions
    /// </summary>
    private void ModifySelectedQuestionIndex(int iVal)
    {
        // Make sure the new index is valid

        if (m_iActiveQuestionIndex+iVal >= m_QuestionPanels.Count)
        {
            SurveyCompleted();
            return;
        }

        m_iActiveQuestionIndex = Mathf.Clamp(m_iActiveQuestionIndex + iVal, 0, m_QuestionPanels.Count - 1);

        SetSelectedQuestionIndex(m_iActiveQuestionIndex);
    }

    /// <summary>
    /// Set which question panel should be currently displayed and hide all the others
    /// </summary>
    /// <param name="iIndex"></param>
    public void SetSelectedQuestionIndex(int iIndex)
    {
        m_iActiveQuestionIndex = Mathf.Clamp(iIndex, 0, m_QuestionPanels.Count - 1);

        foreach (GameObject questionPanel in m_QuestionPanels)
        {
            questionPanel.SetActive(false);
        }

        m_QuestionPanels[m_iActiveQuestionIndex].SetActive(true);

        // Update display of survey progress
        TextProgressTracker.text = "Question " + (m_iActiveQuestionIndex + 1) + " of " + m_QuestionPanels.Count;

        // Set visibility of next/previous questions based on if we're at either end of the survey
        ButtonPrevious.SetActive(m_iActiveQuestionIndex == 0 ? false : true);        
    }

    /// <summary>
    /// Called from the UI system when the NEXT QUESTION button is pressed
    /// </summary>
    public void NextButtonClicked()
    {      
        UISurveyQuestionPanel questionPanel = GetCurrentQuestionPanelScript();

        if (questionPanel==null)
        {
            Debug.Log("ERROR! - NextButtonClicked() - no question panel script found!");
            return;            
        }

        if (!questionPanel.Answered())
            return;

        int iAdvancemenet = questionPanel.QuestionsToAdvance();

        int iThisQuestionIndex = m_iActiveQuestionIndex;

        // Move to the next question
        ModifySelectedQuestionIndex(iAdvancemenet);

        // Now update the new question's previous question index with our own (for skipping back to this one, if we're skipping questions)
        GetCurrentQuestionPanelScript().SetPreviousQuestionIndex(iThisQuestionIndex);
    }

    /// <summary>
    /// Called from the UI system when the PREVIOUS QUESTION button is pressed
    /// </summary>
    public void PreviousButtonClicked()
    {
        UISurveyQuestionPanel questionPanel = GetCurrentQuestionPanelScript();

        if (questionPanel == null)
            return;

        SetSelectedQuestionIndex(questionPanel.PreviousQuestionIndex());
    }

    
    /// <summary>
    /// Called from the UI system when the SKIP question button is pressed
    /// </summary>
    public void SkipButtonClicked()
    {
        LoadNextScene();
    }

    private UISurveyQuestionPanel GetCurrentQuestionPanelScript()
    {
        if (m_iActiveQuestionIndex < 0 || m_iActiveQuestionIndex >= m_QuestionPanels.Count)
            return null;

        return m_QuestionPanels[m_iActiveQuestionIndex].GetComponent<UISurveyQuestionPanel>();
    }
    
    /// <summary>
    /// Called when the last question has been answered or we've skipped to/past the end
    /// </summary>
    private void SurveyCompleted()
    {
        Debug.Log("Survey completed!");
      
        SaveSurveyResults( BuildSurveyResultXML() );

        LoadNextScene();
    }

    private void SaveSurveyResults(string strXml)
    {
        string userName = UserProfile.sCurrent.Name.Trim().Replace(" ", "");

        if (userName.Equals(""))
        {
            Debug.Log("Error! Trying to save a survey when no user is logged in!");
            return;
        }

        string strFolder = Application.persistentDataPath + "/surveys/" + userName;

        if (!Directory.Exists(strFolder))
        {
            Debug.Log("Folder not found!");

            var folder = Directory.CreateDirectory(strFolder);

            if (folder == null)
            {
                Debug.Log("Error creating folder to save surveys!");
                return;
            }
            else
            {
                Debug.Log("Created survey folder at " + strFolder);
            }
        }
        
        string strFileName = strFolder + "/" + SurveyToUse.SurveyXMLFile.name + ".xml";

        try
        {
            System.IO.File.WriteAllText(strFileName, strXml);
            Debug.Log("Survey saved to " + strFileName);
        }
        catch
        {
            Debug.Log("Error writing survey to " + strFileName);
        }               
    }

    private void EmailSurveyResults(string strXML)
    {
        Debug.Log("Attempting to email survey...");

        //Email.SendEmail("masangahospitalppe@gmail.com", "masangahospitalppe@gmail.com", "Email Test", strXML, "4769438fhdjsksh78839574");
    }

    private string BuildSurveyResultXML()
    {
        string strResultsXML = "<survey>\n";

        foreach (GameObject go in m_QuestionPanels)
        {
            UISurveyQuestionPanel qp = go.GetComponent<UISurveyQuestionPanel>();

            if (qp.Answered())
            {
                strResultsXML += qp.GetXML();
            }
        }

        strResultsXML += "</survey>";

        return strResultsXML;     
    }

    private void LoadNextScene()
    {
		if (UILoading.instance != null) UILoading.instance.LoadScene(NextSceneName);
		else Application.LoadLevel(NextSceneName);
    }
}
