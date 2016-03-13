using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateConversationScriptableObject {

    [MenuItem("EbolaApp/Create/Conversation")]
	public static void CreateAsset()
    {
        Conversation convo = ScriptableObject.CreateInstance<Conversation>();

		AssetDatabase.CreateAsset(convo, "Assets/Resources/Conversations/conversation.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

		Selection.activeObject = convo;
    }
}
