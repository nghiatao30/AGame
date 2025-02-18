using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using HyrphusQ.Helpers;

[CustomEditor(typeof(PPrefIntVariable), true), CanEditMultipleObjects]
public class PPrefIntVariableInspector : OdinEditor
{
    private int setValue;
    private PPrefIntVariable data => target as PPrefIntVariable;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        if (GUILayout.Button("Clear"))
            data.Clear();
        setValue = EditorGUILayout.IntField("Set value", setValue);
        if (GUILayout.Button("Set"))
            data.value = setValue;

        if (string.IsNullOrEmpty(data.key))
            data.InvokeMethod("GenerateSaveKey");

        serializedObject.ApplyModifiedProperties();
    }
}