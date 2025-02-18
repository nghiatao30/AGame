using System.Collections.Generic;

namespace LatteGames.Analytics
{
    public static class LevelFailedEvent{
        public interface ILogger{
            void LevelFailed(int levelIndex);    
        }

        public static void LevelFailed(this AnalyticsManager manager, int levelIndex)
        {
            manager.logger.Log($"Send failed level {levelIndex}");
            try
            {
                manager.LogEvent(
                    service => service as ILogger,
                    service => service.LevelFailed(levelIndex),
                    service => service.SendEventLog("level_failed", new Dictionary<string, object>() { { "level_index", levelIndex } }));
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(e);   
            }
        }
    }
}