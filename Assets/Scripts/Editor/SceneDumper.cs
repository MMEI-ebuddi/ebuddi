// SceneDumper.cs
// http://wiki.unity3d.com/index.php?title=SceneDumper
// History:
// version 1.0 - December 2010 - Yossarian King
 
using StreamWriter = System.IO.StreamWriter;
 
using UnityEngine;
using UnityEditor;
 
public static class SceneDumper
{
    [MenuItem("Tools/Dump Scene")]
    public static void DumpScene()
    {
        if ((Selection.gameObjects == null) || (Selection.gameObjects.Length == 0))
        {
            Debug.LogError("Please select the object(s) you wish to dump.");
            return;
        }
 
		string sceneName = EditorApplication.currentScene.Replace('/','_');
		sceneName = sceneName.Replace( ".unity", ".txt" );
        string filename = @"Dump_" + sceneName;
 
        Debug.Log("Dumping scene to " + filename + " ...");
        using (StreamWriter writer = new StreamWriter(filename, false))
        {
            foreach (GameObject gameObject in Selection.gameObjects)
            {
                DumpGameObject(gameObject, writer, "");
            }
        }
        Debug.Log("Scene dumped to " + filename);
    }
 
    public static void DumpSceneWhole()
    {
		string sceneName = EditorApplication.currentScene.Replace('/','_');
		sceneName = sceneName.Replace( ".unity", ".txt" );
        string filename = @"Dump_" + sceneName;
 
		GameObject [] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();


        Debug.Log("Dumping scene to " + filename + " ...");
        using (StreamWriter writer = new StreamWriter(filename, false))
        {
            foreach (GameObject gameObject in allObjects)
            {
				if( gameObject.transform.parent == null )
					DumpGameObject(gameObject, writer, "");
            }
        }
        Debug.Log("Scene dumped to " + filename);
    }
 

    private static void DumpGameObject(GameObject gameObject, StreamWriter writer, string indent)
    {
        writer.WriteLine("{0}+{1}", indent, gameObject.name);
 
        foreach (Component component in gameObject.GetComponents<Component>())
        {
            DumpComponent(component, writer, indent + "  ");
        }
 
        foreach (Transform child in gameObject.transform)
        {
            DumpGameObject(child.gameObject, writer, indent + "  ");
        }
    }
 
    private static void DumpComponent(Component component, StreamWriter writer, string indent)
    {
        writer.WriteLine("{0}{1}", indent, (component == null ? "(null)" : component.GetType().Name));
    }
}

