using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;
using System.Linq;

public class AnalyticsRecorder : MonoBehaviour {

    private static AnalyticsRecorder m_Instance;
                 
    private float m_fTimeOffset = 0f;

    private AnalyticsSession m_Session;

    private int m_iTestRun;

    public static AnalyticsRecorder Instance
    {
        get
        {
            if (m_Instance == null)
            {
                GameObject analyticsRecorderObject = new GameObject("AnalyticsRecorder");
                m_Instance = analyticsRecorderObject.AddComponent<AnalyticsRecorder>();
            }

            return m_Instance;
        }
    }
    
    public void StartSession(string strSessionType)
    {
		Debug.Log(Application.persistentDataPath);
        Debug.Log("Starting AnalyticsRecorder for " + strSessionType);

        m_Session = new AnalyticsSession();
		m_Session.CreateSession(strSessionType, DateTime.Now.ToString("M/d/yyyy h:m:s tt"));

        #if UNITY_ANDROID && !UNITY_EDITOR
            StartCoroutine(GetLocation());
        #endif
    }

    public void StopSession()
    {
//        m_Session.Save( Application.persistentDataPath + AnalyticsData.ANALYTICS_SESSIONS_PATH + UserProfile.sCurrent.Name.Trim() + "/" );   
		SaveSession();
    }

	public void SaveSession() {

		m_Session.Save( Application.persistentDataPath + AnalyticsData.ANALYTICS_SESSIONS_PATH + UserProfile.sCurrent.Name.Trim() + "/" );
	}


    public void SetTimeOffset(float fOffset, bool bIncreaseRun = true)
    {
        m_fTimeOffset = fOffset;

        if (bIncreaseRun)
            m_iTestRun++;        
    }
    
	public void LogCorrectAction(string strCorrectAction, float fTime, Dictionary<string, string> pAdditionalAttributes = null)
    {
        m_Session.AddAction(strCorrectAction, strCorrectAction, fTime - m_fTimeOffset, m_iTestRun, pAdditionalAttributes);       
    }

	public void LogWrongAction(string strCorrectAction, string strGuess, float fTime, Dictionary<string, string> pAdditionalAttributes = null)
    {
        m_Session.AddAction(strCorrectAction, strGuess, fTime - m_fTimeOffset, m_iTestRun, pAdditionalAttributes);               
    }
  
    public void LogQuizAnswer(Quiz sourceQuiz, string strAnswer)
    {
        m_Session.AddQuizAnswer(sourceQuiz.Question, sourceQuiz.GetCorrectAnswer().name, strAnswer, m_iTestRun, QuizType.training);        
    }

	public void LogRightWrongQuizAnswer(RightWrongQuiz sourceQuiz, string strAnswer)
	{
		m_Session.AddQuizAnswer(sourceQuiz.question, sourceQuiz.correctImage.name, strAnswer, m_iTestRun, sourceQuiz.type);        
	}

	public void LogQuestionAnswer(Question question, string strAnswer, Dictionary<string, string> pAdditionalAttributes = null)
	{
		m_Session.AddQuizAnswer(question.question, question.possibleAnswers[question.correctAnswer], strAnswer, m_iTestRun, QuizType.training, pAdditionalAttributes);        
	}


	public void LogQuestionAnswer(RightWrongQuiz sourceQuiz, string strAnswer)
	{
		m_Session.AddQuizAnswer(sourceQuiz.question, sourceQuiz.correctImage.name, strAnswer, m_iTestRun, sourceQuiz.type);        
	}


    IEnumerator GetLocation()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("AnalyticsRecorder: Location services are disabled!");
            yield break;
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 30;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        
        if (maxWait < 1)
        {
            Debug.Log("AnalyticsRecorder: location serice timed out!");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("AnalyticsRecorder: Failed to find GPS location!");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            Debug.Log("Location: " + Input.location.lastData.latitude.ToString() + " " + Input.location.lastData.longitude.ToString() + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();

        if (m_Session!=null)
        {
            m_Session.SetLatitudeLongitude(Input.location.lastData.latitude.ToString(), Input.location.lastData.longitude.ToString());
        }
    }

}

