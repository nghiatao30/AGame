using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

public static class DataManagementEditor
{
	[MenuItem("LatteGames/Data Management/Generate SavedDataManager prefab")]
	private static void GenerateSavedDataManagerPrefab()
    {
		var projWindowUtilType = typeof(ProjectWindowUtil);
		var methodInfo = projWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
		var folderPath = methodInfo.Invoke(null, null).ToString();
		var fullPathName = $"{folderPath}/SavedDataManager.prefab";
		fullPathName = AssetDatabase.GenerateUniqueAssetPath(fullPathName);

		var prefab = new GameObject();
		prefab.AddComponent<SavedDataManager>();
		PrefabUtility.SaveAsPrefabAsset(prefab, fullPathName);
        Object.DestroyImmediate(prefab);
    }

	[MenuItem("LatteGames/Data Management/Clear All Data %#DEL")]
	private static void ClearAllData()
	{
		if (EditorUtility.DisplayDialog("Clear All Data", "Are you sure you wish to clear all data?\nThis action cannot be reversed.", "Clear", "Cancel"))
		{
			DirectoryInfo di = new(Application.persistentDataPath);
			foreach (FileInfo file in di.GetFiles())
				file.Delete();
			foreach (DirectoryInfo dir in di.GetDirectories())
				dir.Delete(true);
			PlayerPrefs.DeleteAll();
		}
	}
}
