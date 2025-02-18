using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LatteGames.Analytics
{
    public static class AppsflyerSetup
    {
#if LatteGames_AppsFlyer
        public static void GUI(string appsflyerDevKey, string appsflyerAppId)
        {
            CheckConfig(appsflyerDevKey, appsflyerAppId);
        }
    private static void CheckConfig(string appsflyerDevKey, string appId)
    {
        var appsflyerService = GameObject.FindObjectOfType<AppsFlyerObjectScript>();
        var appsflyerLGService = GameObject.FindObjectOfType<AnalyticsManager>()?.GetComponentInChildren<AppsflyerAnalyticsService>()??null;

        if(appsflyerService)
        {
            if(string.IsNullOrEmpty(appsflyerService.devKey) 
            #if UNITY_IOS
            || string.IsNullOrEmpty(appsflyerService.appID)
            #endif
            ){
                EditorGUILayout.HelpBox(GetWarningMessage(),MessageType.Warning);
            }   
            if(GUILayout.Button("Set up Appsflyer"))
            {
                appsflyerService.devKey = appsflyerDevKey;
                appsflyerService.appID = appId;
                EditorUtility.SetDirty(appsflyerService);
            }
        }

        if(appsflyerLGService)
        {
            if(string.IsNullOrEmpty(appsflyerLGService.DevKey) 
            #if UNITY_IOS
            || string.IsNullOrEmpty(appsflyerLGService.AppId)
            #endif
            ){
                EditorGUILayout.HelpBox(GetWarningMessage(),MessageType.Warning);
            }   
            if(GUILayout.Button("Set up Appsflyer"))
            {
                appsflyerLGService.DevKey = appsflyerDevKey;
                appsflyerLGService.AppId = appId;
                EditorUtility.SetDirty(appsflyerLGService);
            }
        }
    }

    private static string GetWarningMessage()
    {
        #if UNITY_IOS
        return "Missing Dev key or app id for appsflyer analytics service.";
        #endif
        #if UNITY_ANDROID
        return "Missing Dev key for appsflyer analytics service.";
        #endif
        return "Missing config for appsflyer analytics service.";
    }
#endif
    }
}