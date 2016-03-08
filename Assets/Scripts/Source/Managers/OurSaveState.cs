using System;
using System.Collections;
using System.Collections.Generic;

	[System.Serializable]
	public class GameSaveState
	{
		public string	m_HashString;
		public int		m_HowManyTimesRun;
		public int		m_BuildNumber;

		public class TrackedValue
		{
			int			m_Ident;
			object		m_Value;
		}
		Hashtable		m_Values;

		public GameSaveState()
		{
			m_HashString = null;
			m_HowManyTimesRun = 0;
			m_BuildNumber = 0;

			m_Values = new Hashtable(100);
		}

		internal int GetInt(int keyhash, int defaultValue = 0)
		{
			object	Item = m_Values[keyhash];
			
			if(Item != null)
				return((int)Item);
			else
				return(defaultValue);
		}

		internal float GetFloat(int keyhash, float defaultValue = 0)
		{
			object	Item = m_Values[keyhash];
			
			if(Item != null)
				return((float)Item);
			else
				return(defaultValue);
		}

		internal string GetString(int keyhash, string defaultValue = null)
		{
			object	Item = m_Values[keyhash];
			
			if(Item != null)
				return((string)Item);
			else
				return(defaultValue);
		}

		internal void SetInt(int keyhash, int value)
		{
			m_Values[keyhash] = value;
		}

		internal void SetFloat(int keyhash, float value)
		{
			m_Values[keyhash] = value;
		}

		internal void SetString(int keyhash, string value)
		{
			m_Values[keyhash] = value;
		}

		internal bool HasKey(int keyhash)
		{
			return(m_Values[keyhash] != null);
		}
	}

	public class OurSaveState : TMSaveState
	{
		public static OurSaveState		Instance { get { return sInstance; } }
		static OurSaveState				sInstance;

		GameSaveState					m_GameSaveState;
		bool							m_GameSaveStateDirty;

		OurSaveState() : base()
		{
			sInstance = this;
			try
			{
				m_GameSaveState = LoadObject("SaveState") as GameSaveState;
			}
			catch
			{
				m_GameSaveState = null;
			}

			if(m_GameSaveState != null)
			{
				if(VerifyHash() == false)
					m_GameSaveState = null;
				else
				{
					m_GameSaveState.m_HowManyTimesRun++;

					if(m_GameSaveState.m_BuildNumber != GameManager.sBuildNumber)
					{
						// Do any build change updates
						m_GameSaveState.m_BuildNumber = GameManager.sBuildNumber;
					}
				}
			}

			if(m_GameSaveState == null)
			{
				m_GameSaveState = new GameSaveState();
				m_GameSaveState.m_BuildNumber = GameManager.sBuildNumber;
			}
			SaveState();
			m_GameSaveStateDirty = false;
		}

		public static void Create()
		{
			if (sInstance == null)
				sInstance = new OurSaveState();
		}

		private bool VerifyHash()
		{
			string			SuppliedHash = m_GameSaveState.m_HashString;
			string			CalculatedHash = GetSecureHash();

			return(SuppliedHash == CalculatedHash);
		}

		public void SaveState()
		{
			if (m_GameSaveStateDirty)
			{
				GetSecureHash();
				SaveObject("SaveState", m_GameSaveState);
			}
		}

		public void Reset()
		{
			ResetObject("SaveState");
		}

		string GetSecureHash()
		{
			using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
			{
				// It gives us a byte array, so we'll convert it to a Base64
				// encoded string

				System.IO.MemoryStream		memoryStream = new System.IO.MemoryStream();

				m_GameSaveState.m_HashString = null;
				bf.Serialize(memoryStream, m_GameSaveState);

				byte[] hash = md5.ComputeHash(memoryStream);

				m_GameSaveState.m_HashString = Convert.ToBase64String(hash);
				return(m_GameSaveState.m_HashString);
			}
		}

		internal string GetBuildInfoString()
		{
#if DEMO_BUILD
			return ("Elementals Demo Build: " + m_GameSaveState.m_BuildNumber.ToString());
#elif EARLY_ACCESS
			return ("Elementals Early Access Build: " + m_GameSaveState.m_BuildNumber.ToString());
#else
			return ("Elementals prototype");
#endif
		}

// New get set methods that go into the protected save area
		public int GetIntS(string key, int defaultValue = 0)
		{
			return(GetIntS(key.GetHashCode(), defaultValue));
		}
		public int GetIntS(int keyhash, int defaultValue = 0)
		{
			return(m_GameSaveState.GetInt(keyhash, defaultValue));
		}

		public float GetFloatS(string key, float defaultValue = 0)
		{
			return(GetFloatS(key.GetHashCode(), defaultValue));
		}
		public float GetFloatS(int key, float defaultValue = 0)
		{
			return(m_GameSaveState.GetFloat(key, defaultValue));
		}

		public string GetStringS(string key, string defaultValue = null)
		{
			return(GetStringS(key.GetHashCode(), defaultValue));
		}
		public string GetStringS(int key, string defaultValue = null)
		{
			return(m_GameSaveState.GetString(key, defaultValue));
		}

		public void SetIntS(string key, int value)
		{
			SetIntS(key.GetHashCode(), value);
		}
		public void SetIntS(int key, int value)
		{
			m_GameSaveState.SetInt(key, value);
			m_GameSaveStateDirty = true;
		}

		public void SetFloatS(string key, float value)
		{
			SetFloatS(key.GetHashCode(), value);
		}
		public void SetFloatS(int key, float value)
		{
			m_GameSaveState.SetFloat(key, value);
			m_GameSaveStateDirty = true;
		}

		public void SetStringS(string key, string value)
		{
			SetStringS(key.GetHashCode(), value);
		}
		public void SetStringS(int key, string value)
		{
			m_GameSaveState.SetString(key, value);
			m_GameSaveStateDirty = true;
		}

		public void AddIntS(string key, int value)
		{
			AddIntS(key.GetHashCode(), value);
		}
		public void AddIntS(int key, int value)
		{
			value += m_GameSaveState.GetInt(key);
			m_GameSaveState.SetInt(key, value);
			m_GameSaveStateDirty = true;
		}

		public void AddFloatS(string key, int value)
		{
			AddFloatS(key.GetHashCode(), value);
		}
		public void AddFloatS(int key, float value)
		{
			value += m_GameSaveState.GetFloat(key);
			m_GameSaveState.SetFloat(key, value);
			m_GameSaveStateDirty = true;
		}

		public bool HasKeyS(string key, int value)
		{
			return m_GameSaveState.HasKey(key.GetHashCode());
		}
		public bool HasKeyS(int key)
		{
			return m_GameSaveState.HasKey(key);
		}
	}
