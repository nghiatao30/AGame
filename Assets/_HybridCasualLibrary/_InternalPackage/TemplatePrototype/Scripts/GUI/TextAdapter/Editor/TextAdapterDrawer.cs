using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HyrphusQ.GUI;

[CustomPropertyDrawer(typeof(TextAdapter))]
public class TextAdapterDrawer : PropertyDrawer
{
    private const float k_Offset = 20f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var textTypeRect = new Rect(position.x, position.y, position.width / 2f - k_Offset, position.height);
        var textRect = new Rect(position.x + position.width / 2f - k_Offset, position.y, position.width / 2f + k_Offset, position.height);

        var textTypeProperty = property.FindPropertyRelative("m_TextType");
        var textBuiltInProperty = property.FindPropertyRelative("m_TextBuiltIn");
        var textMeshProProperty = property.FindPropertyRelative("m_TextMeshPro");
        EditorGUI.PropertyField(textTypeRect, textTypeProperty, GUIContent.none);
        switch ((TextAdapter.TextType)textTypeProperty.intValue)
        {
            case TextAdapter.TextType.None:
                break;
            case TextAdapter.TextType.BuildIn:
                EditorGUI.PropertyField(textRect, textBuiltInProperty, GUIContent.none);
                break;
            case TextAdapter.TextType.TextMeshPro:
                EditorGUI.PropertyField(textRect, textMeshProProperty, GUIContent.none);
                break;
            default:
                break;
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}