using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.Analytics
{
    public abstract class AnalyticsService : MonoBehaviour
    {
        [Header("Enable this will create a debug log in the console (for debuging purpose only)")]
        [SerializeField] 
        protected InternalLogger logger = new InternalLogger();
        [Header("Enable this will log analytics event to the corresponding service")]
        public bool EnableAnalyticsLogging = true;
        public abstract void SendEventLog(string eventKey, Dictionary<string, object> additionData);
    }
}