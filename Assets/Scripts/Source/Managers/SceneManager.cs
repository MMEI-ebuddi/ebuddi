using System;
using UnityEngine;
using TMSupport;

using System.Collections.Generic;

public class SceneManager
{
	public enum SceneId
	{
		Splash = 0,
		Title,
		Donning,
		Scene2,
		Doffing,
		Scene4,
		Infection,
		IntroToEbola,
		Triage
	};

	private static Dictionary<SceneId,string> m_idToName = new Dictionary<SceneId,string>
	{
		{SceneId.Splash,		"Splash"},
		{SceneId.Title,			"Title"},
		//{SceneId.Donning,		"Scene1_en_tr02"},
		{SceneId.Donning,		"Scene1"},
		{SceneId.Scene2,		"Scene2"},
		{SceneId.Doffing,		"Scene3"},
		{SceneId.Scene4,		"Triage"},
		{SceneId.Infection,		"infection_room"},
		{SceneId.IntroToEbola,	"introduction_to_ebola"},
	};
	

	public static string GetSceneNameFromIndex( SceneId id )
	{

		return m_idToName[id];
	}
}

