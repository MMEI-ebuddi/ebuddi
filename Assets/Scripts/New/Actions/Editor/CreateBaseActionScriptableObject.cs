using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateBaseActionScriptableObject {

    [MenuItem("EbolaApp/Create/Action")]
	public static void CreateAsset()
    {
        BaseActionScriptableObject newActionAsset = ScriptableObject.CreateInstance<BaseActionScriptableObject>();

        AssetDatabase.CreateAsset(newActionAsset, "Assets/Actions/Actions/NewAction.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = newActionAsset;
    }
}
