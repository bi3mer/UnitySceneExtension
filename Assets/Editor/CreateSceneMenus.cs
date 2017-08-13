using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.IO;

[InitializeOnLoad]
public class CreateSceneMenus : EditorWindow
{
	private static string fileName = "SceneMenu.cs";
	private static string filePath = "Assets/Editor/" + fileName;
	private static string openingString = "using UnityEngine;using System.Collections;using UnityEditor;using UnityEditor.SceneManagement;\n"
		+ "public class SceneMenu : Editor {\n";
	
	private static string endingString = "public static void OpenScene(string scene){ " 
		                               + "if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()){"
		                               + "EditorSceneManager.OpenScene(scene);}}}\n";
	private static string assetFolder = "Assets/";
	private static string gitignore = ".gitignore";

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

	/// <summary>
	/// Adds default file created for menu to git ignore.
	/// </summary>
	public static void AddToGitIgnore(string path)
	{
		using (StreamWriter sw = File.AppendText(path)) 
        {
            sw.WriteLine("\n# Ignore scene menu\n" + fileName + "\n" + fileName + ".meta");
        }
	}

	/// <summary>
	/// Checks the .gitignore and if the default file is not added to the 
	/// .gitignore, it will be added.
	/// </summary>
	public static void CheckAndUpdateGitIgnore()
	{
		// initialize variable to read file
		string line;

		// create path to gitignore
		string path = Path.Combine(Directory.GetCurrentDirectory(), gitignore);

		// check if the file exists
		if(File.Exists(path) == true)
		{
			// open file and read line by line
			StreamReader file = new System.IO.StreamReader(path);
			while((line = file.ReadLine()) != null)
			{	
				// if the scene is in the file then we should ignore it
				if(line == fileName)
				{
					return;
				}
			}

			// close the file
			file.Close();

			// if here then the .gitignore isn't updated and the file should be added
			CreateSceneMenus.AddToGitIgnore(path);
		}
	}

	/// <summary>
	/// Called when a scene is being saved. This is useful for when this scene
	/// does not already exist.
	/// </summary>
	/// <param name="scene">Scene.</param>
	public static void NewSceneCreated(UnityEngine.SceneManagement.Scene scene)
	{
		CreateSceneMenus.CreateScenes();
	}

	/// <summary>
    /// Initializes the <see cref="CreateSceneMenus"/> class and immediatley 
    /// sets up menu. It will overwrite on start to ensure files are not 
    /// missed.
    /// </summary>
    static CreateSceneMenus()
    {
    	// Create scenes
    	CreateSceneMenus.CheckAndUpdateGitIgnore();
    	CreateSceneMenus.CreateScenes();

    	// subscribe to event when new scene is created
//		EditorSceneManager.NewSceneCreatedCallback += NewSceneCreated;
		EditorSceneManager.sceneSaved += CreateSceneMenus.NewSceneCreated;
    }

    /// <summary>
    /// Releases unmanaged resources and performs other cleanup operations 
    /// before the <see cref="CreateSceneMenus"/> is reclaimed by garbage 
    /// collection.
    /// </summary>
    ~CreateSceneMenus()
    {
		EditorSceneManager.sceneSaved -= CreateSceneMenus.NewSceneCreated;
    }
}
