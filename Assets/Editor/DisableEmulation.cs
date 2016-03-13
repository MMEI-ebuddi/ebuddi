using UnityEngine;
using UnityEditor;

[InitializeOnLoad]

public class EditInitialSetting
{
	static EditInitialSetting()
	{
		EditorApplication.update += Update;
	}
	
	static void Update()
	{
		bool isSuccess = EditorApplication.ExecuteMenuItem("Edit/Graphics Emulation/No Emulation");
		if (isSuccess)
			EditorApplication.update -= Update;
	}
}