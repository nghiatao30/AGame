using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.Analytics
{
        [OptionalDependency("Amplitude", "LatteGames_Gismart")]
    public class GismartAnalyticsService : AnalyticsService
    {
#if LatteGames_Gismart
        public string AndroidLogUrl = "";
        public string AndroidApiKey = "";
        public string iOSLogUrl = "";
        public string iOSApiKey = "";
        public string LogUrl = "";
        public string ApiKey = "";
#endif

#if LatteGames_Gismart
        private void Awake() {
#if UNITY_IOS
            Amplitude.Instance.init(iOSLogUrl, iOSApiKey);
#elif UNITY_ANDROID
            Amplitude.Instance.init(AndroidLogUrl, AndroidApiKey);
#else
            Amplitude.Instance.init(LogUrl, ApiKey); //by default
#endif
            Amplitude.Instance.logging = true;
            Amplitude.Instance.trackSessionEvents(true);
        }
#endif

        public override void SendEventLog(string eventKey, Dictionary<string, object> additionData)
        {
#if LatteGames_Gismart
            Amplitude.Instance.logEvent(eventKey, additionData);
#endif
        }
    }
}