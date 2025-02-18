using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Helpers;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PPrefVector3Variable), true), CanEditMultipleObjects]
public class PPrefVector3VariableInspector : OdinEditor
{
    private Vector3 setValue;
    private PPrefVector3Variable data => target as PPrefVector3Variable;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        if (GUILayout.Button("Clear"))
            data.Clear();
        setValue = EditorGUILayout.Vector3Field("Set value", setValue);
        if (GUILayout.Button("Set"))
            data.value = setValue;

        if (string.IsNullOrEmpty(data.key))
            data.InvokeMethod("GenerateSaveKey");

        serializedObject.ApplyModifiedProperties();
    }
}