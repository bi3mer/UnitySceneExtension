using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateSceneMenus : Editor
{
	public static string filePath = "Assets/Editor/SceneMenu.cs";
	public static string openingString = "using UnityEngine;using System.Collections;using UnityEditor;"
		+ "public class SceneMenu : Editor {";
	
	public static string endingString = "public static void OpenScene(string scene){ " 
		+ "if(EditorApplication.SaveCurrentSceneIfUserWantsTo()){"
			+ "EditorApplication.OpenScene(scene);}}}";
	
	[MenuItem("Open Scene/Create Scenes")]
	public static void CreateScenes()
	{
		// Start string
		string completeFile = CreateSceneMenus.openingString;

		// get all paths
		string[] paths = AssetDatabase.GetAllAssetPaths();

		// loop through each path
		for(int i = 0; i < paths.Length; ++i)
		{
			// check if this is a scene
			if(paths[i].Contains(".unity"))
			{
				// add scene header
				completeFile += "[MenuItem(\"Open Scene/" + paths[i] + "\")]";

				// create function with unique name
				completeFile += "public static void Open" + i.ToString() + "(){SceneMenu.OpenScene(\"" + paths[i] + "\");}";
			}
		}
		
		// add ending
		completeFile += CreateSceneMenus.endingString;
		
		// print file to file
		System.IO.StreamWriter writer = System.IO.File.CreateText(CreateSceneMenus.filePath);
		writer.Write(completeFile);
		writer.Flush();
		writer.Close();

		// Re build unity
		AssetDatabase.Refresh();
	}
}
