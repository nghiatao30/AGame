using System.Collections.Generic;

namespace LatteGames.Analytics
{
    public static class LevelStartedEvent{
        public interface ILogger{
            void LevelStarted(int levelIndex);    
        }

        public static void LevelStarted(this AnalyticsManager manager, int levelIndex)
        {
            manager.logger.Log($"Send started level {levelIndex}");
            try
            {
                manager.LogEvent(
                    service => service as ILogger,
                    service => service.LevelStarted(levelIndex),
                    service => service.SendEventLog("level_started", new Dictionary<string, object>() { { "level_index", levelIndex } }));
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(e);   
            }
        }
    }
}