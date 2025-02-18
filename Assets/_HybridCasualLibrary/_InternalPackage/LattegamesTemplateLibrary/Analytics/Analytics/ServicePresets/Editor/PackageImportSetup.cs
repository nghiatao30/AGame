using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using static LatteGames.Analytics.PresetSetting;

namespace LatteGames.Analytics
{
    public static class PackageImportSetup
    {
        public class Package
        {
            public Type ServiceClass;
            public CommonAnalyticsPackage.PackageURL packageURL;

            public Package(Type serviceClass, CommonAnalyticsPackage.PackageURL packageURL)
            {
                ServiceClass = serviceClass;
                this.packageURL = packageURL;
            }
        }
        public static void GUI(params Package[] packages)
        {
            bool packagesImported = true;
            foreach (var package in packages)
            {
                if(!package.ServiceClass.IsSubclassOf(typeof(AnalyticsService)))
                    continue;
                if(!CheckIfDependencyAvailable(package.ServiceClass))
                {
                    packagesImported = false;
                    break;
                }
            }
            if(!packagesImported)
                EditorGUILayout.HelpBox("Missing required SDKs", MessageType.Warning);
            foreach (var package in packages)
            {
                if (CheckIfDependencyAvailable(package.ServiceClass))
                    continue;
                GUILayout.Label(package.ServiceClass.Name);
                EditorGUILayout.BeginHorizontal();
                if (!string.IsNullOrEmpty(package.packageURL.DownLoadURL))
                {
                    if(GUILayout.Button("Download"))
                    {
                        Application.OpenURL(package.packageURL.DownLoadURL);
                    }   
                }
                if (!string.IsNullOrEmpty(package.packageURL.DownloadPageURL))
                {
                    if(GUILayout.Button("Open Download Page"))
                    {
                        Application.OpenURL(package.packageURL.DownloadPageURL);
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (string.IsNullOrEmpty(package.packageURL.DownLoadURL) 
                && string.IsNullOrEmpty(package.packageURL.DownloadPageURL))
                {
                    EditorGUILayout.HelpBox("No Download URL found. Contact your manager for the package", MessageType.Error);
                }
                EditorGUILayout.Separator();
            }
        }
        private static bool CheckIfDependencyAvailable(Type type)
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            var group = BuildPipeline.GetBuildTargetGroup(target);
            var attributes = new List<object>(Attribute.GetCustomAttributes(type, typeof (OptionalDependencyAttribute))).ConvertAll(attr => attr as OptionalDependencyAttribute);
            var depFullfill = true;
            foreach (var attr in attributes)
            {
                if(!PresetSetting.Ultility.HasDefineSymbol(group, attr.define))
                {
                    depFullfill = false;
                    break;
                }
            }
            return depFullfill;
        }
    }
}