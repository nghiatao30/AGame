using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if LatteGames_GA
using GameAnalyticsSDK;
#endif
namespace LatteGames.Analytics
{
        [OptionalDependency("GameAnalyticsSDK.GameAnalytics", "LatteGames_GA")]
    public class GameAnalyticsService : AnalyticsService, LevelAchievedEvent.ILogger, LevelStartedEvent.ILogger, LevelFailedEvent.ILogger
    {
#if LatteGames_GA
        private void Awake() {
            GameAnalytics.Initialize();
        }
#endif
        public void LevelAchieved(int levelIndex)
        {
#if LatteGames_GA
            logger.Log($"Send level achieved event lv{levelIndex} to GA");
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $"Boss{levelIndex + 1}");
#endif
        }

        public void LevelStarted(int levelIndex)
        {
#if LatteGames_GA
            logger.Log($"Send level started event lv{levelIndex} to GA");
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, $"Boss{levelIndex + 1}");
#endif
        }

        public void LevelFailed(int levelIndex)
        {
#if LatteGames_GA
            logger.Log($"Send level failed event lv{levelIndex} to GA");
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, $"Boss{levelIndex + 1}");
#endif
        }

        public override void SendEventLog(string eventKey, Dictionary<string, object> additionData)
        {
            logger.Log($"sending custom event log {eventKey} to GA");
        }
    }
}