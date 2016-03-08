using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
// using XmlWriter = System.Xml.XmlWriter;
// using XmlWriter = System.Xml.XmlWriter;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UserProfile
{
	static readonly int			sNumScenesToTrack = 4;
	public static UserProfile	sCurrent;

	string				m_Name;
	public string		Name { get { return m_Name; } }

	public enum eSex
	{
		Male = 0,
		Female
	}
	eSex				m_Sex;
	public eSex			Sex { get { return m_Sex; } }

	int					m_Age;
	public int			Age { get { return m_Age; } }

	List<int>			m_BuddyLinesSpoken;
	int					m_TimesBuddySkipped;

	int[]				m_SceneCompletionCount;
	float[]				m_SceneCompletionTimes;

	string				m_EquipmentDescriptionString;
	public string		EquipmentDescriptionString { get { return m_EquipmentDescriptionString; } }

	int					m_IncorrectDonning;
	int					m_IncorrectDoffing;




	static public void CreateNewProfile()
	{
		UserProfile		NewProfile = new UserProfile();

		NewProfile.m_Name = "";
		NewProfile.m_Sex = eSex.Male;
		NewProfile.m_Age = 0;
		NewProfile.m_BuddyLinesSpoken = new List<int>(1000);
		NewProfile.m_TimesBuddySkipped = 0;
		NewProfile.m_SceneCompletionCount = new int[sNumScenesToTrack];
		NewProfile.m_SceneCompletionTimes = new float[sNumScenesToTrack];
		for (int loop = 0; loop < sNumScenesToTrack; loop++)
		{
			NewProfile.m_SceneCompletionCount[loop] = -1;
			NewProfile.m_SceneCompletionTimes[loop] = -1;
		}

        NewProfile.m_EquipmentDescriptionString = "FaceShield,FaceMask,HeadCover,Hood,Scrubs,Gown,ReusableApron,InnerGloves,OuterGloves,Boots";
		NewProfile.m_IncorrectDonning = 0;
		NewProfile.m_IncorrectDoffing = 0;
		UserProfile.sCurrent = NewProfile;
	}

	public void SetNameSexAge(string Name, eSex Sex, int Age)
	{
		m_Name = Name;
		m_Sex = Sex;
		m_Age = Age;
		sCurrent.Save();
	}

	public void BuddyHasSpoken(string MediaName)
	{
		int			HashCode = MediaName.GetHashCode();

		if(!m_BuddyLinesSpoken.Contains(HashCode))
			m_BuddyLinesSpoken.Add(HashCode);
	}

	internal bool HasBuddySpoken(string MediaName)
	{
		return(m_BuddyLinesSpoken.Contains(MediaName.GetHashCode()));
	}

	internal void SkippedBuddy()
	{
		m_TimesBuddySkipped++;
	}

	internal void SceneComplete(int SceneIndex, bool ShouldSave = true)
	{
		m_SceneCompletionCount[SceneIndex]++;
		if(ShouldSave)
			Save();
	}
	internal void SetSceneCompletedTime(int SceneIndex, float TimeInScene, bool ShouldSave = true)
	{
		if (m_SceneCompletionTimes[SceneIndex] < 0)
			m_SceneCompletionTimes[SceneIndex] = TimeInScene;

		if (ShouldSave)
			Save();
	}

	internal int GetSceneCompleteCount(int SceneIndex)
	{
		return(m_SceneCompletionCount[SceneIndex]);
	}


	internal void SetEquipmentDescription(string EquipmentDescriptionString)
	{
		m_EquipmentDescriptionString = EquipmentDescriptionString;
		Save();
	}

	internal void IncorrectDonning(bool ShouldSave = true)
	{
		m_IncorrectDonning++;
		if (ShouldSave)
			Save();
	}

	internal void IncorrectDoffing(bool ShouldSave = true)
	{
		m_IncorrectDoffing++;
		if (ShouldSave)
			Save();
	}

	public void Save()
	{        
		XmlWriterSettings		WriterSettings = new XmlWriterSettings();

        WriterSettings.OmitXmlDeclaration = true;
		WriterSettings.Indent = true;
		WriterSettings.IndentChars = ("\t");

		if (m_Name.Length == 0)
		{
			m_Name = "Test";
			m_Age = 30;
			m_Sex = eSex.Male;
		}

        Debug.Log("Saving to " + Path.Combine(Application.persistentDataPath, m_Name + ".xml"));

		XmlWriter		Writer = XmlWriter.Create(Path.Combine(Application.persistentDataPath, m_Name + ".xml"), WriterSettings);	// , , System.Text.Encoding.UTF7);
		string			BuddyLines = "";

		for (int loop = 0; loop < m_BuddyLinesSpoken.Count; loop++)
		{
			if(loop > 0)
				BuddyLines += ",";

			BuddyLines += m_BuddyLinesSpoken[loop].ToString("X");
		}

		Writer.WriteStartDocument();
		Writer.WriteStartElement("User");

			Writer.WriteAttributeString("name", m_Name);
			Writer.WriteAttributeString("sex", m_Sex.ToString());
			Writer.WriteAttributeString("age", m_Age.ToString());

			Writer.WriteStartElement("Buddy");
				Writer.WriteAttributeString("skipped", m_TimesBuddySkipped.ToString());
				Writer.WriteAttributeString("lines", BuddyLines);
			Writer.WriteEndElement();

			Writer.WriteStartElement("ScenesPlayed");
				for(int loop = 0; loop < m_SceneCompletionCount.Length; loop++)
				{
					Writer.WriteAttributeString("scene" + loop.ToString(), m_SceneCompletionCount[loop].ToString());
				}
			Writer.WriteEndElement();

			Writer.WriteStartElement("ScenesTime");
			for (int loop = 0; loop < m_SceneCompletionTimes.Length; loop++)
			{
				Writer.WriteAttributeString("scene" + loop.ToString(), m_SceneCompletionTimes[loop].ToString());
			}
			Writer.WriteEndElement();

			Writer.WriteStartElement("Equipment");
				Writer.WriteAttributeString("description", m_EquipmentDescriptionString);
			Writer.WriteEndElement();

			Writer.WriteStartElement("Tracking");
			Writer.WriteAttributeString("baddonning", m_IncorrectDonning.ToString());
			Writer.WriteAttributeString("baddoffing", m_IncorrectDoffing.ToString());
			Writer.WriteEndElement();

		Writer.WriteEndElement();
		Writer.WriteEndDocument();
		Writer.Close();
	}

    public static List<string> GetUserProfileNames()
    {
        List<string> userNames = new List<string>();

        var info = new DirectoryInfo(Path.GetFullPath(Application.persistentDataPath));
        var fileInfo = info.GetFiles("*.xml");
        
        // Loop through each survey and append it's contents to our XML string
        foreach (FileInfo file in fileInfo)
        {
            userNames.Add(file.Name.Replace(".xml", ""));
        }

        return userNames;
    }

	internal static void Load()
	{
        string[] Files = System.IO.Directory.GetFiles(Application.persistentDataPath, "*.xml");

		if (Files.Length == 0)
			return;
       
		Load(Files[0]);
	}

	internal static bool Load(string UserFileName)
	{
        string strPath = Path.Combine(Application.persistentDataPath, UserFileName);


        Debug.Log("Loading: " + strPath);

        XmlReader Reader = XmlReader.Create(strPath);
		XmlNodeType		nType;

		while (Reader.Read())
		{
			nType = Reader.NodeType;
			if (nType == XmlNodeType.Element)
			{
				if (Reader.Name == "User")
				{
					ProcessUserData(Reader);
					while (Reader.Read())
					{
						nType = Reader.NodeType;
						if (nType == XmlNodeType.Element)
						{
							if (Reader.Name == "Buddy")
							{
								ProcessBuddyData(Reader);
							}
							else if (Reader.Name == "ScenesPlayed")
							{
								ProcessScenesPlayedData(Reader);
							}
							else if (Reader.Name == "ScenesTime")
							{
								ProcessScenesTimeData(Reader);
							}
							else if (Reader.Name == "Equipment")
							{
								ProcessEquipmentData(Reader);
							}
							else if (Reader.Name == "Tracking")
							{
								ProcessTrackingData(Reader);
							}
						}
					}
				}
			}
		}

		return (true);
	}

	static void ProcessUserData(XmlReader Reader)
	{
		sCurrent.m_Name = GetAttributeS(Reader, "name");
		sCurrent.m_Sex = (eSex)GetAttributeE(Reader, "sex", typeof(eSex));
		sCurrent.m_Age = GetAttributeI(Reader, "age");
	}

	static void ProcessBuddyData(XmlReader Reader)
	{
		string		BuddyLines;
		string[]	LinesArray;

		sCurrent.m_TimesBuddySkipped = GetAttributeI(Reader, "skipped");
		BuddyLines = GetAttributeS(Reader, "lines");
		if(BuddyLines.Length > 0)
		{
			LinesArray = BuddyLines.Split(',');
			for (int loop = 0; loop < LinesArray.Length; loop++)
			{
				sCurrent.m_BuddyLinesSpoken.Add(Convert.ToInt32(LinesArray[loop], 16));
			}
		}
	}

	static void ProcessScenesPlayedData(XmlReader Reader)
	{
		for (int loop = 0; loop < sCurrent.m_SceneCompletionCount.Length; loop++)
		{
			sCurrent.m_SceneCompletionCount[loop] = GetAttributeI(Reader, "scene" + loop.ToString());
		}
	}

	static void ProcessScenesTimeData(XmlReader Reader)
	{
		for (int loop = 0; loop < sCurrent.m_SceneCompletionTimes.Length; loop++)
		{
			sCurrent.m_SceneCompletionTimes[loop] = GetAttributeF(Reader, "scene" + loop.ToString());
		}
	}
	

	static void ProcessEquipmentData(XmlReader Reader)
	{
		sCurrent.m_EquipmentDescriptionString = GetAttributeS(Reader, "description");
	}

	private static void ProcessTrackingData(XmlReader Reader)
	{
		sCurrent.m_IncorrectDonning = GetAttributeI(Reader, "baddonning");
		sCurrent.m_IncorrectDoffing = GetAttributeI(Reader, "baddoffing");
	}

	static string GetAttributeS(XmlReader Reader, string Key)
	{
		return(Reader.GetAttribute(Key));
	}
	static int GetAttributeI(XmlReader Reader, string Key)
	{
		string		Value = Reader.GetAttribute(Key);

		if(Value != null)
			return(Convert.ToInt32(Value));
		else
			return(0);
	}
	static float GetAttributeF(XmlReader Reader, string Key)
	{
		string		Value = Reader.GetAttribute(Key);

		if (Value != null)
			return (Convert.ToSingle(Value));
		else
			return (0);
	}
	static int GetAttributeE(XmlReader Reader, string Key, Type EnumType)
	{
		string		Value = Reader.GetAttribute(Key);
		int			ReturnValue = 0;

		if (Value != null && Value.Length > 0)
		{
			try
			{
				ReturnValue = (int)System.Enum.Parse(EnumType, Value);
			}
			catch
			{
			}

			if (ReturnValue == -1)
				ReturnValue = 0;
		}

		return (ReturnValue);
	}
}
