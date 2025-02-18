using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HyrphusQ.Events;

[CustomPropertyDrawer(typeof(EventCode))]
public class EventCodeDrawer : PropertyDrawer
{
    public static class Styles
    {
        public static GUIContent ChangeBtnIcon = EditorGUIUtility.TrTextContentWithIcon("Pick", "d_CollabChangesDeleted Icon");
    }

    private const float k_Offset = 2.5f;

    private void AssignThenApplyModifiedProperties(SerializedProperty property, SerializedProperty eventTypeSerializedProp, SerializedProperty eventCodeSerializedProp, string eventType, string eventCode)
    {
        eventTypeSerializedProp.stringValue = eventType;
        eventCodeSerializedProp.stringValue = eventCode;
        property.serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(property.serializedObject.targetObject);
    }

    private void CreateMenuEventCodeOptions(SerializedProperty property)
    {
        var eventTypeSerializedProp = property.FindPropertyRelative("m_EventType");
        var eventCodeSerializedProp = property.FindPropertyRelative("m_EventCode");

        GenericMenu menu = new GenericMenu();
        // Replace C# Reflection with TypeCache for Editor performance wise
        Type[] eventCodeEnumTypes = TypeCache.GetTypesWithAttribute<EventCodeAttribute>().Where(type => type.IsEnum).OrderBy(type => type.Name).ToArray();
        menu.AddItem(new GUIContent("None"), false, userData =>
        {
            var tuple = (Tuple<string, string>)userData;
            AssignThenApplyModifiedProperties(
                property,
                eventTypeSerializedProp, eventCodeSerializedProp,
                tuple.Item1, tuple.Item2);
        }, Tuple.Create(string.Empty, string.Empty));
        foreach (var eventCodeType in eventCodeEnumTypes)
        {
            foreach (var eventCode in Enum.GetValues(eventCodeType))
            {
                menu.AddItem(new GUIContent($"{eventCodeType.Name}/{eventCode}"), false, userData =>
                {
                    var tuple = (Tuple<string, string>)userData;
                    AssignThenApplyModifiedProperties(
                        property,
                        eventTypeSerializedProp, eventCodeSerializedProp,
                        tuple.Item1, tuple.Item2);
                }, Tuple.Create(eventCodeType.AssemblyQualifiedName, Enum.GetName(eventCodeType, eventCode)));
            }
        }
        menu.ShowAsContext();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var eventTypeSerializedProp = property.FindPropertyRelative("m_EventType");
        var eventCodeSerializedProp = property.FindPropertyRelative("m_EventCode");

        var buttonRect = Rect.zero;

        if (!string.IsNullOrEmpty(eventCodeSerializedProp.stringValue) && !string.IsNullOrEmpty(eventTypeSerializedProp.stringValue))
        {
            var eventEnumRect = new Rect(position.x, position.y, position.width * 0.8f, position.height);
            buttonRect = new Rect(position.x + eventEnumRect.width + k_Offset, position.y, position.width * 0.2f - k_Offset, position.height);

            EditorGUI.BeginChangeCheck();
            var enumType = Type.GetType(eventTypeSerializedProp.stringValue);
            if (enumType == null || !Enum.TryParse(enumType, eventCodeSerializedProp.stringValue, out var eventCodeAsInt))
            {
                if (enumType == null)
                    Debug.LogError($"EventType '{eventTypeSerializedProp.stringValue}' is not found anymore!!!");
                else
                    Debug.LogError($"EventCode '{eventCodeSerializedProp.stringValue}' of type '{enumType.Name}' is not found anymore!!!");
                AssignThenApplyModifiedProperties(
                    property,
                    eventTypeSerializedProp, eventCodeSerializedProp,
                    string.Empty, string.Empty);
                return;
            }
            var displayedOptions = Enum.GetNames(enumType);
            var optionValues = Enum.GetValues(enumType).Cast<int>().ToArray();
            var eventCodeObject = Enum.ToObject(enumType, EditorGUI.IntPopup(eventEnumRect, label.text, (int)eventCodeAsInt, displayedOptions, optionValues));
            if (EditorGUI.EndChangeCheck())
            {
                AssignThenApplyModifiedProperties(
                    property,
                    eventTypeSerializedProp, eventCodeSerializedProp,
                    eventTypeSerializedProp.stringValue, Enum.GetName(enumType, eventCodeObject));
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
            CreateMenuEventCodeOptions(property);
        }
    }
}