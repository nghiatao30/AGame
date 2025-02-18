using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if LatteGames_AppsFlyer
using AppsFlyerSDK;
#endif


namespace LatteGames.Analytics{
    [OptionalDependency("AppsFlyerSDK.AppsFlyer", "LatteGames_AppsFlyer")]
#if LatteGames_AppsFlyer
    public class AppsflyerAnalyticsService : AnalyticsService, IAppsFlyerConversionData, LevelAchievedEvent.ILogger
#else
    public class AppsflyerAnalyticsService : AnalyticsService
#endif
    {
#if LatteGames_AppsFlyer
        public const bool ServiceIsAvailable = true;
        public string DevKey;
        public string AppId;
#else
        public const bool ServiceIsAvailable = false;
#endif
#if LatteGames_AppsFlyer
        void Start()
        {
            AppsFlyer.setIsDebug(logger.IsEnabled);
            AppsFlyer.initSDK(DevKey, AppId, this);
            AppsFlyer.startSDK();
        }

        public void onConversionDataSuccess(string conversionData)
        {
            AppsFlyer.AFLog("onConversionDataSuccess", conversionData);
            Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
            // add deferred deeplink logic here
        }

        public void onConversionDataFail(string error)
        {
            AppsFlyer.AFLog("onConversionDataFail", error);
        }

        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
            Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
            // add direct deeplink logic here
        }

        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
        }
#endif
        public override void SendEventLog(string eventKey, Dictionary<string, object> additionData)
        {
#if LatteGames_AppsFlyer
            logger.Log($"Appsflyer Custom event {eventKey}");
            var additionalDataString = new Dictionary<string,string>();
            foreach (var keyPair in additionData)
                additionalDataString.Add(keyPair.Key, keyPair.Value.ToString());
            AppsFlyer.sendEvent(eventKey, additionalDataString);
#endif
        }
#if LatteGames_AppsFlyer
        public void LevelAchieved(int levelIndex)
        {
            logger.Log($"Appsflyer Level achieved: {levelIndex}");
            AppsFlyer.sendEvent(AFInAppEvents.LEVEL_ACHIEVED,
            new Dictionary<string, string>(){
                {AFInAppEvents.LEVEL, levelIndex.ToString()}
            });
        }
#endif
    }
}
