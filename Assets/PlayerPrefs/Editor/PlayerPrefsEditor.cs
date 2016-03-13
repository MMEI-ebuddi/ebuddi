//	PlayerPrefs Unity Editor Window
//
//	Copyright (c) 2013 Fuzzy Logic (info@fuzzy-logic.co.za)
//	
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//	THE SOFTWARE.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using Microsoft.Win32;

/// <summary> Editor window that displays editable list of entries stored in player prefs </summary>
public class PlayerPrefsEditor : EditorWindow
{
	#region FIELDS
	/// <summary> Key/value pairs read from disk </summary>
	Dictionary<string, object>			m_Plist;
	
	/// <summary> Keeps track of position for scroll view </summary>
	Vector2								m_ScrollPos;

	/// <summary> Strings used for case insensitive filter display of entries </summary>
	string								m_FilterIndex = "";
	string								m_FilterKey = "";
	string								m_FilterValue = "";
	
	/// <summary> Used to keep track of play state changes </summary>
	bool								m_PrevPlayState = false;
	
	/// <summary> Indicates whether values can be changed during play mode </summary>
	bool								m_CanEditInPlayMode = false;
	#endregion
	
	#region FUNCTIONS
	/// <summary> Gets called by system, responsible for creating/opening editor window </summary>
	[MenuItem ("Window/PlayerPrefs")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		PlayerPrefsEditor window = (PlayerPrefsEditor) EditorWindow.GetWindow (typeof(PlayerPrefsEditor));
		window.title = "PlayerPrefs";
	}
	
	/// <summary>
	/// Called after window is created 
	/// (interestingly, it seems the window gets recreated whenever you press play...) 
	/// </summary>
	void OnEnable()
	{
		// If first time, load list of player prefs keys
		if (m_Plist == null)
		{
			Load();		
		}
		
		// We want to know whenever the play state changes (this is so that we can reload
		// the prefs list when coming out of play mode)
		EditorApplication.playmodeStateChanged += OnPlayModeStateChanged;
	}
	
	/// <summary> Called when window is about to be destroyed </summary>
	void OnDisable()
	{
		// Clean up by unsubscribing from event
		EditorApplication.playmodeStateChanged -= OnPlayModeStateChanged;
	}
	
	/// <summary> Notification whenever editor play mode changes </summary>
	void OnPlayModeStateChanged()
	{
		// This gets called twice whenever the state changes
		// When going from stopped -> play, isPlaying is first false, then true
		// When going from play -> stop, isPlaying is first true, then false
		if (m_PrevPlayState != EditorApplication.isPlaying)
		{
			m_PrevPlayState = EditorApplication.isPlaying;
			
			// Have we now entered "stop" mode?
			if (!m_PrevPlayState)
			{
				// Yes, so reload list of player prefs keys, as it could have changed
				Load();
			}
		}
	}
	
	/// <summary>
	/// Gets called at 10fps, allows us to update the display of the window regularly 
	/// (as opposed to just when window has focus)
	/// This should allow us to see any changes in player prefs values during play mode
	/// </summary>
	void OnInspectorUpdate() 
	{
		if (EditorApplication.isPlaying)
		{
			Repaint();			
		}
	}
	
	/// <summary> GUI display logic </summary>
	void OnGUI() 
	{
		bool changesMade = false;
		
		// Work out element widths based on window width
		float widthIndex = 30.0f;
		float widthButton = 25.0f;
		float widthWindow = this.position.width;
		float widthKey = (widthWindow - widthIndex) * 0.3f;
		float widthValue = widthWindow - widthIndex - widthKey - widthButton - 33.0f;
		
		if (m_Plist != null)
		{			
			// Search filter
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Search:");
				m_CanEditInPlayMode = EditorGUILayout.Toggle("Editable in Play Mode", m_CanEditInPlayMode, GUILayout.Width(180.0f));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
				m_FilterIndex = EditorGUILayout.TextField(m_FilterIndex, GUILayout.Width(widthIndex));
				m_FilterKey = EditorGUILayout.TextField(m_FilterKey, GUILayout.Width(widthKey));
				m_FilterValue = EditorGUILayout.TextField(m_FilterValue, GUILayout.MaxWidth(widthValue));
			EditorGUILayout.EndHorizontal();
			
			m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
		
			// Disable GUI in play mode, as we don't want to potentially interfere with changes made by the game code
			GUI.enabled = m_CanEditInPlayMode || !EditorApplication.isPlaying;
			
			// Keep track of index through iteration for display purposes
			int i = 1;
			
			int filterIndex = -1;
			if (!string.IsNullOrEmpty(m_FilterIndex))
			{
				System.Int32.TryParse(m_FilterIndex, out filterIndex);
			}
			
			// Use sorted copy of dictionary for display purposes (allows us to also change dictionary in place)
			SortedDictionary<string, object> copy = new SortedDictionary<string, object>(m_Plist);
			foreach (KeyValuePair<string, object> entry in copy)
			{
				// Only display entries that match filters (or if no filters specified, show all)
				if ( ((filterIndex == - 1) || (i == filterIndex)) &&
					 (string.IsNullOrEmpty(m_FilterKey) || (entry.Key.IndexOf(m_FilterKey, System.StringComparison.OrdinalIgnoreCase) >= 0)) &&
					 (string.IsNullOrEmpty(m_FilterValue) || (entry.Value.ToString().IndexOf(m_FilterValue, System.StringComparison.OrdinalIgnoreCase) >= 0))
					)
				{
					EditorGUILayout.BeginHorizontal();
					
						// Index
						EditorGUILayout.LabelField(i.ToString() + ".", GUILayout.Width(widthIndex));
						
						// Label
						EditorGUILayout.LabelField(entry.Key, GUILayout.Width(widthKey));
						
						// Value
						changesMade |= OnGUIEntryValue(entry, widthValue);
						
						// Remove button
						bool clicked = Button("X", widthButton, Color.red);
						if (clicked)
						{
							if (EditorUtility.DisplayDialog("Delete entry", "Are you sure you want to delete " + entry.Key + "?", "Yes", "No"))
							{
								m_Plist.Remove(entry.Key);
								PlayerPrefs.DeleteKey(entry.Key);
								changesMade = true;
							}
						}
					
					EditorGUILayout.EndHorizontal();					
				}
				
				// Increase counter
				i++;
			}	
			
			GUI.enabled = true;
			
			EditorGUILayout.EndScrollView();				
		}
		
		// If any changes have been made to any values, then save the playerprefs immediately
		if (changesMade)
		{
			Save();
		}
	}
	
	/// <summary> 
	/// Handles GUI display for a specific player pref, taking type into account
	/// TODO: look into using generics to avoid the duplicated code for each type
	/// </summary>
	bool OnGUIEntryValue(KeyValuePair<string, object> entry, float width)
	{
		System.Type valueType = entry.Value.GetType();
		if (valueType == typeof(int))
		{
			int newValue = EditorGUILayout.IntField(PlayerPrefs.GetInt(entry.Key), GUILayout.MaxWidth(width));
			if (newValue != (int)entry.Value)
			{
				m_Plist[entry.Key] = newValue;
				PlayerPrefs.SetInt(entry.Key, newValue);
				return true;
			}
		}
		else if (valueType == typeof(float))
		{
			float newValue = EditorGUILayout.FloatField(PlayerPrefs.GetFloat(entry.Key), GUILayout.MaxWidth(width));
			if (newValue != (float)entry.Value)
			{
				m_Plist[entry.Key] = newValue;
				PlayerPrefs.SetFloat(entry.Key, newValue);
				return true;
			}						
		}
		else if (valueType == typeof(string))
		{
			string newValue = EditorGUILayout.TextField(PlayerPrefs.GetString(entry.Key), GUILayout.MaxWidth(width));
			if (newValue != (string)entry.Value)
			{
				m_Plist[entry.Key] = newValue;
				PlayerPrefs.SetString(entry.Key, newValue);
				return true;
			}
		}
		else
		{
			// Type not supported
			EditorGUILayout.LabelField("(editing of type " + valueType.ToString() + " not supported)", GUILayout.MaxWidth(width));
		}
		
		// No change made
		return false;
	}
	
	/// <summary> Renders a button </summary>
	bool Button(string label, float width, Color color)
	{
		GUI.backgroundColor = color;
		bool result = GUILayout.Button(label, GUILayout.Width(width));
		GUI.backgroundColor = Color.white;
		return result;
	}	

	/// <summary>
	/// Loads the dictionary of keys and their values/types from disk.
	/// Note we have to do this, because although Unity loads PlayerPrefs automatically
	/// there is no way to iterate through all the keys in the PlayerPrefs, so we have to
	/// manually load the file from disk.
	/// </summary>
	void Load()
	{
		// The following operations can throw exceptions
		try
		{
			// On Windows, read from the registry
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				m_Plist = new Dictionary<string, object>();
				string keyName = "Software\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName;
				RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName);
				if (key != null)
				{
					string[] valueNames = key.GetValueNames();
					foreach (string s in valueNames)
					{
						// Remove the "_h...." part from the end of the name of the value
						string valueName = s;
						int i = valueName.LastIndexOf("_");
						if (i >= 0)
						{	
							valueName = s.Remove(i);
						}
	
						m_Plist.Add(valueName, key.GetValue(s));
					}
				}
			}
			// On OS, read from "plist" file in preferences folder
			else if (Application.platform == RuntimePlatform.OSXEditor)
			{
				// Uses open source Plist code from https://github.com/animetrics/PlistCS:
				string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/Library/Preferences/";
				string fullPath = path + "unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist";			
				m_Plist = (Dictionary<string, object>) PlistCS.Plist.readPlist(fullPath);
			}
			
			//Validate();
		}
		catch
		{
			// No need to notify user
		}
	}
	
	/// <summary> Ensures that all player prefs entries are supported </summary>
	void Validate()
	{
		if (m_Plist != null)
		{
			foreach (KeyValuePair<string, object> entry in m_Plist)
			{
				System.Type valueType = entry.Value.GetType();
				if ( (valueType != typeof(int)) &&
					(valueType != typeof(float)) &&
					(valueType != typeof(string)) )
				{
					Debug.LogWarning("PlayerPrefs Editor: entry '" + entry.Key.ToString() + "' type (" + valueType.ToString() + ") is not supported");
				}
			}			
		}
	}
	
	/// <summary> Saves latest values back into playerprefs </summary>
	void Save()
	{
		PlayerPrefs.Save();
	}
	#endregion
}
