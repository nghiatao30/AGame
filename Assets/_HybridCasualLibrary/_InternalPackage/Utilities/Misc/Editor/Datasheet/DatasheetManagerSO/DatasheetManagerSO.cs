using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[WindowMenuItem("General", "Import & Export", "Assets")]
[CreateAssetMenu(fileName = "DatasheetManagerSO", menuName = "LatteGames/Editor/Datasheet/DatasheetManagerSO")]
public class DatasheetManagerSO : UnityEditor.SingletonSO<DatasheetManagerSO>
{
    [Serializable]
    public class DatasheetWrapper
    {
        [SerializeField, InlineEditor]
        private Datasheet m_Datasheet;

        [ShowInInspector, PropertyOrder(-1)]
        private string label
        {
            get => m_Datasheet == null ? "Null" : m_Datasheet.name.Replace("Datasheet", "");
        }
        public Datasheet datasheet => m_Datasheet;

        [Button, HorizontalGroup("Action")]
        private void ImportData()
        {
            if ((m_Datasheet.mode & Datasheet.Mode.ImportOnly) == 0)
            {
                Debug.LogError($"{m_Datasheet} is not support ImportData");
                return;
            }
            m_Datasheet.ImportData();
        }
        [Button, HorizontalGroup("Action")]
        private void ExportData()
        {
            if ((m_Datasheet.mode & Datasheet.Mode.ExportOnly) == 0)
            {
                Debug.LogError($"{m_Datasheet} is not support ExportData");
                return;
            }
            m_Datasheet.ExportData(string.Empty);
        }
        [Button(SdfIconType.ArrowUpRightSquare, ""), HorizontalGroup("Action", Width = 20)]
        private void OpenRemoteURL()
        {
            m_Datasheet.OpenRemoteURL();
        }
    }

    [SerializeField, TableList]
    protected List<DatasheetWrapper> m_DatasheetWrappers;
}