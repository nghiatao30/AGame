using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HyrphusQ.Helpers;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StringSelectorAttribute))]
public class StringSelectorDrawer : PropertyDrawer
{
    private List<string> GetOptions()
    {
        var stringSelectorAttribute = attribute as StringSelectorAttribute;
        var type = stringSelectorAttribute.classType;
        var comparisonMethod = type.GetMethod(stringSelectorAttribute.optionsGetterMethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (comparisonMethod == null)
        {
            Debug.LogWarning("Can not find method with name: " + stringSelectorAttribute.optionsGetterMethodName);
            return null;
        }
        return (List<string>) comparisonMethod.Invoke(null, null);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var options = GetOptions();
        if (options == null || options.Count <= 0)
        {
            EditorGUI.PropertyField(position, property, label, property.isExpanded);
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            var optionIndex = EditorGUI.Popup(position, label.text, options.IndexOf(property.stringValue), options.ToArray());
            if (optionIndex.IsValidRange(0, options.Count - 1))
                property.stringValue = options[optionIndex];

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }
    }
}