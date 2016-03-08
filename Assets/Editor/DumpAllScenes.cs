// DumpAllScenes.cs
// http://stackoverflow.com/questions/24533178/iterating-over-all-scenes-before-compilation
// History:
// Unknown

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

class DumpAllScenes : Editor
{
    private static List<string> FillLevels ()
    {
		List<string> scenes = new List<string>();

		foreach( EditorBuildSettingsScene s in EditorBuildSettings.scenes)
		{
			if( s.enabled )
				scenes.Add( s.path );
		}
        return scenes;
    }

    [MenuItem ("Tools/Dump All Scenes")]
    public static void  buildGame()
    {
        List<string> levels = FillLevels ();
        foreach (string level in levels)
		{
            EditorApplication.OpenScene( level );
			SceneDumper.DumpSceneWhole();
        }
    }
}
