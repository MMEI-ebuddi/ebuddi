using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateActionListScriptableObject {

    [MenuItem("EbolaApp/Create/Action List")]
	public static void CreateAsset()
    {
        ActionListScriptableObject newActionListAsset = ScriptableObject.CreateInstance<ActionListScriptableObject>();

        AssetDatabase.CreateAsset(newActionListAsset, "Assets/Actions/Lists/NewActionList.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = newActionListAsset;
    }
}
