using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using HyrphusQ.Helpers;

[CustomEditor(typeof(PPrefBoolVariable), true), CanEditMultipleObjects]
public class PPrefBoolVariableInspector : OdinEditor
{
    private bool setValue;
    private PPrefBoolVariable data => target as PPrefBoolVariable;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        if (GUILayout.Button("Clear"))
            data.Clear();
        setValue = EditorGUILayout.Toggle("Set value", setValue);
        if (GUILayout.Button("Set"))
            data.value = setValue;

        if (string.IsNullOrEmpty(data.key))
            data.InvokeMethod("GenerateSaveKey");

        serializedObject.ApplyModifiedProperties();
    }
}