using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using LatteGames.EditorUtil;
#endif

namespace LatteGames.Analytics
{
    [CreateAssetMenu(fileName = "BoomHitSetting", menuName = "LatteGames/ScriptableObject/Analytics/BoomHitSetting", order = 0)]
    public class BoomHitPresetSetting : PresetSetting
    {
        [SerializeField] private string appName = "app";
        [SerializeField] private string bundleId = "com.macha.";
        [SerializeField] private int versionCode = 1;
        [SerializeField] private string versionString = "0.1";

        #if LatteGames_AppsFlyer
        [SerializeField] private string appsflyerDevKey = "Ugf2FWT7Hc5L4TQHpm2SyT";
        [SerializeField] private string appsflyerAppId = "";
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
                new PackageImportSetup.Package(typeof(AppsflyerAnalyticsService),CommonAnalyticsPackage.AppsFlyer)
            );
            EditorGUILayout.Separator();

#if LatteGames_AppsFlyer
            AnalyticsObjectInstanceSetup.GUI(
                new AnalyticsObjectInstanceSetup.InstanceObject(
                    typeof(BoomHitAnalyticsManager),
                    Path.Combine(PackageRootFolderDetection.GetPath(),"Analytics/Analytics/ServicePresets/BoomHit/BoomHitPreset.prefab")
                ),
                new AnalyticsObjectInstanceSetup.InstanceObject(
                    typeof(AppsFlyerObjectScript),
                    "Assets/AppsFlyer/AppsFlyerObject.prefab"
                )
            );
#endif
            EditorGUILayout.Separator();
            
            PlayerSettingSetup.GUI(
                BuildTarget.NoTarget,
                appName,
                bundleId,
                versionString,
                versionCode
            );
            EditorGUILayout.Separator();

#if LatteGames_FB
            FacebookSetup.GUI();           
            EditorGUILayout.Separator();
#endif

#if LatteGames_AppsFlyer
            AppsflyerSetup.GUI(appsflyerDevKey, appsflyerAppId);
#endif
        }

        #endif
    }
}