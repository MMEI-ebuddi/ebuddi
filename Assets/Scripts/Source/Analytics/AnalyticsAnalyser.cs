using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class AnalyticsAnalyser
{
    private List<AnalyticsSession> m_Sessions = new List<AnalyticsSession>();

    public AnalyticsAnalyser()
    {
        LoadAllSessions();
    }

    private void LoadAllSessions()
    {
        m_Sessions = new List<AnalyticsSession>();

        List<string> users = UserProfile.GetUserProfileNames();
        
        // Loop through all users and load any session data we have for them
        foreach (string user in users)
        {            
            List<AnalyticsSession> userSessions = LoadAllSessionsForUser(user);

            if (userSessions!=null && userSessions.Count > 0)
            {                
                m_Sessions.AddRange(userSessions);
            }
        }
    }

    private List<AnalyticsSession> LoadAllSessionsForUser(string strUsername)
    {
        string strFolder = Application.persistentDataPath + AnalyticsData.ANALYTICS_SESSIONS_PATH + strUsername;
        
        if (!Directory.Exists(strFolder))
        {
            return null;
        }
        
        // Find all sessions in the user's survey folder
        var info = new DirectoryInfo(strFolder);
        var fileInfo = info.GetFiles();

        List<AnalyticsSession> sessions = new List<AnalyticsSession>();

        // Go through every file in the user's session folder
        foreach (FileInfo file in fileInfo)
        {
            try
            {
                AnalyticsSession session = new AnalyticsSession();
                session.Load(file.FullName);
                sessions.Add(session);
            }
            catch (Exception e)
            {
                Debug.Log("Error trying to LoadAllSessionsForUser() for user " + strUsername + ". " + e);
            }
        }

        return sessions;
    }

    public List<AnalyticsSession> GetSessionsForUser(string strUser, string strSessesionType = "")
    {
        List<AnalyticsSession> sessions = new List<AnalyticsSession>();

        foreach (AnalyticsSession session in m_Sessions)
        {
            if (session.GetUserName() == strUser)
            {
                if (strSessesionType == "")
                {
                    sessions.Add(session);
                }
                else 
                {
                    if (session.GetSessionType() == strSessesionType)
                        sessions.Add(session);
                }                                            
            }
        }

        return sessions;
    }

    // Return a list of all the users for whom we have session data
    public List<string> GetUserNames()
    {
        List<string> users = new List<string>();

        foreach (AnalyticsSession session in m_Sessions)
        {
            if (!users.Contains(session.GetUserName()))
            {
                users.Add(session.GetUserName());                
            }
        }

        return users;
    }

    public int GetTotalRunsForUser(string strUser, string strSessionType)
    {
        List<AnalyticsSession> sessions = GetSessionsForUser(strUser);

        int iTotal = 0;

        if (sessions == null)
            return iTotal;

        
        foreach (AnalyticsSession session in sessions)
        {
            if (session.GetSessionType()==strSessionType)
                iTotal += GetNumberOfRunsForSession(session);
        }

        return iTotal;
    }

    public int GetNumberOfRunsForSession(AnalyticsSession session)
    {        
        List<int> runIds = new List<int>();

        foreach (AnalyticsAction action in session.GetActions())
        {
            if (!runIds.Contains(action.Run))
                runIds.Add(action.Run);
        }


        return runIds.Count;
    }

    public int GetTotalNumberOfFailedRunsForUser(string strUser, string strSessionType)
    {
        List<AnalyticsSession> sessions = GetSessionsForUser(strUser);

        int iTotal = 0;

        if (sessions == null)
            return iTotal;

        foreach (AnalyticsSession session in sessions)
        {
            if (session.GetSessionType()==strSessionType)
                iTotal += GetNumberOfFailedRunsForSession(session);
        }

        return iTotal;
    }

    public int GetNumberOfFailedRunsForSession(AnalyticsSession session)
    {
        int iTotal = 0;

        foreach (AnalyticsAction action in session.GetActions())
        {
            if (action.Expected != action.Guessed)
                iTotal++;
        }


        return iTotal;
    }    


    public List<String> GetFailedQuizesForSession(AnalyticsSession session)
    {
        List<string> failedItems = new List<String>();

        List<AnalyticsQuiz> quizes = session.GetAllQuizes();

        foreach (AnalyticsQuiz quiz in quizes)
        {
            if (quiz.Guesses.Count > 1)
            {                
                failedItems.Add(quiz.Question);
            }
        }
        return failedItems;
    }

    public List<string> GetFailedItemsForSession(AnalyticsSession session)
    {
        List<string> failedItems = new List<String>();

        List<AnalyticsAction> actions = session.GetActions();

        foreach (AnalyticsAction action in actions)
        {
            if (action.Expected != action.Guessed)
            {
                failedItems.Add(action.Expected);
            }
        }

        return failedItems;
    }
}
