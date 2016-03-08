using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;
using System.Linq;

public class AnalyticsSession 
{       
	public string sessionId = "";
    private string m_sessionType;
    private string m_userName;
    private string m_date;    
         
    private List<AnalyticsAction> m_Actions;
    private List<AnalyticsQuiz> m_Quizes;

    private string m_strLatitude, m_strLongitude;

    public void CreateSession(string strSessionType, string strDate)
    {
		Debug.Log(Application.persistentDataPath);
		Debug.Log("Creating AnalyticsSession for " + strSessionType);

		sessionId = UnityEngine.Random.Range(0, int.MaxValue).ToString();
        m_sessionType = strSessionType;
//        m_userName = strUserName;
        m_date = strDate;

        m_strLatitude = "0";
        m_strLongitude = "0";
        m_Actions = new List<AnalyticsAction>();      
        m_Quizes = new List<AnalyticsQuiz>();
    }

	public void AddAction(string strCorrectAction, string strAnswer, float fTime, int iTestRun, Dictionary<string, string> pAdditionalAttributes = null)
    {
        m_Actions.Add(new AnalyticsAction(strCorrectAction, strAnswer, fTime, iTestRun, pAdditionalAttributes));  
		AnalyticsRecorder.Instance.SaveSession();
    }



    
    public List<AnalyticsAction> GetActions()
    {
        return m_Actions;
    }


	public void AddQuizAnswer(string strQuestion, string strCorrectAnswer, string strGuess, int iTestRun, QuizType type = QuizType.training, Dictionary<string, string> pAdditionalAttributes = null)
    {
        AnalyticsQuiz quiz = GetQuiz(strQuestion, iTestRun);

        if (quiz == null)
        {
            quiz = AddQuiz();
            quiz.TestRun = iTestRun;
            quiz.Question = strQuestion;
            quiz.Answer = strCorrectAnswer;
			quiz.type = type;
			if (pAdditionalAttributes != null) {
				quiz.additionalAttributes = pAdditionalAttributes;
			}
        }

        quiz.Guesses.Add(strGuess);

		AnalyticsRecorder.Instance.SaveSession();
    }
  


    private AnalyticsQuiz AddQuiz()
    {
        AnalyticsQuiz newQuiz = new AnalyticsQuiz();

        m_Quizes.Add(newQuiz);

        return newQuiz;
    }

    public List<AnalyticsQuiz> GetAllQuizes()
    {
        return m_Quizes;
    }

    private AnalyticsQuiz GetQuiz(string strQuestion, int iRun)
    {
        foreach (AnalyticsQuiz quiz in m_Quizes)
            if (quiz.Question==strQuestion && quiz.TestRun == iRun)
                return quiz;

        return null;
    }
  
    public void Load(string strPath)
    {
        Debug.Log("Loading session from " + strPath);

        if (!File.Exists(strPath))
        {
            Debug.Log("ERROR! Unable to load AnaylticsSession at " + strPath);
            return;
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(strPath);

        XmlNodeList sessionList = xmlDoc.GetElementsByTagName("session");
       
        foreach (XmlNode session in sessionList)
        {
            LoadSession(session);
        }
    }

    private void LoadSession(XmlNode sessionNode)
    {        
        string strUser = sessionNode.Attributes["user"].InnerText;
        string strType = sessionNode.Attributes["type"].InnerText;
        string strDate = sessionNode.Attributes["date"].InnerText;

        CreateSession(strType, strDate);
          
        XmlNodeList runs = sessionNode.SelectNodes("run");

        foreach (XmlNode run in runs)
        {   
            string strRun = run.Attributes["number"].InnerText;

            // Load actions
            XmlNodeList actions = run.SelectSingleNode("actions").SelectNodes("action");
            
            foreach (XmlNode action in actions)
            {                
                string strTime = action.Attributes["time"].InnerText;
                string strExpected = action.Attributes["expected"].InnerText;
                string strAnswer = action.Attributes["answer"].InnerText;

                AddAction(strExpected, strAnswer, float.Parse(strTime), int.Parse(strRun));
            }

            // Load quizes
            XmlNodeList quizes = run.SelectSingleNode("quizQuestions").SelectNodes("question");

            foreach (XmlNode question in quizes)
            {                
                string strText = question.Attributes["text"].InnerText;
                string strCorrect = question.Attributes["correctAnswer"].InnerText;
                string strAnswers = question.Attributes["answers"].InnerText;
				QuizType quizType = (QuizType)System.Enum.Parse(typeof(QuizType), question.Attributes["quizType"].InnerText);

                string[] answers = strAnswers.Split(',');

                foreach (string strAnswer in answers)
                    AddQuizAnswer(strText, strCorrect, strAnswer, int.Parse(strRun), quizType);
            }
        }              
    }

    public void Save(string strPath)
    {
        if (m_Actions.Count == 0 && m_Quizes.Count == 0)
            return;

        if (!Directory.Exists(strPath))
        {
            var folder = Directory.CreateDirectory(strPath);

            if (folder == null)
            {
                Debug.Log("ERROR! Unable to create folder for AnalyticsSession: " + strPath);
                return;
            }
        }

//        var info = new DirectoryInfo(strPath);
//        var fileInfo = info.GetFiles();
//        int iFileCount = fileInfo.Length;

        strPath += (m_sessionType);

//        Debug.Log("AnalyticsSession::SaveSession(): Saving session to " + strPath);

        XmlWriterSettings WriterSettings = new XmlWriterSettings();
        WriterSettings.OmitXmlDeclaration = true;
		

		XmlWriter Writer = XmlWriter.Create(strPath + "_" + sessionId + ".xml", WriterSettings);
        
        
        // Session
        Writer.WriteStartElement("session");
                Writer.WriteAttributeString("type", m_sessionType);                
                Writer.WriteAttributeString("user", UserProfile.sCurrent.Name);
				Writer.WriteAttributeString("trainer", PlayerPrefs.GetString("trainerName", ""));
				Writer.WriteAttributeString("hospital", PlayerPrefs.GetString("hospitalName", ""));
                Writer.WriteAttributeString("date", m_date);
                Writer.WriteAttributeString("latitude", m_strLatitude);
                Writer.WriteAttributeString("longitude", m_strLongitude);
                
        int iTotalTime = 0;
        int iCurrentRun = -1;
        int iRunTime = 0;

		if (m_Actions.Count > 0) {
			// TODO: Make this whole bit a lot less ugly 
			foreach (AnalyticsAction action in m_Actions)
			{           
				if (action.Run > iCurrentRun)
				{
//					if (iCurrentRun >= 0)
//					{

						// /actions
//						Writer.WriteEndElement();
						
						SaveQuizes(Writer, iCurrentRun);
						
//						// /run
//						Writer.WriteEndElement();
//					}
					
					iCurrentRun = action.Run;
					iRunTime = 0;
					
					// Run
					Writer.WriteStartElement("run");
					Writer.WriteAttributeString("number", iCurrentRun.ToString());
					
					// Actions
					Writer.WriteStartElement("actions");
				}
				
				int iActionTime = (int)action.Time - iRunTime;
				iRunTime = (int)action.Time;
				
				iTotalTime += iActionTime;
				
				Writer.WriteStartElement("action");
				Writer.WriteAttributeString("totalTime", iTotalTime.ToString());
				Writer.WriteAttributeString("time", iActionTime.ToString());
				Writer.WriteAttributeString("expected", action.Expected);
				Writer.WriteAttributeString("answer", action.Guessed);
				if (action.additionalAttributes.Count > 0) {
					foreach(KeyValuePair<string,string> attribute in action.additionalAttributes) {
						Writer.WriteAttributeString(attribute.Key, attribute.Value);
					}
				}
				Writer.WriteEndElement();                                                
			}
			
			// / actions
			Writer.WriteEndElement();
			
			// / run
			Writer.WriteEndElement();
		}



		//questionnaires
		if (m_Quizes.Count > 0 && iCurrentRun == -1) {
	        SaveQuizes(Writer);
		}

        // /session
        Writer.WriteEndElement();
        Writer.Close();
    }
	

	private void SaveQuizes(XmlWriter Writer, int iRun = -1)
    {
		//overriding run for questionnaire
		if (iRun == -1) {
			Writer.WriteStartElement("run");
			Writer.WriteAttributeString("number", "1");
		}

		Writer.WriteStartElement("quizQuestions");



        foreach (AnalyticsQuiz quiz in m_Quizes)
        {   
            if (quiz.TestRun==iRun || iRun == -1)
            {
                Writer.WriteStartElement("question");
                    Writer.WriteAttributeString("text", quiz.Question);
                    Writer.WriteAttributeString("time", "0");
                    Writer.WriteAttributeString("correctAnswer", quiz.Answer);
					Writer.WriteAttributeString("quizType", quiz.type.ToString());
					if (quiz.additionalAttributes.Count > 0) {
						foreach(KeyValuePair<string,string> attribute in quiz.additionalAttributes) {
							Writer.WriteAttributeString(attribute.Key, attribute.Value);
						}
					}


                    string joined = string.Join(",", quiz.Guesses.ToArray());
                    Writer.WriteAttributeString("answers", joined);
				Writer.WriteEndElement();           
            }
        }

        Writer.WriteEndElement();

		if (iRun == -1) {
			Writer.WriteEndElement();
		}
    }


    public string GetUserName()
    {
        return m_userName;
    }

    public string GetSessionType()
    {
        return m_sessionType;
    }

    public string GetDate()
    {
        return m_date;
    }

    public void SetLatitudeLongitude(string strLat, string strLong)
    {
        m_strLatitude = strLat;
        m_strLongitude = strLong;
    }

    public string GetLatitude()
    {
        return m_strLatitude;
    }

    public string GetLongitude()
    {
        return m_strLongitude;
    }
}

