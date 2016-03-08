using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

	public class TrackedResource
	{
		static List<TrackedResource>		sTrackedResources;

		UnityEngine.Object	m_Object;
		string				m_FullName;
		int					m_RefCount;

		TrackedResource()
		{
			if(sTrackedResources == null)
				sTrackedResources = new List<TrackedResource>();

			sTrackedResources.Add(this);
		}
	
		public static UnityEngine.Object Load(string ResourceName)
		{
			TrackedResource		FoundTrack = Find(ResourceName);
		
			if(FoundTrack != null)
				FoundTrack.m_RefCount++;
			else
			{
				UnityEngine.Object		LoadedAsset;

				try
				{
					LoadedAsset = Resources.Load(ResourceName);
				}
				catch
				{
					return (null);
				}

				if (LoadedAsset != null)
				{
					FoundTrack = new TrackedResource();
					FoundTrack.m_FullName = ResourceName;
					FoundTrack.m_Object = LoadedAsset;
				}
			}	
			if(FoundTrack != null && FoundTrack.m_Object != null)
				return(FoundTrack.m_Object);

			return (null);
		}

		public static string LoadXML(string ResourceName)
		{
			TrackedResource 	FoundTrack = Find(ResourceName);

			if (FoundTrack != null)
				FoundTrack.m_RefCount++;
			else
			{
				UnityEngine.Object		LoadedAsset;
				
				try
				{
					LoadedAsset = Resources.Load(ResourceName);
				}
				catch	// (Exception e)
				{
//					TMSystem.Logger.Add("Failed to load resource - " + ResourceName, TMLogger.eLogType.Error);
					return (null);
				}
				if(LoadedAsset != null)
				{
					FoundTrack = new TrackedResource();
					FoundTrack.m_FullName = ResourceName;
					FoundTrack.m_Object = LoadedAsset;
				}
			}
			if(FoundTrack != null && FoundTrack.m_Object != null)
				return (FoundTrack.m_Object.ToString());

			return (null);
		}

		public static UnityEngine.Object[] LoadAllFromFolder(string ResourceFolder)
		{
			TrackedResource				FoundTrack = Find(ResourceFolder);
			UnityEngine.Object[]		ObjectArray = null;
		
			if(FoundTrack != null)
				FoundTrack.m_RefCount++;
			else
			{
				ObjectArray = Resources.LoadAll(ResourceFolder);
				
				for(int loop = 0; loop < ObjectArray.Length; loop++)
				{
					FoundTrack = new TrackedResource();
					FoundTrack.m_FullName = ResourceFolder + "//" + ObjectArray[loop].name;
					FoundTrack.m_Object = ObjectArray[loop];
				}
			}	
			return(ObjectArray);
		}
	
		static TrackedResource Find (string ResourceName)
		{
			if(sTrackedResources != null)
			{
				for(int loop = 0; loop < sTrackedResources.Count; loop++)
				{
					if(sTrackedResources[loop].m_FullName == ResourceName)
						return(sTrackedResources[loop]);
				}
			}
			return(null);
		}
	}
