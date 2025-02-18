using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using HyrphusQ.Events;
using HyrphusQ.Helpers;

[CustomEditor(typeof(GameEventListeners))]
public class GameEventListenersEditor : Editor
{
    private GenericMenu menu;
    private Type[] eventCodeEnumTypes;
    private SerializedProperty listEventsSerializedProp;

    private GameEventListeners data
    {
        get
        {
            return target as GameEventListeners;
        }
    }

    private void OnEnable()
    {
        // Replace C# Reflection with TypeCache for Editor performance wise
        eventCodeEnumTypes = TypeCache.GetTypesWithAttribute<EventCodeAttribute>().Where(type => type.IsEnum).ToArray();
        listEventsSerializedProp = serializedObject.FindProperty("m_EventsList");
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DoLayoutListEvent();
    }

    private void CreateMenuEventCodeOptions()
    {
        menu = new GenericMenu();
        foreach (var eventCodeType in eventCodeEnumTypes)
        {
            foreach (var eventCode in Enum.GetValues(eventCodeType))
            {
                if (data.ContainsEvent(eventCode.ToString()))
                    continue;
                menu.AddItem(new GUIContent($"{eventCodeType.Name}/{eventCode}"), false, OnAddEvent, Tuple.Create(eventCodeType.AssemblyQualifiedName, Enum.GetName(eventCodeType, eventCode)));
            }
        }
        menu.ShowAsContext();
    }
    private void OnAddEvent(object userData)
    {
        var tuple = (Tuple<string, string>) userData;
        data.m_EventsList.Add(new GameEventListeners.EventTuple(new EventCode(tuple.Item2, tuple.Item1)));
        EditorUtility.SetDirty(target);
    }
    private void DoLayoutListEvent()
    {
        // Update data
        serializedObject.Update();

        GUIStyle toolbarEditorStyle = new GUIStyle(EditorStyles.toolbar);
        toolbarEditorStyle.fontSize += (int) (toolbarEditorStyle.fontSize * 0.25f);
        toolbarEditorStyle.alignment = TextAnchor.MiddleCenter;

        // Layout header label
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("EventCode", toolbarEditorStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f));
        EditorGUILayout.LabelField("EventAction", toolbarEditorStyle);
        EditorGUILayout.EndHorizontal();

        // Layout space
        EditorGUILayout.Space();

        // Layout element
        for (int i = 0; i < data.m_EventsList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUI.BeginChangeCheck();
            var enumType = Type.GetType(data.m_EventsList[i].m_EventCode.GetFieldValue<string>("m_EventType"));
            var displayedOptions = Enum.GetNames(enumType).ToList().FindAll(item => item == data.m_EventsList[i].m_EventCode.GetFieldValue<string>("m_EventCode") || !data.m_EventsList.Exists(element => element.m_EventCode.GetFieldValue<string>("m_EventCode") == item)).ToArray();
            var optionValues = Enum.GetValues(enumType).Cast<int>().Where(item => Enum.ToObject(enumType, item).Equals(data.m_EventsList[i].m_EventCode.eventCode) || !data.ContainsEvent(Enum.ToObject(enumType, item).ToString())).ToArray();
            var eventCodeObject = Enum.ToObject(enumType, EditorGUILayout.IntPopup((int) Enum.Parse(enumType, data.m_EventsList[i].m_EventCode.GetFieldValue<string>("m_EventCode")), displayedOptions, optionValues, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f)));
            data.m_EventsList[i].m_EventCode.SetFieldValue("m_EventCode", Enum.GetName(enumType, eventCodeObject));
            data.m_EventsList[i].m_EventCode.SetFieldValue("m_EventType", enumType.AssemblyQualifiedName);
            EditorGUILayout.PropertyField(listEventsSerializedProp.GetArrayElementAtIndex(i).FindPropertyRelative("m_UnityEvent"));
            if (GUILayout.Button(EditorGUIUtility.TrTextContentWithIcon(string.Empty, "TreeEditor.Trash"), GUILayout.Width(28f), GUILayout.Height(28f)))
                data.m_EventsList.RemoveAt(i);

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(target);

            EditorGUILayout.EndHorizontal();
        }

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(EditorGUIUtility.TrTextContent("Add EventAction")))
                CreateMenuEventCodeOptions();
            GUILayout.Space(18f);
        }

        // Save data
        serializedObject.ApplyModifiedProperties();
    }
}
