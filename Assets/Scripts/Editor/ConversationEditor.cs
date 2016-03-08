using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Reflection;


[CustomEditor(typeof(Conversation))]
public class ConversationEditor : Editor {
	

	public override void OnInspectorGUI() {

		Conversation conversation = (Conversation)target;

		EditorGUILayout.PropertyField(serializedObject.FindProperty("language"), true);
		conversation.audioClip = EditorGUILayout.ObjectField("Audio Clip", conversation.audioClip, typeof(AudioClip), true) as AudioClip;

		EditorStyles.textField.wordWrap = true;
		conversation.script = EditorGUILayout.TextArea(conversation.script, GUILayout.Height(200));


		if(GUILayout.Button("Preview audio")){
			PlayClip(conversation.audioClip);
		}

		if(GUILayout.Button("Save")){
			EditorUtility.SetDirty(conversation);
		}

//		EditorUtility.SetDirty(conversation);
	
	}


	public void PlayClip(AudioClip clip) {

		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		MethodInfo method = audioUtilClass.GetMethod(
			"PlayClip",
			BindingFlags.Static | BindingFlags.Public,
			null,
			new System.Type[] {
			typeof(AudioClip)
		},
		null
		);
		method.Invoke(
			null,
			new object[] {
			clip
		}
		);
	}




}
