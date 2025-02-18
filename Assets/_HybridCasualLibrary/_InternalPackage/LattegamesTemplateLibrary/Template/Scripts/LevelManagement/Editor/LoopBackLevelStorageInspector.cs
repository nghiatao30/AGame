using System.Collections;
using System.Collections.Generic;
using LatteGames;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LoopBackLevelStorage), true), CanEditMultipleObjects]
public class LoopBackLevelStorageInspector : OdinEditor
{
    private int currentPage = 0;
    private int itemsPerPage = 10; // Number of items per page
    private int levelIndex
    {
        get
        {
            if ((target as LoopBackLevelStorage).PlayerAchievedContinuousLevel != null) return (target as LoopBackLevelStorage).PlayerAchievedContinuousLevel.value + 1;
            else return 0;
        }
        set
        {
            if ((target as LoopBackLevelStorage).PlayerAchievedContinuousLevel == null) return;
            (target as LoopBackLevelStorage).PlayerAchievedContinuousLevel.value = value;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the levelAssets list with Odin's ListDrawerSettings
        var levelAssetsProperty = serializedObject.FindProperty("levelAssets");

        SirenixEditorGUI.BeginBox();
        SirenixEditorGUI.Title("Levels", null, TextAlignment.Left, true);

        DrawList(levelAssetsProperty);

        SirenixEditorGUI.EndBox();

        // Draw the default inspector excluding the levelAssets field
        var excludedProperties = new string[] { "levelAssets", "m_Script" };
        DrawPropertiesExcluding(serializedObject, excludedProperties);

        int totalItem = levelAssetsProperty.arraySize;
        if (totalItem > 0 && levelIndex < totalItem)
        {
            string levelName = "";
            if (levelAssetsProperty.GetArrayElementAtIndex(levelIndex).objectReferenceValue != null)
            {
                levelName = levelAssetsProperty.GetArrayElementAtIndex(levelIndex).objectReferenceValue.name;
            }
            EditorGUILayout.LabelField("Current level index: " + levelIndex + " (" + levelName + ")");
        }

        // Apply changes to the serializedObject
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawList(SerializedProperty levelAssetsProperty)
    {
        // Create a foldout for the list
        EditorGUILayout.PropertyField(levelAssetsProperty, true);

        // Calculate total pages
        int totalItems = levelAssetsProperty.arraySize;
        int totalPages = Mathf.CeilToInt((float)totalItems / itemsPerPage);

        // Draw the items for the current page
        int startIndex = currentPage * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, totalItems);

        if (!levelAssetsProperty.isExpanded)
        {
            // Iterate through each element in the serialized property
            for (int i = startIndex; i < endIndex; i++)
            {
                SerializedProperty element = levelAssetsProperty.GetArrayElementAtIndex(i);

                SirenixEditorGUI.BeginHorizontalToolbar();
                EditorGUILayout.PropertyField(element, GUIContent.none);

                if (GUILayout.Button("Set index to this level", GUILayout.Width(150)))
                {
                    levelIndex = i - 1;
                }

                SirenixEditorGUI.EndHorizontalToolbar();
            }

            // Draw navigation buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous Page") && currentPage > 0)
            {
                currentPage--;
            }

            EditorGUILayout.LabelField($"Page {currentPage + 1} of {totalPages}", EditorStyles.centeredGreyMiniLabel);

            if (GUILayout.Button("Next Page") && currentPage < totalPages - 1)
            {
                currentPage++;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
