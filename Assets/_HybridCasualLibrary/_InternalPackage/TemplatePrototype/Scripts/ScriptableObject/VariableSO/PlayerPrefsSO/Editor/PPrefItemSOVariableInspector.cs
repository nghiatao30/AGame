using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using HyrphusQ.Helpers;

[CustomEditor(typeof(PPrefItemSOVariable), true), CanEditMultipleObjects]
public class PPrefItemSOVariableInspector : OdinEditor
{
    protected ItemSO setValue;
    protected Sprite currentItemThumbnail;
    protected PPrefItemSOVariable data => target as PPrefItemSOVariable;

    protected override void OnEnable()
    {
        base.OnEnable();
        currentItemThumbnail = data.value != null ? (data.value.TryGetModule(out ImageItemModule imageModule) ? imageModule.thumbnailImage : null) : null;
    }

    protected virtual void DrawFirstProperties()
    {
        if (data.value != null)
        {
            GUI.enabled = false;
            SirenixEditorFields.UnityPreviewObjectField(currentItemThumbnail, typeof(Sprite), false, 100f, ObjectFieldAlignment.Left);
            EditorGUILayout.ObjectField(new GUIContent("Current Value"), data.value, typeof(ItemSO), false);
            GUI.enabled = true;
        }
        else
            EditorGUILayout.LabelField($"Current Value: Null");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();

        if (GUILayout.Button("Clear"))
            data.Clear();
        setValue = EditorGUILayout.ObjectField("Set value", setValue, typeof(ItemSO), false) as ItemSO;
        if (GUILayout.Button("Set"))
            data.value = setValue;

        if (string.IsNullOrEmpty(data.key))
            data.InvokeMethod("GenerateSaveKey");

        var isAnyChangesApplied = serializedObject.ApplyModifiedProperties();
        if (isAnyChangesApplied)
        {
            currentItemThumbnail = data.value != null ? (data.value.TryGetModule(out ImageItemModule imageModule) ? imageModule.thumbnailImage : null) : null;
        }
    }
}