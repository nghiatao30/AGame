using System.Collections.Generic;

namespace LatteGames.Analytics
{
    public static class LevelAchievedEvent{
        public interface ILogger{
            void LevelAchieved(int levelIndex);    
        }

        public static void LevelAchieved(this AnalyticsManager manager, int levelIndex)
        {
            manager.logger.Log($"Send ended level {levelIndex}");
            try
            {
                manager.LogEvent(
                    service => service as ILogger,
                    service => service.LevelAchieved(levelIndex),
                    service => service.SendEventLog("level_achieved", new Dictionary<string, object>() { { "level_index", levelIndex } }));
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }  
        }
    }
}