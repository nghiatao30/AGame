using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LatteGames.Analytics
{
    public static class FlurrySetup
    {
        public static void GUI(string flurryAndroidApiKey, string flurryiOSApiKey, string flurryName)
        {
#if LatteGames_Flurry
            
            if(GameObject.FindObjectOfType<ZplayAnalyticsManager>() != null)
            {
                var flurryService = GameObject.FindObjectOfType<ZplayAnalyticsManager>().GetComponentInChildren<FlurryAnalyticsService>();
                if(string.IsNullOrEmpty(flurryService.FLURRY_API_KEY) || string.IsNullOrEmpty(flurryService.flurryName))
                {
                    EditorGUILayout.HelpBox("Missing Api key for flurry analytics service.",MessageType.Warning);
                }
                if(GUILayout.Button("Set up Flurry"))
                {
                    flurryService.AndroidApiKey = flurryAndroidApiKey;
                    flurryService.iOSApiKey = flurryiOSApiKey;
                    flurryService.flurryName = flurryName;
                    EditorUtility.SetDirty(flurryService);
                }
            }
#endif
        }
    }
}