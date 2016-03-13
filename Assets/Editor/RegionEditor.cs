using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(LightProbeRegion))]
public class RegionEditor : Editor
{
	LightProbeRegion region;
	
	public void OnEnable()
	{
		region = (LightProbeRegion)target;
	}
	
	public override void OnInspectorGUI()
	{
	    GUILayout.BeginHorizontal();
	    GUILayout.Label(" Height Probe Density ");
	    region.hDensity = EditorGUILayout.FloatField(region.hDensity, GUILayout.Width(50));
	    GUILayout.EndHorizontal();
	 
	    GUILayout.BeginHorizontal();
	    GUILayout.Label(" Width Probe Density ");
	    region.wDensity = EditorGUILayout.FloatField(region.wDensity, GUILayout.Width(50));
	    GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
	    GUILayout.Label(" Depth Probe Density ");
	    region.dDensity = EditorGUILayout.FloatField(region.dDensity, GUILayout.Width(50));
	    GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
	    GUILayout.Label(" Treat Density Values as Probes Per Axis ");
	    region.Distribute = EditorGUILayout.Toggle(region.Distribute, GUILayout.Width(50));
	    GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
	    GUILayout.Label(" Show Density Field ");
	    region.ShowDensityField = EditorGUILayout.Toggle(region.ShowDensityField, GUILayout.Width(50));
	    GUILayout.EndHorizontal();
	 
	    if (GUILayout.Button("Calculate Light Probe Mesh", GUILayout.Width(255)))
	    {
	       region.GenerateMesh();
	    }
		
		if (GUILayout.Button("Delete Light Probe Mesh", GUILayout.Width(255)))
	    {
	       region.ClearMesh();
	    }
	 
	    SceneView.RepaintAll();
	}
}

