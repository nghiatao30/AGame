using System.Collections.Generic;
using UnityEngine;

#if LatteGames_UnityAnalytics
using UnityEngine.Analytics;
#endif
namespace LatteGames.Analytics
{
        [OptionalDependency("UnityEngine.Analytics.AnalyticsEvent", "LatteGames_UnityAnalytics")]
    public class UnityAnalyticsService : AnalyticsService, LevelAchievedEvent.ILogger, LevelStartedEvent.ILogger
    {
        public void LevelAchieved(int levelIndex)
        {
#if LatteGames_UnityAnalytics            
            logger.Log($"Unity Analytics: Send level achieved event lv{levelIndex}");
            AnalyticsEvent.LevelComplete(levelIndex, new Dictionary<string, object>());
#endif     
        }

        public void LevelStarted(int levelIndex)
        {
#if LatteGames_UnityAnalytics
            logger.Log($"Unity Analytics: Send level started event lv{levelIndex}");
            AnalyticsEvent.LevelStart(levelIndex, new Dictionary<string, object>());
#endif
        }

        public override void SendEventLog(string eventKey, Dictionary<string, object> additionData)
        {
#if LatteGames_UnityAnalytics
            logger.Log($"Unity Analytics: Sending custom event log {eventKey}");
            AnalyticsEvent.Custom(eventKey, additionData);
#endif
        }
    }
}