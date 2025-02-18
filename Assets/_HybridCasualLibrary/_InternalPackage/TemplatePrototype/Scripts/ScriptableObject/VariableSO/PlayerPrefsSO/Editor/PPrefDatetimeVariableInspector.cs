using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using HyrphusQ.Helpers;

[CustomEditor(typeof(PPrefDatetimeVariable), true), CanEditMultipleObjects]
public class PPrefDatetimeVariableInspector : OdinEditor
{
    private string setValue;
    private PPrefDatetimeVariable data => target as PPrefDatetimeVariable;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        if (GUILayout.Button("Clear"))
            data.Clear();
        setValue = EditorGUILayout.TextField("Set value", setValue);
        if (GUILayout.Button("Set"))
        {
            DateTime originDatetime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            originDatetime = originDatetime.AddSeconds(double.Parse(setValue));
            data.value = originDatetime.ToLocalTime();
        }

        if (string.IsNullOrEmpty(data.key))
            data.InvokeMethod("GenerateSaveKey");

        serializedObject.ApplyModifiedProperties();
    }
}