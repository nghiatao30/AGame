using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HyrphusQ.Helpers;

[CustomPropertyDrawer(typeof(SerializedEnum), true)]
public class SerializedEnumDrawer : PropertyDrawer
{
    public static class Styles
    {
        public static GUIContent ChangeBtnIcon = EditorGUIUtility.TrTextContentWithIcon("Pick", "d_CollabChangesDeleted Icon");
    }

    private const float k_Offset = 2.5f;

    private Type GetSerializedGenericEnumType(Type enumSelector)
    {
        var baseType = enumSelector;
        do
        {
            if (baseType == null)
                return null;
            if (baseType.IsGenericType && baseType.BaseType == typeof(SerializedEnum))
                return baseType;
            baseType = baseType.BaseType;
        }
        while (true);
    }

    private void AssignThenApplyModifiedProperties(SerializedProperty property, SerializedProperty eventTypeSerializedProp, SerializedProperty eventCodeSerializedProp, string eventType, string eventCode)
    {
        eventTypeSerializedProp.stringValue = eventType;
        eventCodeSerializedProp.stringValue = eventCode;
        property.serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(property.serializedObject.targetObject);
    }

    private void CreateMenuEnumOptions(SerializedProperty property)
    {
        var enumTypeSerializedProp = property.FindPropertyRelative("m_EnumType");
        var enumNameSerializedProp = property.FindPropertyRelative("m_EnumName");

        GenericMenu menu = new GenericMenu();
        // Replace C# Reflection with TypeCache for Editor performance wise
        var enumSelector = property.serializedObject.targetObject.GetFieldValue<object>(property.propertyPath);
        Type attributeType = GetSerializedGenericEnumType(enumSelector.GetType()).GetGenericArguments()[0];
        Type[] enumTypes = TypeCache.GetTypesWithAttribute(attributeType).Where(type => type.IsEnum).OrderBy(type => type.Name).ToArray();
        menu.AddItem(new GUIContent("None"), false, userData =>
        {
            var tuple = (Tuple<string, string>)userData;
            AssignThenApplyModifiedProperties(
                property,
                enumTypeSerializedProp, enumNameSerializedProp,
                tuple.Item1, tuple.Item2);
        }, Tuple.Create(string.Empty, string.Empty));
        foreach (var enumType in enumTypes)
        {
            foreach (var enumValue in Enum.GetValues(enumType))
            {
                menu.AddItem(new GUIContent($"{enumType.Name}/{enumValue}"), false, userData =>
                {
                    var tuple = (Tuple<string, string>)userData;
                    AssignThenApplyModifiedProperties(
                        property,
                        enumTypeSerializedProp, enumNameSerializedProp,
                        tuple.Item1, tuple.Item2);
                }, Tuple.Create(enumType.AssemblyQualifiedName, Enum.GetName(enumType, enumValue)));
            }
        }
        menu.ShowAsContext();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var enumTypeSerializedProp = property.FindPropertyRelative("m_EnumType");
        var enumNameSerializedProp = property.FindPropertyRelative("m_EnumName");

        var buttonRect = Rect.zero;

        if (!string.IsNullOrEmpty(enumNameSerializedProp.stringValue) && !string.IsNullOrEmpty(enumTypeSerializedProp.stringValue))
        {
            var eventEnumRect = new Rect(position.x, position.y, position.width * 0.8f, position.height);
            buttonRect = new Rect(position.x + eventEnumRect.width + k_Offset, position.y, position.width * 0.2f - k_Offset, position.height);

            EditorGUI.BeginChangeCheck();
            var enumType = Type.GetType(enumTypeSerializedProp.stringValue);
            if (enumType == null || !Enum.TryParse(enumType, enumNameSerializedProp.stringValue, out var enumObjectAsInt))
            {
                if (enumType == null)
                    Debug.LogError($"Type '{enumTypeSerializedProp.stringValue}' is not found anymore!!!");
                else
                    Debug.LogError($"Enum value '{enumNameSerializedProp.stringValue}' of type '{enumType.Name}' is not found anymore!!!");
                AssignThenApplyModifiedProperties(
                    property,
                    enumTypeSerializedProp, enumNameSerializedProp,
                    string.Empty, string.Empty);
                return;
            }
            var displayedOptions = Enum.GetNames(enumType);
            var optionValues = Enum.GetValues(enumType).Cast<int>().ToArray();
            var enumObject = Enum.ToObject(enumType, EditorGUI.IntPopup(eventEnumRect, label.text, (int)enumObjectAsInt, displayedOptions, optionValues));
            if (EditorGUI.EndChangeCheck())
            {
                AssignThenApplyModifiedProperties(
                    property,
                    enumTypeSerializedProp, enumNameSerializedProp,
                    enumTypeSerializedProp.stringValue, Enum.GetName(enumType, enumObject));
            }
        }
        else
        {
            buttonRect = new Rect(position.x + position.width * 0.4f + 7.5f, position.y, position.width * 0.6f - 7.5f, position.height);
            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            GUI.Label(labelRect, label);
        }

        if (GUI.Button(buttonRect, Styles.ChangeBtnIcon))
        {
            CreateMenuEnumOptions(property);
        }
    }
}