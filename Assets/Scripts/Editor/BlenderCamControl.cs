// BlenderCameraControls.cs
// by Marc Kusters (Nighteyes)
// 
//Numpad1 = Front view
//Control + Numpad1 = Rear view 
//Numpad2 = Rotate view down
//Numpad3 = Right view
//Control + Numpad3 = Left view 
//Numpad4 = Rotate view left
//Numpad5 = Switch between orthographic and perspective
//Numpad6 = Rotate view right
//Numpad7 = Top view
//Control + Numpad7 = Down view
//Numpad8 = Move view up
//Numpad. = Center view on object(s) Note:only works on objects that have the CanEditMultipleObjects property
//Numpad- = Zoom camera out
//Numpad+ = Zoom camera in
// Usage: Select any object to use the camera hotkeys. 
//
 
using UnityEngine;
using UnityEditor;
using System.Collections;
 
[CustomEditor(typeof(Transform))]
public class BlenderCamControl : Editor
{
    UnityEditor.SceneView sceneView;
 
    private Vector3 eulerAngles;
    private Event current;
    private Quaternion rotHelper;

	public override void OnInspectorGUI()
	{
		Transform t = (Transform)target;
	
		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginHorizontal();
		Vector3 position = EditorGUILayout.Vector3Field("Position", t.localPosition );
		if (GUILayout.Button("R", GUILayout.Width(20) ))
		{
			position.Set(0,0,0);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		Vector3 eulerAngles = EditorGUILayout.Vector3Field("Rotation", t.localEulerAngles);
		if (GUILayout.Button("R", GUILayout.Width(20) ))
		{
			eulerAngles.Set(0,0,0);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		Vector3 scale = EditorGUILayout.Vector3Field("Scale", t.localScale);
		if (GUILayout.Button("R", GUILayout.Width(20) ))
		{
			scale.Set(1,1,1);
		}
		EditorGUILayout.EndHorizontal();

// locks but need to unlock from another script. off until better solution maybe..		
//		EditorGUILayout.BeginHorizontal();
//		if (GUILayout.Button("Lock", GUILayout.Width(100) ))
//		{
//			t.hideFlags = HideFlags.NotEditable;
//		}
//
//		if (GUILayout.Button("UnLock", GUILayout.Width(100) ))
//		{
//			t.hideFlags = 0;
//		}
//		EditorGUILayout.EndHorizontal();

 
		if (GUI.changed)
		{
			Undo.RecordObject(t, "Transform Change");
 
			t.localPosition = FixIfNaN(position);
			t.localEulerAngles = FixIfNaN(eulerAngles);
			t.localScale = FixIfNaN(scale);
		}
	}
 
	private Vector3 FixIfNaN(Vector3 v)
	{
		if (float.IsNaN(v.x))
		{
			v.x = 0;
		}
		if (float.IsNaN(v.y))
		{
			v.y = 0;
		}
		if (float.IsNaN(v.z))
		{
			v.z = 0;
		}
		return v;
	}
	
	
    public void OnSceneGUI()
    {
 
        current = Event.current;
 
        if (!current.isKey || current.type != EventType.keyDown)
            return;
 
        sceneView = UnityEditor.SceneView.lastActiveSceneView;
        eulerAngles = sceneView.camera.transform.rotation.eulerAngles;
        rotHelper = sceneView.camera.transform.rotation;
 
        switch (current.keyCode)
        {
            case KeyCode.Keypad1:
                if (current.control == false)
                    sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(0f, 360f, 0f)));
                else
                    sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(0f, 180f, 0f)));
                break;
            case KeyCode.Keypad2:
                sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, rotHelper * Quaternion.Euler(new Vector3(-15f, 0f, 0f)));
                break;
            case KeyCode.Keypad3:
                if (current.control == false)
                    sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(0f, 270f, 0f)));
                else
                    sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
                break;
            case KeyCode.Keypad4:
                sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y + 15f, eulerAngles.z)));
                break;
            case KeyCode.Keypad5:
                sceneView.orthographic = !sceneView.orthographic;
                break;
            case KeyCode.Keypad6:
                sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y - 15f, eulerAngles.z)));
                break;
            case KeyCode.Keypad7:
                if (current.control == false)
                    sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(90f, 0f, 0f)));
                else
                    sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, Quaternion.Euler(new Vector3(270f, 0f, 0f)));
                break;
            case KeyCode.Keypad8:
                sceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, rotHelper * Quaternion.Euler(new Vector3(15f, 0f, 0f)));
                break;
            case KeyCode.KeypadPeriod:
                if (Selection.transforms.Length == 1)
                    sceneView.LookAtDirect(Selection.activeTransform.position, sceneView.camera.transform.rotation);
                else if (Selection.transforms.Length > 1)
                {
                    Vector3 tempVec = new Vector3();
                    for (int i = 0; i < Selection.transforms.Length; i++)
                    {
                        tempVec += Selection.transforms[i].position;
                    }
                    sceneView.LookAtDirect((tempVec / Selection.transforms.Length), sceneView.camera.transform.rotation);
                }
                break;
            case KeyCode.KeypadMinus:
                SceneView.RepaintAll();
                sceneView.size *= 1.1f;
                break;
            case KeyCode.KeypadPlus:
                SceneView.RepaintAll();
                sceneView.size /= 1.1f;
                break;
        }
    }
}