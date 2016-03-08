using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


#region Actions on a Selection		ReplaceSelectionWithPrefab		 MaterialInfo		ToggleSelectionActive


public class ReplaceSelectionWithPrefab : ScriptableObject 
{
	[MenuItem ("Total Monkery/Tools/Replace Selection With Prefab %&#p" , false , 1)]
	public static void Replace()
	{
		GameObject prefab = new List<Object>(Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets)).Find(x => {return AssetDatabase.IsMainAsset(x);}) as GameObject;
		if(prefab == null)
		{
			return;	
		}
		
		Object[] sceneTransforms = Selection.GetFiltered(typeof(Transform), SelectionMode.ExcludePrefab);
		Undo.RecordObjects(sceneTransforms, "Replace Selection with Prefab");
		foreach(Transform t in sceneTransforms)
		{
			GameObject instantiatedPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
			instantiatedPrefab.transform.parent = t.parent;
			instantiatedPrefab.transform.localPosition = t.localPosition;
			instantiatedPrefab.transform.localRotation = t.localRotation;
			instantiatedPrefab.transform.localScale = t.localScale;
			DestroyImmediate(t.gameObject);
		}
	}
}
#if false

public class AddMaterialInfoToSelection : Editor 
{
	[MenuItem ("Total Monkery/Tools/Add Material Info To Selection" , false , 4)]
	public static void Toggle()
	{
		Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable | SelectionMode.TopLevel);
		for(int i=0; i<selection.Length; i++)
		{
			GameObject go = selection[i] as GameObject;
			if(go == null) { continue; }

			Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/MaterialInfoDisplay.prefab", typeof(GameObject));

			GameObject clone = Instantiate(prefab, go.transform.position + (Vector3.up * 2), Quaternion.identity) as GameObject;

			clone.name = "MaterialInfo";

			clone.transform.parent = go.transform;
	
		}
	}
}

public class RemoveMaterialInfoFromSelection : Editor 
{
	[MenuItem ("Total Monkery/Tools/Remove Material Info From Selection"  , false , 5)]
	public static void Toggle()
	{
		Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable | SelectionMode.TopLevel);
		for (int i = 0; i < selection.Length; i++)
		{
			GameObject go = selection[i] as GameObject;
			if (go == null) { continue; }

			if (go.GetComponentInChildren<InfoDisplay>().gameObject.name == "MaterialInfo")
			{
				DestroyImmediate(go.GetComponentInChildren<InfoDisplay>().gameObject);
			}
		}
	}
}
#endif

public class ToggleSelectionActive : Editor {

	[MenuItem ("Total Monkery/Tools/Toggle Selection Active %#g" , false , 2 )]
	public static void Toggle()
	{
		Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable | SelectionMode.TopLevel);
		for(int i=0; i<selection.Length; i++)
		{
			GameObject go = selection[i] as GameObject;
			if(go == null) { continue; }
			go.SetActive(!(go.activeSelf));
		}
	}
}

#endregion

#region Single Item Actions			RendererBoundsSize

public class MoveToOrigin : Editor 
{
	[MenuItem ("Total Monkery/Tools/MoveToOrigin" , false , 4 )]
	public static void Toggle()
	{
		Vector3 m_DistanceFromOrigin;

		Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable | SelectionMode.TopLevel);
		for(int i=0; i<selection.Length; i++)
		{
			GameObject go = selection[i] as GameObject;
			if(go == null || go.transform.position == Vector3.zero)			  //|| go.transform.parent != null 
			{ 
				Debug.Log("Failed to move" + go.name);
				continue; 			
			}
			m_DistanceFromOrigin = go.transform.position;

			Debug.Log(go.name + " is at " + m_DistanceFromOrigin);

			go.transform.position -= m_DistanceFromOrigin;

			if (go.transform.childCount > 0)
			{ 
				for(int loop = 0; loop < go.transform.childCount; loop++)
				{
					Transform Child = go.transform.GetChild(loop);
					Child.localPosition += m_DistanceFromOrigin;
				}
			}
		}
	}
}

public class RendererBoundsSize : Editor {

	[MenuItem ("Total Monkery/Tools/Show Bounds Size %&s" , false , 3)]
	public static void Toggle()
	{
		Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable | SelectionMode.TopLevel);
		for(int i=0; i<selection.Length; i++)
		{
			GameObject go = selection[i] as GameObject;
			if(go == null) 
			{ 
				continue; 			
			}

			Debug.Log(go.name);
			if (go.GetComponent<Renderer>())
			{
				Debug.Log(go.GetComponent<Renderer>().bounds);
				Debug.Log(go.GetComponent<Renderer>().bounds.size);
			}
			else
			{
				Debug.Log("Has no Renderer");
			}
		}
	}
}


#endregion


#region Wizards

public class InvertSelection : ScriptableWizard 
{	
	[MenuItem ("Total Monkery/Tools/Invert Selection" , false, 4)]
	static void static_InvertSelection() { 
		
		List< GameObject > oldSelection = new List< GameObject >();
		List< GameObject > newSelection = new List< GameObject >();
				
		foreach( GameObject obj in Selection.GetFiltered( typeof( GameObject ), SelectionMode.ExcludePrefab ) )
			oldSelection.Add( obj );
		
		foreach( GameObject obj in FindObjectsOfType( typeof( GameObject ) ) )
		{
			if ( !oldSelection.Contains( obj ) )
				newSelection.Add( obj );
		}		
		Selection.objects = newSelection.ToArray();		
	}	
}

public class SelectScaledObjects : ScriptableWizard
{
	[MenuItem("Total Monkery/Tools/Select Scaled Objects")]
	static void static_SelectScaled()
	{		
		List< GameObject > newSelection = new List< GameObject >();
					
		foreach( GameObject obj in FindObjectsOfType( typeof( GameObject ) ) )
		{
			if (obj.transform.localScale != Vector3.one)
				newSelection.Add( obj );
		}
		Selection.objects = newSelection.ToArray();		
	}	
}
#endregion


#region CreateThings


public class CustomGameObject : Editor
{
	static GameObject CreatePrefabGameObject(string PrefabName, string DisplayName)
	{
		Object		Prefab = AssetDatabase.LoadAssetAtPath(PrefabName, typeof(GameObject));
		GameObject	Clone = PrefabUtility.InstantiatePrefab(Prefab) as GameObject;

		Clone.name = DisplayName;
		Clone.transform.position = Vector3.zero;
		Clone.transform.rotation = Quaternion.identity;
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = Clone;
		return(Clone);
	}

	// Add context menu named "Do Something" to rigid body's context menu
	[MenuItem("GameObject/Rememeber")]
	static public void Remember()
	{
	}

	[MenuItem("GameObject/Create Other/MagNets/Recycletron")]
	static public void CreateRecycletron()
	{
		CreatePrefabGameObject("Assets/Prefabs/Interactables/Recycletron V3.prefab", "Recycletron RENAME ME");
	}

	[MenuItem("GameObject/Create Other/MagNets/Health Pad")]
	static public void CreatePowerPlate()
	{
		CreatePrefabGameObject("Assets/Prefabs/Interactables/HealthPad.prefab", "HealthPad RENAME ME");
	}

	[MenuItem("GameObject/Create Other/MagNets/Charge Pillar")]
	static public void CreateChargePillar()
	{
		CreatePrefabGameObject("Assets/Prefabs/Interactables/ChargePillar.prefab", "ChargePillar RENAME ME");
	}

	[MenuItem("GameObject/Create Other/MagNets/Circuit")]
	static public void CreateCircuit()
	{
		CreatePrefabGameObject("Assets/Prefabs/Interactables/Circuit/GenericCircuit.prefab", "Circuit RENAME ME");
	}

	[MenuItem("GameObject/Create Other/MagNets/Circuit Hole")]
	static public void CreateCircuitHole()
	{
		CreatePrefabGameObject("Assets/Prefabs/Interactables/Circuit/GenericCircuitHole.prefab", "CircuitHole RENAME ME");
	}

	[MenuItem("GameObject/Create Other/MagNets/Circuit Fuse")]
	static public void CreateCircuitFuse()
	{
		CreatePrefabGameObject("Assets/Prefabs/Interactables/Circuit/CircuitFuse.prefab", "CircuitFuse RENAME ME");
	}

// 	[MenuItem("GameObject/Create Empty Child #&n")]
// 	static void CreateEmptyChild()
// 	{
// 		GameObject		go = new GameObject("GameObject");
// 
// 		if (Selection.activeTransform != null)
// 		{
// 			go.transform.parent = Selection.activeTransform;
// 			go.transform.Translate(Selection.activeTransform.position);
// 			go.transform.localRotation = Quaternion.Euler(0, 0, 0);
// 		}
// 	}

	[MenuItem("GameObject/Create Empty Parent #&m", false, 0)]
	static void CreateEmptyParent()
	{
		GameObject		go = new GameObject("GameObject");
		Object[]		sceneTransforms = Selection.GetFiltered(typeof(Transform), SelectionMode.Unfiltered);

		Undo.RecordObjects(sceneTransforms, "Undo empty parent move");
		go.transform.parent = Selection.activeTransform.parent;
		go.transform.position = Vector3.zero;
		go.transform.localRotation = Quaternion.Euler(0, 0, 0);

		foreach (Transform t in sceneTransforms)
		{
			t.parent = go.transform;
		}

		Selection.activeGameObject = go;
	}
}

#endregion


#region 
#endregion