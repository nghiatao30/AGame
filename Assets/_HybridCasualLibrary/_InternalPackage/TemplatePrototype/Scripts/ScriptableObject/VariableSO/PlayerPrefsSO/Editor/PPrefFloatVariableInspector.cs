using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using HyrphusQ.Helpers;

[CustomEditor(typeof(PPrefFloatVariable), true), CanEditMultipleObjects]
public class PPrefFloatVariableInspector : OdinEditor
{
    private float setValue;
    private PPrefFloatVariable data => target as PPrefFloatVariable;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        if (GUILayout.Button("Clear"))
            data.Clear();
        setValue = EditorGUILayout.FloatField("Set value", setValue);
        if (GUILayout.Button("Set"))
            data.value = setValue;

        if (string.IsNullOrEmpty(data.key))
            data.InvokeMethod("GenerateSaveKey");

        serializedObject.ApplyModifiedProperties();
    }
}