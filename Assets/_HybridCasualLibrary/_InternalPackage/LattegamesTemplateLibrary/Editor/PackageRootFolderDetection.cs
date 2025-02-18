using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LatteGames.EditorUtil{
public class PackageRootFolderDetection
{
    const string dummyObjectRelativeResourcePath = "LatteGames/pathfindingDummy";
    static string DummyObjectResourcePath => "/Resource/" + dummyObjectRelativeResourcePath + ".txt";
    public static string GetPath()
    {
        Object dummyPath = Resources.Load(dummyObjectRelativeResourcePath);
        string dummyFullPath = AssetDatabase.GetAssetPath(dummyPath);
        Resources.UnloadAsset(dummyPath);
        return dummyFullPath.Remove(dummyFullPath.Length - DummyObjectResourcePath.Length , DummyObjectResourcePath.Length);
    }
    public static string GetAssetRelativePath()
    {
        return GetPath().Remove(0, "Assets/".Length);
    }
}
}