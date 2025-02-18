using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedDataManager : MonoBehaviour
{
    [SerializeField]
    private List<SavedDataSO> m_SavedDataSOs;

    private void Awake()
    {
        LoadAllData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveAllData();
    }

    private void OnDestroy()
    {
        SaveAllData();
    }


    public void LoadAllData()
    {
        for (int i = 0; i < m_SavedDataSOs.Count; i++)
        {
            var savedDataSO = m_SavedDataSOs[i];
            savedDataSO.Load();
        }
    }

    public void SaveAllData()
    {
        for (int i = 0; i < m_SavedDataSOs.Count; i++)
        {
            var savedDataSO = m_SavedDataSOs[i];
            savedDataSO.Save();
        }
    }
}