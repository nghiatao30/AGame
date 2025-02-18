using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MultiImageItemModule))]
public class MultiImageItemModulePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var rect = new Rect(position.position, new Vector2(position.width, EditorGUIUtility.singleLineHeight));
        if(EditorGUI.PropertyField(rect, property, false))
        {
            EditorGUI.indentLevel++;
            var serializedProp = property.FindPropertyRelative("m_ThumbnailImageBlocks");
            rect.position += Vector2.up * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            EditorGUI.PropertyField(rect, serializedProp, serializedProp.isExpanded);
            EditorGUI.indentLevel++;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return base.GetPropertyHeight(property, label);
        var serializedProp = property.FindPropertyRelative("m_ThumbnailImageBlocks");
        return EditorGUI.GetPropertyHeight(serializedProp, serializedProp.isExpanded) + base.GetPropertyHeight(property, label) + EditorGUIUtility.standardVerticalSpacing;
    }
}