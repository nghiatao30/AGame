using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LatteGames.EditorUtil;
using System;

namespace LatteGames.Analytics
{
    public abstract class PresetSetting : ScriptableObject {
        public static class CommonAnalyticsPackage{
            public struct PackageURL
            {
                public readonly string DownLoadURL;
                public readonly string DownloadPageURL;
                public PackageURL(string downloadURL, string downloadPageURL)
                {
                    DownLoadURL = downloadURL;
                    DownloadPageURL = downloadPageURL;
                }
            }
            public static PackageURL Facebook => 
            new PackageURL(
                "https://lookaside.facebook.com/developers/resources/?id=FacebookSDK-current.zip",
                "https://developers.facebook.com/docs/unity/downloads/" 
            );
            public static PackageURL GA => 
            new PackageURL(
                "https://download.gameanalytics.com/unity/GA_SDK_UNITY.unitypackage",
                "https://github.com/GameAnalytics/GA-SDK-UNITY"
            );
            public static PackageURL AppsFlyer => 
            new PackageURL(
                "",
                "https://github.com/AppsFlyerSDK/appsflyer-unity-plugin"
            );
            public static PackageURL Flurry => 
            new PackageURL(
                "",
                "https://github.com/flurry/unity-flurry-sdk"
            );
            public static PackageURL AppMetrica => 
            new PackageURL(
                "https://github.com/yandexmobile/metrica-plugin-unity/raw/master/AppMetrica.unitypackage",
                "https://github.com/yandexmobile/metrica-plugin-unity");
            public static PackageURL Gismart => 
            new PackageURL(
                "",
                "https://drive.google.com/file/d/1B1772jpdAVyhXNs0ExFfgWGf_BFndfMr/view"
            );
            public static PackageURL CLIKAnalytics => 
            new PackageURL(
                "https://s3.amazonaws.com/com.tabtale.repo/android/maven/ttplugins/com/tabtale/tt_plugins/unity/CLIK/Latest%20CLIK%20Package/Latest%20CLIK.unitypackage",
                ""
            );
            public static PackageURL Adjust =>
            new PackageURL(
                "",
                "https://github.com/adjust/unity_sdk/releases"
            );
        }
        public virtual void DrawCustomInspector(){}
        public static class Ultility{
            public static void AddScriptingDefineSymbolForGroup(UnityEditor.BuildTargetGroup group, string newSymbol)
            {
                var currentDefine = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';'));
                currentDefine.RemoveAll(symbol => string.IsNullOrEmpty(symbol) || string.IsNullOrWhiteSpace(symbol));
                var currentDefineSimplified = new HashSet<string>();
                currentDefine.ForEach(symbol => currentDefineSimplified.Add(symbol));
                currentDefineSimplified.Add(newSymbol);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";",new List<string>(currentDefineSimplified).ToArray()));
            }

            public static bool HasDefineSymbol(UnityEditor.BuildTargetGroup group, string define)
            {
                var currentDefine = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';'));
                currentDefine.RemoveAll(symbol => string.IsNullOrEmpty(symbol) || string.IsNullOrWhiteSpace(symbol));
                return currentDefine.Contains(define);
            }
        }
    }
}