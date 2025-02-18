using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

public abstract class Datasheet : SerializedScriptableObject
{
    public enum Mode
    {
        ImportOnly = 1 << 0,
        ExportOnly = 1 << 1,
        ImportAndExport = 1 << 0 | 1 << 1,
    }
    [Serializable]
    public class SheetLocation
    {
        [SerializeField]
        private string m_RemotePath;
        [SerializeField]
        private string m_LocalPath;

        public string remotePath => m_RemotePath;
        public string localPath => m_LocalPath;
    }

    [SerializeField]
    protected Mode m_Mode = Mode.ImportOnly;
    [SerializeField]
    protected SheetLocation m_SheetLocation;

    public virtual Mode mode => m_Mode;
    public virtual string localPath => sheetLocation.localPath;
    public virtual string remotePath => sheetLocation.remotePath;
    public virtual SheetLocation sheetLocation => m_SheetLocation;

    protected virtual string RemoveAfterExport(string input)
    {
        int exportIndex = input.IndexOf("/export");

        if (exportIndex >= 0)
        {
            return input.Substring(0, exportIndex);
        }

        return input;
    }

    public abstract void ImportData();
    public abstract void ExportData(string directoryPath);
    public virtual void OpenRemoteURL()
    {
        Application.OpenURL(RemoveAfterExport(m_SheetLocation.remotePath));
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Datasheet), true)]
public class DatasheetInspector : OdinEditor
{
    protected Datasheet datasheet => target as Datasheet;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if ((datasheet.mode & Datasheet.Mode.ImportOnly) != 0)
        {
            if (GUILayout.Button("Import Data"))
            {
                datasheet.ImportData();
            }
        }
        if ((datasheet.mode & Datasheet.Mode.ExportOnly) != 0)
        {
            if (GUILayout.Button("Export Data"))
            {
                var directoryPath = EditorUtility.SaveFolderPanel("Save as", "", "");
                if (string.IsNullOrEmpty(directoryPath))
                    return;
                datasheet.ExportData(directoryPath);
            }
        }
    }
}
#endif