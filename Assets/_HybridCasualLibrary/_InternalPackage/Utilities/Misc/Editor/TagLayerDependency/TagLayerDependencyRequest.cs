using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class TagLayerDependencyRequest
{
    [InitializeOnLoadMethod]
    private static void CheckMissingTagsAndLayers()
    {
        EditorApplication.delayCall += DelayCall;

        void DelayCall()
        {
            if (TagLayerDependencySO.Instance == null)
                return;

            var isAnyMissing = false;
            var tags = TagLayerDependencySO.Instance.tags;
            var layers = TagLayerDependencySO.Instance.layers;

            // Find tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            // Check tags
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            for (int i = 0; i < tags.Count; i++)
            {
                SerializedProperty prop = tagsProp.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(tags[i].tag) || tags[i].tag.Equals(prop.stringValue))
                    continue;
                isAnyMissing = true;
                goto Result;
            }

            // Check layers
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            for (int i = 0; i < layers.Count; i++)
            {
                SerializedProperty prop = layersProp.GetArrayElementAtIndex(i);
                if (layers[i].isBuiltIn || string.IsNullOrEmpty(layers[i].layerName) || layers[i].layerName.Equals(prop.stringValue))
                    continue;
                isAnyMissing = true;
                goto Result;
            }

        Result:
            if (isAnyMissing)
            {
                if (EditorUtility.DisplayDialog("Missing Tags Or Layers", "It looks like you're missing some dependent *Tags or Layers*.\nPlease add it in order to make it work properly!!!", "Add pls!", "No???"))
                {
                    UpdateTagsAndLayers();
                }
            }
            EditorApplication.delayCall -= DelayCall;
        }
    }

    [MenuItem("LatteGames/Update Tags And Layers")]
    private static void UpdateTagsAndLayers()
    {
        var tags = TagLayerDependencySO.Instance.tags;
        var layers = TagLayerDependencySO.Instance.layers;

        // Find tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        // Update tags
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        for (int i = 0; i < tags.Count; i++)
        {
            SerializedProperty prop = tagsProp.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(tags[i].tag) || tags[i].tag.Equals(prop.stringValue))
                continue;
            // Assign string value to tag
            prop.stringValue = tags[i].tag;
            Debug.Log("Tag: " + tags[i].tag + " has been added");
        }

        // Update layers
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        for (int i = 0; i < layers.Count; i++)
        {
            SerializedProperty prop = layersProp.GetArrayElementAtIndex(i);
            if (layers[i].isBuiltIn || string.IsNullOrEmpty(layers[i].layerName) || layers[i].layerName.Equals(prop.stringValue))
                continue;
            // Assign string value to layer
            prop.stringValue = layers[i].layerName;
            Debug.Log("Layer: " + layers[i].layerName + " has been added");
        }

        // Save settings
        var isAnyChanged = tagManager.ApplyModifiedProperties();

        if (!isAnyChanged)
        {
            EditorUtility.DisplayDialog("Catch up", "All tags and layers have already been added Bruh!!!", "OK");
        }
        else
        {
            EditorUtility.SetDirty(tagManager.targetObject);
            AssetDatabase.SaveAssetIfDirty(tagManager.targetObject);
            EditorUtility.DisplayDialog("Succeeded", "Add tags and layers successfully Bruh!!!", "OK");
        }
    }
}