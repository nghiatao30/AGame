using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.Analytics
{
    [OptionalDependency("AppMetrica", "LatteGames_AppMetrica")]
    public class AppMetricaAnalyticsService : AnalyticsService, LevelStartedEvent.ILogger, LevelAchievedEvent.ILogger
    {
        public void LevelAchieved(int levelIndex)
        {
            logger.Log($"Appmetric level achieved {levelIndex}");
            SendEventLog("level_finish", new Dictionary<string, object>(){
                {"level_number", levelIndex},
                {"level_name", $"level_{levelIndex+1}"}
            });
            #if LatteGames_AppMetrica
            AppMetrica.Instance?.SendEventsBuffer();
            #endif
        }

        public void LevelStarted(int levelIndex)
        {
            logger.Log($"Appmetric level started {levelIndex}");
            SendEventLog("level_start", new Dictionary<string, object>(){
                {"level_number", levelIndex},
                {"level_name", $"level_{levelIndex+1}"}
            });
            #if LatteGames_AppMetrica
            AppMetrica.Instance?.SendEventsBuffer();
            #endif
        }
 
        public override void SendEventLog(string eventKey, Dictionary<string, object> additionData)
        {
            logger.Log($"Appmetric custom event {eventKey}");
            #if LatteGames_AppMetrica
            AppMetrica.Instance?.ReportEvent(eventKey, additionData);
            #endif
        }
    }
}