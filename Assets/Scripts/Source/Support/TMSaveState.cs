using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;

#if UNITY_WP8 || UNITY_METRO
using System.Xml;
using System.Xml.Serialization;
#else
using System.Runtime.Serialization.Formatters.Binary;
#endif

	public class TMSaveState
	{
#if !(UNITY_WP8 || UNITY_METRO)
		public static BinaryFormatter		bf = new BinaryFormatter();
#endif
		public TMSaveState()
		{
		}

		protected bool		m_Dirty = false;

		public int GetInt(string key, int defaultValue = 0)
		{
			return PlayerPrefs.GetInt(key, defaultValue);
		}
		public float GetFloat(string key, float defaultValue = 0)
		{
			return PlayerPrefs.GetFloat(key, defaultValue);
		}
		public string GetString (string key, string defaultValue = null)
		{
			return PlayerPrefs.GetString (key, defaultValue);
		}

		public void SetInt (string key, int value)
		{
			PlayerPrefs.SetInt (key, value);
		}
		public void SetFloat (string key, float value)
		{
			PlayerPrefs.SetFloat (key, value);
			m_Dirty = true;
		}
		public void SetString (string key, string value)
		{
			PlayerPrefs.SetString (key, value);
			m_Dirty = true;
		}

		public void AddInt(string key, int value)
		{
			value += PlayerPrefs.GetInt(key);
			PlayerPrefs.SetInt(key, value);
			m_Dirty = true;
		}
		public void AddFloat(string key, float value)
		{
			value += PlayerPrefs.GetFloat(key);
			PlayerPrefs.SetFloat(key, value);
			m_Dirty = true;
		}

		public bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(key);
		}

		public void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(key);
			m_Dirty = true;
		}

		public void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
			m_Dirty = false;
		}

		public void Load()
		{
// 			// Unity handles the normal save values, but we need to check on the custom ones
// 			if (HasKey("CUSTOMSAVESTATE"))
// 			{
// 				string			CustomSaveState = GetString("CUSTOMSAVESTATE");
// 				TMFile	Container = new TMFile();
// 
// 				Container.ParseDocument(CustomSaveState);
// 				CustomLoad(Container);
// 			}
		}

		public void Save()
		{
			if (m_Dirty == false)
				return;

			PlayerPrefs.Save();

			if (NeedToSaveCustomData())
			{
// 				TMFile		Container = new TMFile();
// 
// 				Container.CreateDocumentString("custom");
// 				CustomSave(Container);
// 				SetString("CUSTOMSAVESTATE", Container.SaveDocument());
			}

			m_Dirty = false;
		}

		protected virtual bool NeedToSaveCustomData()
		{
			return (false);
		}

		public static void SaveObject(string prefKey, object serializableObject)
		{
			MemoryStream			memoryStream = new MemoryStream();

#if UNITY_WP8 || UNITY_METRO
			XmlSerializer	serializer = new XmlSerializer(serializableObject.GetType());
			serializer.Serialize(memoryStream, serializableObject);
			string tmp = System.Convert.ToBase64String(memoryStream.ToArray());
#else
			bf.Serialize(memoryStream, serializableObject);
			string tmp = System.Convert.ToBase64String(memoryStream.ToArray());
#endif
			PlayerPrefs.SetString(prefKey, tmp);
		}

		protected void SerialiseToMemoryStream(MemoryStream memoryStream, object Data)
		{
#if UNITY_WP8 || UNITY_METRO
			XmlSerializer	serializer = new XmlSerializer(typeof(object));
			serializer.Serialize(memoryStream, Data);
#else
			bf.Serialize(memoryStream, Data);
#endif
		}

		public static void ResetObject(string prefKey)
		{
			PlayerPrefs.DeleteKey(prefKey);
		}

		public static object LoadObject(string prefKey)
		{
			string						tmp = PlayerPrefs.GetString(prefKey, string.Empty);

			if (tmp == string.Empty)
				return null;

			MemoryStream			memoryStream = new MemoryStream(System.Convert.FromBase64String(tmp));

#if UNITY_WP8 || UNITY_METRO
			XmlSerializer	serializer = new XmlSerializer(typeof(object));
			return(serializer.Deserialize(memoryStream));
#else
			return bf.Deserialize(memoryStream);
#endif
		}
	}
