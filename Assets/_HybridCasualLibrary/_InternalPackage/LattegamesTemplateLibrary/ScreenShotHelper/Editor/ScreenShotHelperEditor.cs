using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LatteGames.EditorUtil
{
    public class ScreenShotHelperEditor
    {
        [MenuItem("GameObject/LatteGames/PlayStore Screen Shot Helper", false, 0)]
        static void AddHelperObject()
        {
            var screenshotObject = AssetDatabase.LoadAssetAtPath(PackageRootFolderDetection.GetPath() + "ScreenShotHelper/PlayStoreScreenShot.prefab",typeof(GameObject));
            PrefabUtility.InstantiatePrefab(screenshotObject);
        }
    }
}