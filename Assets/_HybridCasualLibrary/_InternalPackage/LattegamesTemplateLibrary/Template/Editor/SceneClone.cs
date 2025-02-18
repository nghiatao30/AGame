using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using LatteGames.EditorUtil;
public class SceneClone
{
    [MenuItem("Assets/Create/LatteGames/Scenes/New Empty Scene")]
    public static void CloneNewEmpty()
    {
        CloneAsset("Template/Scenes/empty.unity", "main.unity");   
    }
    [MenuItem("Assets/Create/LatteGames/Scenes/New State Based Game")]
    public static void CloneStateBasedGame()
    {
        CloneAsset("Template/Scenes/StateBasedGame.unity", "main.unity");   
    }

    private static void CloneAsset(string assetPath, string newName)
    {
        File.Copy(
            Path.Combine(Application.dataPath, PackageRootFolderDetection.GetAssetRelativePath(), assetPath),
            Path.Combine(Application.dataPath, GetSelectedPathOrFallback().Remove(0, "Assets/".Length), newName));
        AssetDatabase.Refresh();
    }

    public static string GetSelectedPathOrFallback()
	{
		string path = "Assets";
		
		foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
		{
			path = AssetDatabase.GetAssetPath(obj);
			if ( !string.IsNullOrEmpty(path) && File.Exists(path) ) 
			{
				path = Path.GetDirectoryName(path);
				break;
			}
		}
		return path;
	}
}
