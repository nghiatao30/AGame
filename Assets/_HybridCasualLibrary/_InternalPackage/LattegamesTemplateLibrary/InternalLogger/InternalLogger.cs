using System;
using UnityEngine;

namespace LatteGames
{
    [Serializable]
    public class InternalLogger
    {
        [SerializeField] private bool enableLogger = false;
        [SerializeField] private string tag = "default";
        public bool IsEnabled => enableLogger;

        private bool IsLogEnabled()
        {
            return enableLogger && InternalLoggerSetting.Instance.EnabledTags.Contains(tag);
        }

        public void Log(string msg)
        {
            if (!IsLogEnabled())
                return;
            Debug.Log(msg);
        }

        public void LogWarning(string msg)
        {
            if(!IsLogEnabled())
                return;
            Debug.LogWarning(msg);
        }

        public void LogError(string msg)
        {
            if(!IsLogEnabled())
                return;
            Debug.LogError(msg);
        }
    }
}