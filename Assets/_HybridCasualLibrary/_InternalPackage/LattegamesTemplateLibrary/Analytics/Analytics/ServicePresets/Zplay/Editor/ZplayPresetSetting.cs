using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using System.IO;
using System.Xml;
using UnityEditor;
#endif
using LatteGames.EditorUtil;
namespace LatteGames.Analytics
{
    [CreateAssetMenu(fileName = "ZplaySetting", menuName = "LatteGames/ScriptableObject/Analytics/ZplaySetting", order = 0)]
    public class ZplayPresetSetting : PresetSetting
    {
        [SerializeField] private string appName = "app";
        [SerializeField] private string bundleId = "com.macha.";
        [SerializeField] private int versionCode = 1;
        [SerializeField] private string versionString = "0.1";
#if LatteGames_AppsFlyer
        [SerializeField] private string appsflyerDevKey = "";
        #if UNITY_ANDROID
        [HideInInspector]
        #endif
        [SerializeField] private string appsflyerAppId = "";
#endif
#if LatteGames_Flurry
        [SerializeField] private string flurryAndroidApiKey = "";
        [SerializeField] private string flurryiOSApiKey = "";
        [SerializeField] private string flurryName = "";
#endif
    
#if UNITY_EDITOR
        private void OnEnable() {
            AssetDatabase.importPackageCompleted += OnPackageImported;
        }

        private void OnDisable() {
            AssetDatabase.importPackageCompleted -= OnPackageImported;
        }

        private void OnPackageImported(string packageName)
        {
            if(Directory.Exists(Path.Combine(Application.dataPath, "PlayServicesResolver")) 
                && Directory.Exists(Path.Combine(Application.dataPath, "ExternalDependencyManager")))
            {
                Directory.Delete(Path.Combine(Application.dataPath, "PlayServicesResolver"),true);
            }
        }

        public override void DrawCustomInspector()
        {
            base.DrawCustomInspector();
            PackageImportSetup.GUI(
                new PackageImportSetup.Package(typeof(FacebookAnalyticsService), CommonAnalyticsPackage.Facebook),
                new PackageImportSetup.Package(typeof(AppsflyerAnalyticsService), CommonAnalyticsPackage.AppsFlyer),
                new PackageImportSetup.Package(typeof(FlurryAnalyticsService), CommonAnalyticsPackage.Flurry)
            );
            EditorGUILayout.Space();
            AnalyticsObjectInstanceSetup.GUI(
                new AnalyticsObjectInstanceSetup.InstanceObject(
                    typeof(ZplayAnalyticsManager),
                    Path.Combine(PackageRootFolderDetection.GetPath(),"Analytics/Analytics/ServicePresets/Zplay/ZplayPreset.prefab")
                )
            );
            EditorGUILayout.Space();
            PlayerSettingSetup.GUI(
                BuildTarget.Android,
                appName,
                bundleId,
                versionString,
                versionCode
            );
            EditorGUILayout.Space();
            FacebookSetup.GUI();           
            EditorGUILayout.Space();
#if LatteGames_AppsFlyer
            AppsflyerSetup.GUI(appsflyerDevKey, appsflyerAppId);
#endif
            EditorGUILayout.Space();
#if LatteGames_Flurry
            FlurrySetup.GUI(flurryAndroidApiKey, flurryiOSApiKey, flurryName);
#endif
        }


#endif
    }
}