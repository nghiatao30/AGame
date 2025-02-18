using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;
using LatteGames.EditorUtil;
namespace LatteGames.Analytics
{
    [CreateAssetMenu(fileName = "AzurSetting", menuName = "LatteGames/ScriptableObject/Analytics/AzurSetting", order = 0)]
    public class AzurPresetSetting : PresetSetting
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
                new PackageImportSetup.Package(typeof(FacebookAnalyticsService),CommonAnalyticsPackage.Facebook),
                new PackageImportSetup.Package(typeof(AppsflyerAnalyticsService),CommonAnalyticsPackage.AppsFlyer),
                new PackageImportSetup.Package(typeof(AppMetricaAnalyticsService),CommonAnalyticsPackage.AppMetrica)
            );
            EditorGUILayout.Space();
            
#if LatteGames_AppMetrica
            AnalyticsObjectInstanceSetup.GUI(
                new AnalyticsObjectInstanceSetup.InstanceObject(
                    typeof(AppMetrica),
                    "Assets/AppMetrica/AppMetrica.prefab"
                ),
                new AnalyticsObjectInstanceSetup.InstanceObject(
                    typeof(AzurAnalyticsManager),
                    Path.Combine(PackageRootFolderDetection.GetPath(),"Analytics/Analytics/ServicePresets/Azur/AzurPreset.prefab")
                )
            );
#endif
            EditorGUILayout.Space();
            PlayerSettingSetup.GUI(
                BuildTarget.NoTarget,
                appName,
                bundleId,
                versionString,
                versionCode,
                minAndroidVersion : AndroidSdkVersions.AndroidApiLevel22,
                optionalPlayerSetting: ()=>{
                    PlayerSettings.SetUseDefaultGraphicsAPIs(
                        BuildTarget.Android,
                        false
                    );
                    PlayerSettings.SetGraphicsAPIs(
                        BuildTarget.Android, 
                        new UnityEngine.Rendering.GraphicsDeviceType[] {
                                UnityEngine.Rendering.GraphicsDeviceType.Vulkan, 
                                UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3
                            });
                }
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
        }
#endif
    }
}