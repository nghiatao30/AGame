using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

[CustomPropertyDrawer(typeof(Requirement))]
public class RequirementDrawer : PropertyDrawer
{
    private const float k_Offset = 12.5f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();
        var rect_1 = new Rect(position.position + Vector2.right * k_Offset, new Vector2(position.width - k_Offset, EditorGUIUtility.singleLineHeight));
        var rect_2 = new Rect(position.position + Vector2.up * (rect_1.height + EditorGUIUtility.standardVerticalSpacing), new Vector2(position.width, EditorGUI.GetPropertyHeight(property, property.isExpanded)));
        property.managedReferenceValue = SirenixEditorFields.PolymorphicObjectField(rect_1, new GUIContent("Requirements"), property.managedReferenceValue, typeof(Requirement), false);
        if (!(property.managedReferenceValue == null || property.managedReferenceValue is Requirement_Empty))
            EditorGUI.PropertyField(rect_2, property, label, property.isExpanded);
        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null || property.managedReferenceValue is Requirement_Empty)
            return EditorGUIUtility.singleLineHeight;
        return EditorGUI.GetPropertyHeight(property, property.isExpanded) + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }
}