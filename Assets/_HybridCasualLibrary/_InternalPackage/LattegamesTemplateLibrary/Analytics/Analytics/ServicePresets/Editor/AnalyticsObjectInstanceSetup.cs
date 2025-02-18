using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using LatteGames.EditorUtil;

namespace LatteGames.Analytics
{
    public static class AnalyticsObjectInstanceSetup
    {
        public class InstanceObject
        {
            public Type classType;
            public string prefabPath;

            public InstanceObject(Type classType, string prefabPath)
            {
                this.classType = classType;
                this.prefabPath = prefabPath;
            }
        }

        public static void GUI(params InstanceObject[] instances)
        {
            bool instanceCreated = true;
            foreach (var instance in instances)
            {
                if(GameObject.FindObjectOfType(instance.classType) == null)
                {
                    instanceCreated = false;
                    break;
                }
            }
            if(!instanceCreated)
                EditorGUILayout.HelpBox("Missing analytics manager object, click create bellow", MessageType.Warning);
            if(GUILayout.Button("Create Analytics Preset object"))
            {
                var analyticsManagers = GameObject.FindObjectsOfType<AnalyticsManager>();
                foreach (var manager in analyticsManagers)
                    GameObject.DestroyImmediate(manager.gameObject);
                
                foreach (var instance in instances)
                {
                    if(GameObject.FindObjectOfType(instance.classType))
                        continue;
                    var prefab = AssetDatabase.LoadMainAssetAtPath(instance.prefabPath) as GameObject;
                    var gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    gameObject.transform.SetAsFirstSibling();
                }
            }
        }
    }
}