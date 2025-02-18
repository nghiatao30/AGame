#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using System.Xml;
using LatteGames.EditorUtil;
#endif
using UnityEngine;

namespace LatteGames.Analytics
{
    [CreateAssetMenu(fileName = "CrazyLabSetting", menuName = "LatteGames/ScriptableObject/Analytics/CrazyLab")]
    public class CrazyLabPresetSetting : PresetSetting
    {
        [SerializeField] private string appName = "app";
        [SerializeField] private string bundleId = "app.name.game";
        [SerializeField] private int versionCode = 1;
        [SerializeField] private string versionString = "0.1";

#if UNITY_EDITOR

        public override void DrawCustomInspector()
        {
            base.DrawCustomInspector();
            EditorGUILayout.HelpBox("After Import the SDK, please reject Analytics for External Dependency Manager and disable Registry Addition", MessageType.Info);
            PackageImportSetup.GUI(
                new PackageImportSetup.Package(typeof(CLIKAnalyticsService), CommonAnalyticsPackage.CLIKAnalytics)
            );
            EditorGUILayout.Separator();
            PlayerSettingSetup.GUI(
                BuildTarget.NoTarget,
                appName,
                bundleId,
                versionString,
                versionCode
            );
            EditorGUILayout.Separator();
            AnalyticsObjectInstanceSetup.GUI(
                new AnalyticsObjectInstanceSetup.InstanceObject(
                    typeof(CrazyLabAnalyticsManager),
                    Path.Combine(PackageRootFolderDetection.GetPath(),"Analytics/Analytics/ServicePresets/CrazyLab/CrazyLabAnalytics.prefab")
                )
            );
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("Please make sure to config CLIK with the config file downloaded from the dashboard (CLIK/Configure). Make sure you have a green checkmark next to AppsFlyer.Then update the PlayerSetting by click the bellow button", MessageType.Info);
            if (PlayerSettings.Android.minifyDebug
                || PlayerSettings.Android.minifyRelease
                || PlayerSettings.Android.minifyWithR8
            ){
                EditorGUILayout.HelpBox("Android Minify is enabled!!! Please turn this off", MessageType.Error);
            }
            
            if (GUILayout.Button("CrazyLab PlayerSetting Update"))
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
                PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel30;
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

                PlayerSettings.Android.minifyDebug = false;
                PlayerSettings.Android.minifyRelease = false;
                PlayerSettings.Android.minifyWithR8 = false;

                PlayerSettings.stripEngineCode = false;

                PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Disabled);
                PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.iOS, ManagedStrippingLevel.Disabled);
            }
            //check android manifest
            XmlDocument androidManifestDoc = new XmlDocument();
            var androidManifestPath = Path.Combine(Application.dataPath, "Plugins", "Android", "AndroidManifest.xml");
            androidManifestDoc.Load(androidManifestPath);
            var applicationTags = androidManifestDoc.GetElementsByTagName("activity");
            if(applicationTags.Count > 0)
            {
                var tabtaleActivityName = "com.tabtale.ttplugins.ttpunity.TTPUnityMainActivity";
                var currentMainActivityName = applicationTags[0].Attributes["android:name"].Value;
                if (currentMainActivityName != tabtaleActivityName)
                {
                    EditorGUILayout.HelpBox($"[Plugins/Android/AndroidManifest.xml] The main activity in Android manifest should be {tabtaleActivityName}.\n current value is: {currentMainActivityName}", MessageType.Error);
                    if (GUILayout.Button("Fix me"))
                    {
                        applicationTags[0].Attributes["android:name"].Value = tabtaleActivityName;
                        androidManifestDoc.Save(androidManifestPath);
                    }
                }
            }
        }
#endif
    }
}