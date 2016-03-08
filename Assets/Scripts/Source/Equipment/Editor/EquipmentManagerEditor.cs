using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(EquipmentManager))]
public class EquipmentManagerEditor : Editor {


	public override void OnInspectorGUI() {

		EquipmentManager manager = (EquipmentManager)target;

		base.DrawDefaultInspector();




//		serializedObject.Update();
//
//	
//
//
//		serializedObject.ApplyModifiedProperties();


	}


}
