using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Prime31;

public class AnalyticsData : MonoBehaviour
{
    private static AnalyticsData m_Instance;

    public const string ANALYTICS_ROOT_PATH = "/analytics/";
    public const string ANALYTICS_SURVEYS_PATH = ANALYTICS_ROOT_PATH + "surveys/";
    public const string ANALYTICS_SESSIONS_PATH = ANALYTICS_ROOT_PATH + "sessions/";

    // Delegate events
    public delegate void DataSubmissionStartedEvent();
    public event DataSubmissionStartedEvent DataSubmissionStarted;

    public delegate void DataSubmissionCompletedEvent();
    public event DataSubmissionCompletedEvent DataSubmissionCompleted;

    public delegate void DataSubmissionFailedEvent(string error);
    public event DataSubmissionFailedEvent DataSubmissionFailed;

    private float m_fSubmissionTimeout = 15f;

    // Note: Wanted to keep this a static class and not have a singleton, but need to have one to use a Coroutine for posting...
    public static AnalyticsData Instance
    {
        get
        {
            if (m_Instance == null)
            {
                GameObject analyticsObject = new GameObject("AnalyticsData");
                DontDestroyOnLoad(analyticsObject);
                m_Instance = analyticsObject.AddComponent<AnalyticsData>();                
            }

            return m_Instance;
        }
    }
     
    // Post data for all users
    public string PostAllUserData(bool bPost = true)
    {
        List<string> users = UserProfile.GetUserProfileNames();

        string strXML = "<analytics>\n";
        
        foreach (string user in users)
        {
            Debug.Log("User: " + user + ". Pending session count: " + GetPendingSessionCount(user));

            strXML += "<userdata>\n";
            strXML += (GetUserData(user) + "\n");
			Debug.Log((GetUserData(user)));
            strXML += "\n</userdata>\n";
        }

        strXML += "\n</analytics>";

        if (bPost)
        {
            Debug.Log(strXML);
            PostDataToServer(strXML);
        }

        return strXML;
    }






    private string GetUserData(string strUsername)
    {           
        return GetProfileXML(strUsername) + GetSurveyXML(strUsername) + GetSessionXML(strUsername); 
    }

	private void SaveLocalCopy(string strXML)
    {
		string filename = "AnalyticsData_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH_mm") + ".xml";

		var sr = File.CreateText(filename);
		sr.Write(strXML);

		sr.Close();
	}



	public void SendDataByEmail(string strXML)
	{
		string filename = Application.persistentDataPath + "/AnalyticsData_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH_mm") + ".xml";
		
		var sr = File.CreateText(filename);
		sr.Write(strXML);
		
		sr.Close();

		Debug.Log("Open email composer");
		EtceteraAndroid.showEmailComposer("", "MMEI Analytica Data", "", false, filename);

	}


    private void PostDataToServer(string strXML)
    {
		//if editor save for easy preview
	    if (Application.isEditor) SaveLocalCopy(strXML);

        if (DataSubmissionStarted != null) DataSubmissionStarted();

        try
        {                    
            WWWForm form = new WWWForm();            

            form.AddField("data", strXML);
        
            var headers = form.headers;
            var rawData = form.data;

            headers["Authorization"] = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("3VUXCLz5XcuYo3S3zzh3:MAsqFT6FiyYd2fbiHVwS"));
                        
            WWW www = new WWW("http://178.79.159.78:9091", rawData, headers);            
                         
            StartCoroutine(WaitForRequest(www));

        }
        catch (Exception e)
        {
			if (DataSubmissionFailed != null) DataSubmissionFailed(e.Message);
            Debug.LogError("Error posting data! " + e);
        }
    }



    IEnumerator WaitForRequest(WWW www)
    {
        Invoke("DataSubmissionError", m_fSubmissionTimeout);

        yield return www;

        CancelInvoke("DataSubmissionError");

        // check for errors
        if (www.error == null)
        {
            if (DataSubmissionCompleted != null)
                DataSubmissionCompleted();

            Debug.Log("Posted survey data ok! " + www.text);
        }
        else
        {
			Debug.LogError(www.error);
			if (DataSubmissionFailed != null) DataSubmissionFailed(www.error);
        }
    }    



    private string GetProfileXML(string strUsername)
    {
        string profileXML = "";

        try
        {
            profileXML = System.IO.File.ReadAllText(Application.persistentDataPath + "/" + strUsername + ".xml") + "\n\n";
        }
        catch (Exception e)
        {
            Debug.LogError("Error trying to GetProfileXML(). " + e);
        }

        return profileXML;
    }

    private string GetSurveyXML(string strUsername)
    {
        string surveyXML = "";

        string strFolder = Application.persistentDataPath + ANALYTICS_SURVEYS_PATH + strUsername;

        if (!Directory.Exists(strFolder))
        {
            Debug.Log("GetSurveyXML() - Surveyfolder not found!");
            return surveyXML;
        }
              
        // Find all surveys in the user's survey folder
        var info = new DirectoryInfo(strFolder);
        var fileInfo = info.GetFiles();
        
        // Loop through each survey and append it's contents to our XML string
        foreach (FileInfo file in fileInfo)
        {            
            try
            {
                surveyXML += System.IO.File.ReadAllText(file.FullName) + "\n";
            }
            catch (Exception e)
            {
                Debug.Log("Error trying to GetSurveyXML() " + e);
            }
        }        
       
        return surveyXML;
    }

    static public int GetTotalPendingSessionCount()
    {
        int iCount = 0;

        List<string> users = UserProfile.GetUserProfileNames();

        foreach (string user in users)
        {
            iCount += GetPendingSessionCount(user);
        }

        return iCount;
        
    }

    // Return how many session files we have for this user in the pending folder
    static public int GetPendingSessionCount(string strUsername)
    {
        string strFolder = Application.persistentDataPath + ANALYTICS_SESSIONS_PATH + strUsername;

        if (!Directory.Exists(strFolder))
        {
            Debug.Log("GetPendingSessionCount() - session user folder not found!");
            return 0;
        }

        // Find all sessions in the user's survey folder
        var info = new DirectoryInfo(strFolder);
        var fileInfo = info.GetFiles();

        return fileInfo.Length;
    }

    
    static public bool ArchiveAllSessions()
    {
        //TODO: Archive! Return true if we managed to backup them up or false if there was an error
        return true;
    }
 
    static public bool ArchiveSessionsForUser(string strUsername)
    {
        //TODO: Archive! Return true if we managed to backup them up or false if there was an error
        return true;
    }

    static public void DeleteAllSessions()
    {
        List<string> users = UserProfile.GetUserProfileNames();

        foreach (string user in users)
        {
            DeleteSessionsForUser(user);
        }
    }
    
    static public void DeleteSessionsForUser(string strUsername)
    {
        string strFolder = Application.persistentDataPath + ANALYTICS_SESSIONS_PATH + strUsername;

        if (!Directory.Exists(strFolder))
        {
            return;
        }

        var info = new DirectoryInfo(strFolder);
        var fileInfo = info.GetFiles();

        // Loop through each file
        foreach (FileInfo file in fileInfo)
        {
            try
            {
                file.Delete();
            }
            catch (Exception e)
            {
                Debug.Log("Error trying to delete session data. " + e);
            }
        }

    }

    private string GetSessionXML(string strUsername)
    {
        string sessionXML = "";

        string strFolder = Application.persistentDataPath + ANALYTICS_SESSIONS_PATH + strUsername;

        if (!Directory.Exists(strFolder))
        {
            Debug.Log("---------- GetSessionXML() - session user folder not found for user " + strUsername);
            return sessionXML;
        }

        // Find all sessions in the user's survey folder
        var info = new DirectoryInfo(strFolder);
        var fileInfo = info.GetFiles();

        // Loop through each survey and append it's contents to our XML string
        foreach (FileInfo file in fileInfo)
        {
            try
            {
                sessionXML += System.IO.File.ReadAllText(file.FullName) + "\n";
            }
            catch (Exception e)
            {
                Debug.Log("Error trying to GetSessionXML() for user " + strUsername + ". "  + e);
            }
        }
        
        return sessionXML;
    }
}

 