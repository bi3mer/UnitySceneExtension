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
		foreach(string path in AssetDatabase.GetAllAssetPaths())
		{
			// check it this is a unity scene
			if(path.Contains(".unity"))
			{
				// add scene header
				completeFile += "[MenuItem(\"Open Scene/" + path + "\")]";
				
				// add static function
				string[] splitPath = path.Split('/');
				string stringAndExtension = splitPath[splitPath.Length - 1];
				string[] splitName = stringAndExtension.Split('.');
				
				// remove whitespace from string
				string completedName = System.Text.RegularExpressions.Regex.Replace(splitName[0], " ", "");
				
				// create function with correct name
				completeFile += "public static void Open" + completedName + "(){SceneMenu.OpenScene(\"" + path + "\");}";
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
