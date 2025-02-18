using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDatabaseSavedDataSO : SavedDataSO<PlayerDatabaseSavedDataSO.SaveData>
{
    [Serializable]
    public class SaveData : SavedData
    {
        public List<AIBotInfo.SaveData> m_BotInfosSaveData = new List<AIBotInfo.SaveData>();
    }

    [SerializeField]
    private PlayerDatabaseSO m_PlayerDatabaseSO;

    public override SaveData defaultData
    {
        get
        {
            var saveData = new SaveData();
            foreach (var botInfo in m_PlayerDatabaseSO.botInfos)
            {
                saveData.m_BotInfosSaveData.Add(botInfo.GetSaveData());
            }
            return saveData;
        }
    }
}