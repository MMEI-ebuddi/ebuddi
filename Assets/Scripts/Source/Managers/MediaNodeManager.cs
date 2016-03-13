using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public enum eLanguage
{
	English,
	LiberianEnglish,
	Krio,
	MAX
}

public class MediaNodeManager
{	

	static eLanguage			sLanguage = eLanguage.English;
	public static eLanguage		Language { get { return sLanguage; } }
	
	static string[]				sLanguageAudioDir = { "Speech/English/", "Speech/English_Liberian/", "Speech/Krio/" };
	static string[]				sLanguageResourceDir = { "English/", "English_Liberian/", "Krio/" };

	class SpeechNode
	{
		string		m_EnglishText;
		string		m_Text;
		string		m_AudioFilename;
		SpeechNode	m_Next;

		public SpeechNode(string EnglishText, string Text, string AudioFilename)
		{
			m_EnglishText = EnglishText;
			m_Text = Text;
			m_AudioFilename = AudioFilename;
			m_Next = null;
		}

		internal void GetTextAndAudio(ref string Text, ref string Audio)
		{
			if (sLanguage == eLanguage.English || sLanguage == eLanguage.LiberianEnglish)
				Text = m_EnglishText;
			else
				Text = m_Text;
			Audio = m_AudioFilename;
		}
		internal string GetText()
		{
			if (sLanguage == eLanguage.English || sLanguage == eLanguage.LiberianEnglish)
				return(m_EnglishText);
			else
				return(m_Text);
		}
		internal string GetAudio()
		{
			return (m_AudioFilename);
		}

		internal void SetNext(SpeechNode NextLine)
		{
			m_Next = NextLine;
		}

		internal SpeechNode GetNext()
		{
			return(m_Next);
		}
	}

	static Hashtable				sNodes = new Hashtable(1000);

	public static bool GetTextAndAudio(string IdentString, ref string Text, ref string Audio)
	{
		if (IdentString != null)
		{
            Debug.Log("=========GetTextAndAudio ident: " + IdentString);

			SpeechNode		Node = null;

            if (EquipmentManager.Instance != null && EquipmentManager.Instance.ProtectionRequired == ePPEType.Basic)
            {                
                Node = sNodes[(IdentString + "_Basic").GetHashCode()] as SpeechNode;
            }
          
            if (Node==null)
                Node = sNodes[IdentString.GetHashCode()] as SpeechNode;

			if(Node != null)
			{
				Node.GetTextAndAudio(ref Text, ref Audio);
				MediaClipManager.EnsureLoaded(Audio);
				return (true);
			}
		}
		return (false);
	}




	public static bool GetConversation(eLanguage language,  string ConversationId, ref Conversation conversation)
	{
		string resourcePath = "Conversations/" + language.ToString() + "/" + ConversationId;

		Conversation convo = Resources.Load(resourcePath, typeof(Conversation)) as Conversation;
		if (convo != null) conversation = convo;
		return (convo != null);
	}
	


	public static string GetText(string IdentString)
	{
		if (IdentString != null)
		{
			SpeechNode		Node = sNodes[IdentString.GetHashCode()] as SpeechNode;

			if (Node != null)
				return(Node.GetText());
		}
		return (null);
	}

	public static string GetAudio(string IdentString)
	{
		if (IdentString != null)
		{
			SpeechNode		Node = sNodes[IdentString.GetHashCode()] as SpeechNode;

			if (Node != null)
			{
				MediaClipManager.EnsureLoaded(Node.GetAudio());
				return (Node.GetAudio());
			}
		}
		return (null);
	}

	public static void LoadCSV(string Filename)
	{
		string		TSVData = TrackedResource.LoadXML(Filename);

		if(TSVData == null)
			return;

		// If no language setting, then set default to English
		if (PlayerPrefs.HasKey("language") == false)
		{
			PlayerPrefs.SetInt("language", (int)eLanguage.English);
			sLanguage = eLanguage.English;
		}
 		else
			sLanguage = (eLanguage)Mathf.Clamp(PlayerPrefs.GetInt("language"), 0, (int)eLanguage.MAX);

		string[]		Lines = TSVData.Split('\n');		
		Regex			CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

		string			LastBaseLine = "";
		SpeechNode		LastLineSpeechNode = null;
		int				NumberOfExtensions = 0;

		for(int line = 1; line < Lines.Length; line++)
		{
			SpeechNode		LineSpeechNode;
			String[]		Strings = CSVParser.Split(Lines[line]);
			string			EnglishText, Text;

			if(Strings.Length < 2 || Strings[0].Length == 0)
				continue;

			if (Strings.Length == 2)
				continue;

			EnglishText = Strings[1].TrimStart(' ', '"');
			EnglishText = EnglishText.TrimEnd('"');

			Text = Strings[2].TrimStart(' ', '"');
			Text = Text.TrimEnd('"');

			EnglishText = EnglishText.Replace("/n", "\n");
			Text = Text.Replace("/n", "\n");

			LineSpeechNode = new SpeechNode(EnglishText, Text, Strings[0]);
			sNodes[Strings[0].GetHashCode()] = LineSpeechNode;

			if(Strings[0].StartsWith(LastBaseLine))
			{
				string		ContinuationName;

				NumberOfExtensions++;
				ContinuationName = LastBaseLine + "_" + ((NumberOfExtensions + 1).ToString());
				// Possible extension
				if(String.Compare(Strings[0], ContinuationName) == 0)
				{
					LastLineSpeechNode.SetNext(LineSpeechNode);
				}
				else
				{
					LastBaseLine = Strings[0];
					NumberOfExtensions = 0;
				}
			}
			else
			{
				LastBaseLine = Strings[0];
				NumberOfExtensions = 0;
			}

			LastLineSpeechNode = LineSpeechNode;
		}
	}

	internal static string GetAudioDirectory()
	{
		return(sLanguageAudioDir[(int)sLanguage]);
	}

	internal static void TriggerMediaNode(string Ident)
	{
		string		Text = "";
		string		AudioFileName = "";

		if (GetTextAndAudio(Ident, ref Text, ref AudioFileName) == true)
		{
			MediaClipManager.Instance.Play(AudioFileName);
			UIManager.Instance.BringUpDialog(UIManager.eUIReferenceName.MainDialog, Text);
		}
	}

	internal static bool GotContinuation(string Ident)
	{
        SpeechNode Node = null;

        if (EquipmentManager.Instance != null && EquipmentManager.Instance.ProtectionRequired == ePPEType.Basic)
        {
            Node = sNodes[(Ident + "_Basic").GetHashCode()] as SpeechNode;
        }

        if(Node==null)
		    Node = sNodes[Ident.GetHashCode()] as SpeechNode;

		if(Node != null && Node.GetNext() != null)
			return(true);

		return (false);
	}

	internal static string GotContinuationIdent(string Ident)
	{
        SpeechNode Node = null;

        if (EquipmentManager.Instance != null && EquipmentManager.Instance.ProtectionRequired == ePPEType.Basic)
        {
            Node = sNodes[(Ident + "_Basic").GetHashCode()] as SpeechNode;
        }

        if (Node == null)
            Node = sNodes[Ident.GetHashCode()] as SpeechNode;
		
		if (Node != null && Node.GetNext() != null)
			return (Node.GetNext().GetAudio());

		return (null);
	}

	public static void ChangeLanguage(eLanguage NewLanguage)
	{
		if(NewLanguage == sLanguage)
			return;

		sLanguage = NewLanguage;
		OurSaveState.Instance.SetInt("language", (int)sLanguage);
		MediaClipManager.LanguageChanged();
	}

	public static string GetLanguageResourceFolder( int i )
	{
		return sLanguageResourceDir[i];
	}

	public static AudioClip SearchForAudioClipBasedOnLanguage( string name )
	{
		AudioClip clip = null;
		if (PlayerPrefs.HasKey("language") == true)
		{
			int lang = PlayerPrefs.GetInt("language");
			clip = (AudioClip)Resources.Load(MediaNodeManager.GetLanguageResourceFolder(lang) + name);
		}
		else
		{
			clip = (AudioClip)Resources.Load("English_Liberian/" + name);	// Default to English_Liberian as this has the most fully featured audio dataset
		}
		return clip;
	}


	public static AudioClip SearchForAudioClipAnyLanguage( string name )
	{
		for( int i=0; i<(int)eLanguage.MAX; ++i )
		{
			AudioClip clip = (AudioClip)Resources.Load( MediaNodeManager.GetLanguageResourceFolder(i) + name );
			if( clip != null )
			{
				Debug.LogWarning( "Language substitution for AudioClip named '" + name + "' as it does not exist in the chosen language." );
				return clip;
			}
		}
		return null;
	}

	public static AudioClip LoadAudioClipResource( string name )
	{
		AudioClip clip = (AudioClip)Resources.Load(name);
		if( clip == null )
		{
			clip = SearchForAudioClipBasedOnLanguage( name );
			if( clip == null )
			{
				clip = SearchForAudioClipAnyLanguage( name );
				if( clip == null )
				{
					Debug.LogError("Unable to load resource named " + name + " in any language set");
					return null;
				}
			}
		}
		return clip;
	}







}
