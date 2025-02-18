using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using System.Xml;
using UnityEditor;
#endif
using LatteGames.EditorUtil;
namespace LatteGames.Analytics
{
    [CreateAssetMenu(fileName = "KwalleSetting", menuName = "LatteGames/ScriptableObject/Analytics/KwalleSetting", order = 0)]
    public class KwallePresetSetting : PresetSetting
    {
        [SerializeField] private string appName = "app";
        [SerializeField] private string bundleId = "com.macha.";
        [SerializeField] private int versionCode = 1;
        [SerializeField] private string versionString = "0.1";
    
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
                new PackageImportSetup.Package(typeof(GameAnalyticsService), CommonAnalyticsPackage.GA)
            );
            EditorGUILayout.Space();
            AnalyticsObjectInstanceSetup.GUI(
                new AnalyticsObjectInstanceSetup.InstanceObject(
                    typeof(KwalleAnalyticsManager),
                    Path.Combine(PackageRootFolderDetection.GetPath(),"Analytics/Analytics/ServicePresets/Kwalle/KwallePreset.prefab")
                )
            );
            EditorGUILayout.Space();
            PlayerSettingSetup.GUI(BuildTarget.Android, appName, bundleId, versionString, versionCode);
            EditorGUILayout.Space();
            FacebookSetup.GUI();
            EditorGUILayout.Space();
            GameAnalyticsSetup.GUI(versionString);
        }
#endif
    }
}