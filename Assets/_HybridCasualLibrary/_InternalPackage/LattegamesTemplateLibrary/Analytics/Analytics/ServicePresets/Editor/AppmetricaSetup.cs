using System.Collections;
using System.Collections.Generic;
using System.IO;
using LatteGames.EditorUtil;
using UnityEditor;
using UnityEngine;

namespace LatteGames.Analytics
{
    public static class AppmetricaSetup
    {
        public static void GUI(string appMetricaApikey)
        {
#if LatteGames_AppMetrica
            if(GameObject.FindObjectOfType<AppMetrica>() != null)
            {
                var appMetrica = GameObject.FindObjectOfType<AppMetrica>() as AppMetrica;
                var serializedAppMetrica = new SerializedObject(appMetrica);
                var apiKeyProperty = serializedAppMetrica.FindProperty("ApiKey");
                if(string.IsNullOrEmpty(apiKeyProperty.stringValue))
                    EditorGUILayout.HelpBox("Missing Api key for appmetrica analytics service.",MessageType.Warning);
                if(GUILayout.Button("Set up App metrica"))
                {
                    var target = EditorUserBuildSettings.activeBuildTarget;
                    var group = BuildPipeline.GetBuildTargetGroup(target);
                    PresetSetting.Ultility.AddScriptingDefineSymbolForGroup(group, "APP_METRICA_TRACK_LOCATION_DISABLED");

                    apiKeyProperty.stringValue = appMetricaApikey;

                    serializedAppMetrica.FindProperty("LocationTracking").boolValue = false;
                    serializedAppMetrica.FindProperty("SessionTimeoutSec").intValue = 300;

                    serializedAppMetrica.ApplyModifiedProperties();
                    EditorUtility.SetDirty(appMetrica);

                    //find gradle template and update to add install installreferrer
                    var gradlePath = Path.Combine(Application.dataPath, "Plugins", "Android", "mainTemplate.gradle");
                    var defaultGradleTemplatePath = Path.Combine(PackageRootFolderDetection.GetPath(), "Analytics", "Analytics", "Editor", "mainTemplate.gradle");
                    if(!File.Exists(gradlePath))
                    {
                        if(!Directory.Exists(Path.GetDirectoryName(gradlePath)))
                            Directory.CreateDirectory(Path.GetDirectoryName(gradlePath));
                        File.Copy(defaultGradleTemplatePath, gradlePath);
                    }
                    var gradleContent = File.ReadAllText(gradlePath);
                    if(!gradleContent.Contains("implementation 'com.android.installreferrer:installreferrer:1.1.2'"))
                    {
                        var depsIndex = gradleContent.IndexOf("**DEPS**");
                        gradleContent = gradleContent.Insert(depsIndex, "\timplementation 'com.android.installreferrer:installreferrer:1.1.2'\n");
                        File.WriteAllText(gradlePath,gradleContent);
                    }
                }
            }
#endif
        }
    }
}