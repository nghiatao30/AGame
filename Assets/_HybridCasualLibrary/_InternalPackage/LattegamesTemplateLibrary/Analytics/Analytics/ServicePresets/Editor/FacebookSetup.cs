using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using LatteGames.EditorUtil;
using UnityEditor;
using UnityEngine;

namespace LatteGames.Analytics
{
    public static class FacebookSetup
    {
        public static void GUI()
        {
#if LatteGames_FB
            if(string.IsNullOrEmpty(Facebook.Unity.Settings.FacebookSettings.AppId))
                EditorGUILayout.HelpBox("Please Specify Facebook App Name & Facebook App Id", MessageType.Error);
            //check android manifest
            var androidManifestPath = Path.Combine(Application.dataPath, "Plugins", "Android", "AndroidManifest.xml");
            var gradlePropertyPath = Path.Combine(Application.dataPath, "Plugins", "Android", "gradleTemplate.properties");
            if(!File.Exists(gradlePropertyPath))
                EditorGUILayout.HelpBox("gradleTemplate.properties not found. The build might fail due to duplicated libraries\nClick Facebook SDK setup bellow to fix it.", MessageType.Warning);
            if(!File.Exists(androidManifestPath))
                EditorGUILayout.HelpBox("Android Manifest has not been created!", MessageType.Error);
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(androidManifestPath);
                var e = doc.GetElementsByTagName("application");
                if(e.Count >0)
                {
                    var applicationE = e.Item(0).Attributes.GetNamedItem("android:debuggable");
                    if(applicationE != null)
                    {
                        if(applicationE.InnerText != "false")
                        {
                            applicationE.InnerText = "false";
                            doc.Save(androidManifestPath);
                        }
                    }
                }
            }
            if(!Facebook.Unity.Settings.FacebookSettings.IsValidAppId)
                EditorGUILayout.HelpBox("Missing FB App Id", MessageType.Warning);

            if(GUILayout.Button("Facebook SDK setup"))
            {
                Selection.activeObject = Facebook.Unity.Settings.FacebookSettings.Instance;
                //find gradle property template and fix FB old android libraries
                if(!File.Exists(gradlePropertyPath))
                {
                    if(!Directory.Exists(Path.GetDirectoryName(gradlePropertyPath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(gradlePropertyPath));
                    var defaultGradleTemplatePropertiesPath = Path.Combine(PackageRootFolderDetection.GetPath(), "Analytics", "Analytics", "Editor", "gradleTemplate.properties");
                    File.Copy(defaultGradleTemplatePropertiesPath, gradlePropertyPath);
                }
            }
            if(File.Exists(gradlePropertyPath))
            {
                var properties = File.ReadAllText(gradlePropertyPath);
                if(!properties.Contains("android.useAndroidX=true"))
                    properties = properties + "\nandroid.useAndroidX=true";
                if(!properties.Contains("android.enableJetifier=true"))
                    properties = properties + "\nandroid.enableJetifier=true";
                File.WriteAllText(gradlePropertyPath, properties);

            }
            if(File.Exists(androidManifestPath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(androidManifestPath);
                AppendPermissionIfNotExist(doc, "android.permission.INTERNET");
                AppendPermissionIfNotExist(doc, "android.permission.ACCESS_NETWORK_STATE");
                AppendPermissionIfNotExist(doc, "android.permission.ACCESS_WIFI_STATE");
                doc.Save(androidManifestPath);
            }
#endif
        }

        private static void AppendPermissionIfNotExist(XmlDocument document, string permission)
        {
            var manifestE = document.GetElementsByTagName("manifest");
            var e = document.GetElementsByTagName("uses-permission");
            List<XmlNode> eNodes = new List<XmlNode>();
            for (int i = 0; i < e.Count; i++)
                eNodes.Add(e.Item(i));
            if(eNodes.FindIndex(item => item.Attributes.GetNamedItem("android:name")?.InnerText == permission) != -1)
                return;
            XmlElement elem = document.CreateElement("uses-permission");
            var attr = document.CreateAttribute("android", "name", "http://schemas.android.com/apk/res/android");
            attr.InnerText = permission;
            elem.SetAttributeNode(attr);
            manifestE.Item(0).AppendChild(elem);
        }
    }
}