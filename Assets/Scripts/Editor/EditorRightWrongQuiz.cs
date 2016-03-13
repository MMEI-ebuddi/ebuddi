using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RightWrongQuiz))]
public class EditorRightWrongQuiz : Editor {


	public override void OnInspectorGUI() {

		RightWrongQuiz quiz = (RightWrongQuiz)target;

		base.DrawDefaultInspector();

		EditorGUILayout.BeginHorizontal();

		GUILayout.Box(AssetPreview.GetAssetPreview(quiz.correctImage));
		GUILayout.Box(AssetPreview.GetAssetPreview(quiz.incorrectImage));

		EditorGUILayout.EndHorizontal();

	}




}
