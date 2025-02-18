using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace LatteGames.Analytics
{
    public class PlayerSettingSetup
    {
        public static void GUI(
            BuildTarget expectedTarget, 
            string appName, 
            string bundleId, 
            string versionString, 
            int versionCode, 
            AndroidSdkVersions minAndroidVersion = AndroidSdkVersions.AndroidApiLevel22, 
            AndroidSdkVersions targetAndroidVersion = AndroidSdkVersions.AndroidApiLevelAuto,
            Action optionalPlayerSetting = null)
        {
            if(EditorUserBuildSettings.activeBuildTarget != expectedTarget && expectedTarget != BuildTarget.NoTarget)
               EditorGUILayout.HelpBox($"this preset expects to build a {expectedTarget} app. The current platform is {EditorUserBuildSettings.activeBuildTarget}. Make sure this is not a mistake", MessageType.Warning);
            if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                if(PlayerSettings.Android.useCustomKeystore == false || string.IsNullOrEmpty(PlayerSettings.Android.keystoreName))
                    EditorGUILayout.HelpBox("Open PlayerSetting and create a custom keystore with the Keystore manager", MessageType.Warning);
            }
            if(GUILayout.Button("Setup player setting"))
            {
                PlayerSettings.companyName = "Macha Fun";
                PlayerSettings.productName = appName;
                PlayerSettings.bundleVersion = versionString;
                var target = EditorUserBuildSettings.activeBuildTarget;
                var group = BuildPipeline.GetBuildTargetGroup(target);
                PlayerSettings.SetApplicationIdentifier(group,bundleId);
                PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.IL2CPP);
                PlayerSettings.SetApiCompatibilityLevel(group, ApiCompatibilityLevel.NET_4_6);
                if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;
                    PlayerSettings.Android.bundleVersionCode = versionCode;
                    PlayerSettings.Android.minSdkVersion = minAndroidVersion;
                    PlayerSettings.Android.targetSdkVersion = targetAndroidVersion;
                    PlayerSettings.Android.useCustomKeystore = true;
                }
                else
                {
                    PlayerSettings.iOS.buildNumber = versionCode.ToString();
                }
                optionalPlayerSetting?.Invoke();
            }
        }
    }
}