using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
#if UNITY_ADDRESSABLES
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

public class BuildAddressablesPreprocessor
{
#if UNITY_ADDRESSABLES
    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandlerCallback);
    }

    private static void BuildPlayerHandlerCallback(BuildPlayerOptions options)
    {
        if (EditorUtility.DisplayDialog("Build with Addressables",
            "Do you want to build a clean addressables before export?",
            "Build with Addressables", "Skip"))
        {
            try
            {
                BuildAddressablesContent();
            }
            catch (Exception exc)
            {
                Debug.LogException(exc);
                return;
            }
        }
        BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
    }

    private static void BuildAddressablesContent()
    {
        Debug.Log("BuildAddressablesProcessor.PreExport start");
        AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
        AddressableAssetSettings.BuildPlayerContent();
        Debug.Log("BuildAddressablesProcessor.PreExport done");
    }
#endif
}