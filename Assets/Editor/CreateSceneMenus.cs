using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateSceneMenus : Editor
{
	private static string filePath = "Assets/Editor/SceneMenu.cs";
	private static string openingString = "using UnityEngine;using System.Collections;using UnityEditor;\n"
		+ "public class SceneMenu : Editor {\n";
	
	private static string endingString = "public static void OpenScene(string scene){ " 
		+ "if(EditorApplication.SaveCurrentSceneIfUserWantsTo()){"
			+ "EditorApplication.OpenScene(scene);}}}\n";
	private static string assetFolder = "Assets/";

	/// <summary>
	/// This will create a string that represents a function that can be called
	/// by the navigation bar. The index is so the function will be unique.
	/// </summary>
	/// <returns>The scene string.</returns>
	/// <param name="file">File.</param>
	/// <param name="index">Index.</param>
	public static string CreateSceneString(string file, int index)
	{
		// add scene header
		string function =  "[MenuItem(\"Open Scene/" + file + "\")]";

		// create function with unique name
		return function + "public static void Open" + index.ToString() + "(){SceneMenu.OpenScene(\"" +
		       CreateSceneMenus.assetFolder + file + "\");}\n";
	}

	/// <summary>
	/// Each available scene is added to the navigation menu by creating the file 
	/// defined by filePath and then adding the functions to the file for unity to
	/// parse.
	/// </summary>
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
				// strip beginning Assets/ foldr from string
				paths[i] = paths[i].Replace(CreateSceneMenus.assetFolder, "");

				// add function to file
				completeFile += CreateSceneMenus.CreateSceneString(paths[i], i);
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
