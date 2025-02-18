using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using System.Xml;
using UnityEditor;
#endif

#if LatteGames_Adjust
using com.adjust.sdk;
#endif
using LatteGames.EditorUtil;
namespace LatteGames.Analytics
{
    [CreateAssetMenu(fileName = "MooneeSetting", menuName = "LatteGames/ScriptableObject/Analytics/MooneeSetting", order = 0)]
    public class MooneePresetSetting : PresetSetting
    {
        [SerializeField] private string appName = "app";
        [SerializeField] private string bundleId = "com.macha.";
        [SerializeField] private int versionCode = 1;
        [SerializeField] private string versionString = "0.1";
    #if LatteGames_Adjust
        [SerializeField] private string appToken = "";
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
                new PackageImportSetup.Package(typeof(GameAnalyticsService), CommonAnalyticsPackage.GA),
                new PackageImportSetup.Package(typeof(AdjustAnalyticsService), CommonAnalyticsPackage.Adjust),
                new PackageImportSetup.Package(typeof(FacebookAnalyticsService), CommonAnalyticsPackage.Facebook)
            );
            EditorGUILayout.Space();
#if LatteGames_Adjust
            AnalyticsObjectInstanceSetup.GUI(
                new AnalyticsObjectInstanceSetup.InstanceObject(
                    typeof(MooneeAnalyticsManager),
                    Path.Combine(PackageRootFolderDetection.GetPath(),"Analytics/Analytics/ServicePresets/Moonee/MooneePreset.prefab")
                ),
                new AnalyticsObjectInstanceSetup.InstanceObject(
                    typeof(Adjust),
                    "Assets/Adjust/Prefab/Adjust.prefab"
                )
            );
#endif
            EditorGUILayout.Space();
            PlayerSettingSetup.GUI(BuildTarget.Android, appName, bundleId, versionString, versionCode);
            EditorGUILayout.Space();
#if LatteGames_Adjust
            AdjustSetup.GUI(appToken);
            EditorGUILayout.Space();
#endif
            FacebookSetup.GUI();
            EditorGUILayout.Space();
            GameAnalyticsSetup.GUI(versionString);
        }
#endif
    }
}