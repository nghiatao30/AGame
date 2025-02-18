using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using HyrphusQ.Helpers;

[CustomEditor(typeof(PPrefStringVariable), true), CanEditMultipleObjects]
public class PPrefStringVariableInspector : OdinEditor
{
    private string setValue;
    private PPrefStringVariable data => target as PPrefStringVariable;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        if (GUILayout.Button("Clear"))
            data.Clear();
        setValue = EditorGUILayout.TextField("Set value", setValue);
        if (GUILayout.Button("Set"))
            data.value = setValue;

        if (string.IsNullOrEmpty(data.key))
            data.InvokeMethod("GenerateSaveKey");

        serializedObject.ApplyModifiedProperties();
    }
}