using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
// using XmlWriter = System.Xml.XmlWriter;
// using XmlWriter = System.Xml.XmlWriter;
using UnityEngine;
using UnityEngine.UI;

public class UserProfileManager
{
	static UserProfileManager			sInstance = null;
	public static UserProfileManager	Instance { get { return sInstance; } }

	string[]							m_UserProfileFilenames = null;


	internal static UserProfileManager Get()
	{
		if(sInstance == null)
		{
			sInstance = new UserProfileManager();
            sInstance.m_UserProfileFilenames = System.IO.Directory.GetFiles(Application.persistentDataPath, "*.xml");
		}

		if(UserProfile.sCurrent == null)
			UserProfile.CreateNewProfile();

		return(sInstance);
	}


	public bool DoesUserProfileExist(string UserProfileName)
	{
		for(int loop = 0; loop < m_UserProfileFilenames.Length; loop++)
		{
			if(String.Compare(Path.GetFileNameWithoutExtension(sInstance.m_UserProfileFilenames[loop]), UserProfileName, true) == 0)
				return(true);
		}
		return (false);
	}
}

