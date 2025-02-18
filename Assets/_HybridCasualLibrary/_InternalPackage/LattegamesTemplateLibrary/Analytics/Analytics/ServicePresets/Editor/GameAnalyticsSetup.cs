using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LatteGames.Analytics
{
    public static class GameAnalyticsSetup
    {
        public static void GUI(string versionString){
#if LatteGames_GA
            var emptyGameKey = false;
            var emptyGameSecret = false;
            for (int i = 0; i < GameAnalyticsSDK.GameAnalytics.SettingsGA.Platforms.Count; i++)
            {
                if(string.IsNullOrEmpty(GameAnalyticsSDK.GameAnalytics.SettingsGA.GetGameKey(i)))
                {
                    emptyGameKey = true;
                    break;
                }
            }
            for (int i = 0; i < GameAnalyticsSDK.GameAnalytics.SettingsGA.Platforms.Count; i++)
            {
                if(string.IsNullOrEmpty(GameAnalyticsSDK.GameAnalytics.SettingsGA.GetSecretKey(i)))
                {
                    emptyGameSecret = true;
                    break;
                }
            }
            if(emptyGameKey || emptyGameSecret || GameAnalyticsSDK.GameAnalytics.SettingsGA.Platforms.Count == 0)
            {
                EditorGUILayout.HelpBox("Please specify game Key and game secret for Game Analytics", MessageType.Warning);
            }
            if(GameObject.FindObjectOfType<GameAnalyticsSDK.GameAnalytics>() == null)
            {
                EditorGUILayout.HelpBox("Please create GameAnalytics GameObject via \"Window/GameAnalytics/Create GameAnalytics Object\"", MessageType.Warning);
            }
            if(GUILayout.Button("Game Analytics SDK setup"))
            {
                Selection.activeObject = GameAnalyticsSDK.GameAnalytics.SettingsGA;
                for (int i = 0; i < GameAnalyticsSDK.GameAnalytics.SettingsGA.Platforms.Count; i++)
                {
                    GameAnalyticsSDK.GameAnalytics.SettingsGA.Build[i] = versionString;
                }
            }
#endif
        }
    }
}