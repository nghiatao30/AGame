using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if LatteGames_Flurry
using FlurrySDK;
#endif

namespace LatteGames.Analytics
{
        [OptionalDependency("FlurrySDK.Flurry", "LatteGames_Flurry")]
    public class FlurryAnalyticsService : AnalyticsService
    {
#if LatteGames_Flurry
        public string AndroidApiKey = "";
        public string iOSApiKey = "";
        public string ApiKey = "";
        public string flurryName = "";
        public bool withCrashReporting = true;
        public bool withLogEnabled = true;
        public Flurry.LogLevel logLevel = Flurry.LogLevel.VERBOSE;
        public bool withMessaging = true;

        #if UNITY_ANDROID
        public string FLURRY_API_KEY => AndroidApiKey;
        #elif UNITY_IPHONE
        public string FLURRY_API_KEY => iOSApiKey;
        #else
        public string FLURRY_API_KEY => ApiKey;
        #endif

        void Start()
        {
                // Initialize Flurry.
                #if UNITY_ANDROID
                Flurry.SetVersionName(flurryName);
                #endif
                new Flurry.Builder()
                        .WithCrashReporting(withCrashReporting)
                        .WithLogEnabled(withLogEnabled)
                        .WithLogLevel(logLevel)
                        .WithMessaging(withMessaging)
                        #if UNITY_IOS
                        .WithAppVersion(flurryName)
                        #endif
                        .Build(FLURRY_API_KEY);
        }
#endif

        public override void SendEventLog(string eventKey, Dictionary<string, object> additionData)
        {
#if LatteGames_Flurry
                logger.Log($"Flurry Event log: {eventKey}");
                var additionalDataString = new Dictionary<string,string>();
                foreach (var keyPair in additionData)
                        additionalDataString.Add(keyPair.Key, keyPair.Value.ToString());
                Flurry.LogEvent(eventKey, additionalDataString);
#endif
        }
    }   
}