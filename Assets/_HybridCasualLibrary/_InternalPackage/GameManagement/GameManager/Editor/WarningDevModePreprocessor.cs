using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using HyrphusQ.Helpers;

public class WarningDevModePreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        var gameDataSO = EditorUtils.FindAssetOfType<GameDataSO>();
        if (gameDataSO.isDevMode)
        {
            if (!EditorUtility.DisplayDialog("Warning Developer mode",
"Developer mode is enabled. Are you sure about that?",
"Definitely sure", "Disable it"))
            {
                var isEnableLogOnBuild = gameDataSO.isEnableLogOnBuild;
                gameDataSO.SetFieldValue("m_IsDevMode", false);
                gameDataSO.InvokeMethod("OnValidateIsEnableLogOnBuild");
                EditorUtility.SetDirty(gameDataSO);
                AssetDatabase.SaveAssetIfDirty(gameDataSO);
                if (isEnableLogOnBuild)
                    throw new BuildFailedException("Failed to build because of recompiling script (remove custom define symbols). Please build again!!!");
            }
        }
    }
}