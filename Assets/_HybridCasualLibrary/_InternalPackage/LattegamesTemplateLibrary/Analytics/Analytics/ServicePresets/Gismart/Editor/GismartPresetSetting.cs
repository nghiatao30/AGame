using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using LatteGames.EditorUtil;

namespace LatteGames.Analytics
{
    [CreateAssetMenu(fileName = "GismartSetting", menuName = "LatteGames/ScriptableObject/Analytics/GismartSetting", order = 0)]
    public class GismartPresetSetting : PresetSetting
    {
        [SerializeField] private string appName = "app";
        [SerializeField] private string bundleId = "com.macha.";
        [SerializeField] private int versionCode = 1;
        [SerializeField] private string versionString = "0.1";

        #if LatteGames_AppsFlyer
        [SerializeField] private string appsflyerDevKey = "";
        [SerializeField] private string appsflyerAppId = "";
        #endif
        #if LatteGames_AppMetrica
        [SerializeField] private string appMetricaApikey = "";
        #endif
        #if LatteGames_Gismart
        [SerializeField] private string gismartAndroidLogUrl = "";
        [SerializeField] private string gismartAndroidApiKey = "";
        [SerializeField] private string gismartiOSLogUrl = "";
        [SerializeField] private string gismartiOSApiKey = "";
        #endif

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
                new PackageImportSetup.Package(
                    typeof(FacebookAnalyticsService), CommonAnalyticsPackage.Facebook
                ),
                new PackageImportSetup.Package(
                    typeof(AppMetricaAnalyticsService), CommonAnalyticsPackage.AppMetrica
                ),
                new PackageImportSetup.Package(
                    typeof(AppsflyerAnalyticsService), CommonAnalyticsPackage.AppsFlyer
                ),
                new PackageImportSetup.Package(
                    typeof(GismartAnalyticsService), CommonAnalyticsPackage.Gismart
                )
            );
#if LatteGames_AppMetrica
            AnalyticsObjectInstanceSetup.GUI(
                new AnalyticsObjectInstanceSetup.InstanceObject(
                    typeof(AppMetrica),
                    "Assets/AppMetrica/AppMetrica.prefab"
                ),
                new AnalyticsObjectInstanceSetup.InstanceObject(
                    typeof(GismartAnalyticsManager),
                    Path.Combine(PackageRootFolderDetection.GetPath(),"Analytics/Analytics/ServicePresets/Gismart/GismartPreset.prefab")
                )
            );
#endif
            EditorGUILayout.Space();
            PlayerSettingSetup.GUI(
                BuildTarget.NoTarget,
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
#if LatteGames_AppMetrica
            AppmetricaSetup.GUI(appMetricaApikey);
#endif
            EditorGUILayout.Space();
            //special setup
#if LatteGames_Gismart
            var gismartService = GameObject.FindObjectOfType<GismartAnalyticsService>();
            if(gismartService == null)
                EditorGUILayout.HelpBox("Missing Gismart Analytics service", MessageType.Warning);
            else
            {
                var gAndLogUrlMissing = string.IsNullOrEmpty(gismartService.AndroidLogUrl);
                var gAndApiMissing = string.IsNullOrEmpty(gismartService.AndroidApiKey);

                var giOSLogUrlMissing = string.IsNullOrEmpty(gismartService.iOSLogUrl);
                var giOSApiMissing = string.IsNullOrEmpty(gismartService.iOSApiKey);

                #if UNITY_ANDROID
                if(gAndLogUrlMissing)
                    EditorGUILayout.HelpBox("Gismart missing Log Url", MessageType.Warning);
                if(gAndApiMissing)
                    EditorGUILayout.HelpBox("Gismart missing Api key", MessageType.Warning);
                #endif

                #if UNITY_IOS
                if(giOSLogUrlMissing)
                    EditorGUILayout.HelpBox("Gismart missing Log Url", MessageType.Warning);
                if(giOSApiMissing)
                    EditorGUILayout.HelpBox("Gismart missing Api key", MessageType.Warning);
                #endif
            }

            if(GUILayout.Button("Setup Gismart"))
            {
                var gradlePath = CreateMainGradle();
                AddImplementation(gradlePath, "implementation 'com.squareup.okhttp3:okhttp:4.4.0'");
                AddImplementation(gradlePath, "implementation 'com.squareup.okio:okio:2.2.2'");
                if(gismartService != null)
                {
                    gismartService.AndroidLogUrl = gismartAndroidLogUrl;
                    gismartService.AndroidApiKey = gismartAndroidApiKey;
                    
                    gismartService.iOSLogUrl = gismartiOSLogUrl;
                    gismartService.iOSApiKey = gismartiOSApiKey;

                    EditorUtility.SetDirty(gismartService);
                }

                //change appmetrica script
                var appmetricaScript = File.ReadAllText(Path.Combine(Application.dataPath,"AppMetrica/AppMetrica.cs"));
                appmetricaScript = appmetricaScript.Replace(
                    "configuration.LocationTracking = false;", 
                    "configuration.LocationTracking = LocationTracking;");
                File.WriteAllText(Path.Combine(Application.dataPath,"AppMetrica/AppMetrica.cs"), appmetricaScript);
            }
#endif
        }

        private string CreateMainGradle()
        {
            var gradlePath = Path.Combine(Application.dataPath, "Plugins", "Android", "mainTemplate.gradle");
            var defaultGradleTemplatePath = Path.Combine(PackageRootFolderDetection.GetPath(), "Analytics", "Analytics", "Editor", "mainTemplate.gradle");
            if(!File.Exists(gradlePath))
            {
                if(!Directory.Exists(Path.GetDirectoryName(gradlePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(gradlePath));
                File.Copy(defaultGradleTemplatePath, gradlePath);
            }
            return gradlePath;
        }

        private void AddImplementation(string gradlePath, string implementationString)
        {
            var gradleContent = File.ReadAllText(gradlePath);
            if(!gradleContent.Contains(implementationString))
            {
                var depsIndex = gradleContent.IndexOf("**DEPS**");
                gradleContent = gradleContent.Insert(depsIndex, $"\t{implementationString}\n");
                File.WriteAllText(gradlePath,gradleContent);
            }
        }
    }
}