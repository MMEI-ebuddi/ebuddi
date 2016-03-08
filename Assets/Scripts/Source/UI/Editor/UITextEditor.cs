using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(UIText))]
public class UITextEditor : Editor
{
	UIText		m_UIText;

	void OnEnable() 
	{
		m_UIText = target as UIText;

	}

	void OnCreate()
	{
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUI.changed)
		{ 
			EditorUtility.SetDirty(m_UIText);
		}
	}
}
