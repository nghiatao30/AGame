using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(TextureSize))]
public class TextureSizePropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var optionValueSerializedProp = property.FindPropertyRelative("m_OptionValue");
        if (optionValueSerializedProp.intValue == 0)
            return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;
        return base.GetPropertyHeight(property, label);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var widthSerializedProp = property.FindPropertyRelative("m_Width");
        var heightSerializedProp = property.FindPropertyRelative("m_Height");
        var optionValueSerializedProp = property.FindPropertyRelative("m_OptionValue");
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), optionValueSerializedProp, label);
        if(optionValueSerializedProp.intValue != -1 && optionValueSerializedProp.intValue != 0)
        {
            widthSerializedProp.intValue = optionValueSerializedProp.intValue;
            heightSerializedProp.intValue = optionValueSerializedProp.intValue;
        }
        else if(optionValueSerializedProp.intValue == 0)
        {
            var optionsCustomSizeRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
            var newTextureSize = EditorGUI.Vector2IntField(optionsCustomSizeRect, new GUIContent("Size"), new Vector2Int(widthSerializedProp.intValue, heightSerializedProp.intValue));
            widthSerializedProp.intValue = newTextureSize.x;
            heightSerializedProp.intValue = newTextureSize.y;
        }
        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }
}