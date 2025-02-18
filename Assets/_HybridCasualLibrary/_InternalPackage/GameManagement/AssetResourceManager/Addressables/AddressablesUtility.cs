using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif

#if UNITY_ADDRESSABLES
public static class AddressablesUtility
{
    public static IList<object> AssetReferences2Keys(params AssetReference[] assetReferences)
    {
        if (assetReferences == null || assetReferences.Length <= 0)
            return null;
        var count = assetReferences.Length;
        var runtimeKeys = new object[count];
        for (int i = 0; i < count; i++)
        {
            runtimeKeys[i] = assetReferences[i].RuntimeKey;
        }
        return runtimeKeys;
    }

    public static IList<object> AssetReferences2Keys(List<AssetReference> assetReferences)
    {
        if (assetReferences == null || assetReferences.Count <= 0)
            return null;
        var count = assetReferences.Count;
        var runtimeKeys = new object[count];
        for (int i = 0; i < count; i++)
        {
            runtimeKeys[i] = assetReferences[i].RuntimeKey;
        }
        return runtimeKeys;
    }

    public static List<string> AssetReferences2AssetGUIDs(params AssetReference[] assetReferences)
    {
        if (assetReferences == null || assetReferences.Length <= 0)
            return null;
        var count = assetReferences.Length;
        var assetGUIDs = new List<string>(new string[count]);
        for (int i = 0; i < count; i++)
        {
            assetGUIDs[i] = assetReferences[i].AssetGUID;
        }
        return assetGUIDs;
    }

    public static string FindAssetKeyBySceneName(string sceneName)
    {
        var sceneNameWithExtension = $"{sceneName}.unity";
        var resourceLocators = Addressables.ResourceLocators;
        foreach (var resourceLocator in resourceLocators)
        {
            var keys = resourceLocator.Keys;
            foreach (object key in keys)
            {
                if (key is string assetKeyId && assetKeyId.Contains(sceneNameWithExtension))
                    return assetKeyId;
            }
        }
        return string.Empty;
    }
}
#endif