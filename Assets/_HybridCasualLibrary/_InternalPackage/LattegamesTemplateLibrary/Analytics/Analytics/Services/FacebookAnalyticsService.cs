using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if LatteGames_FB
using Facebook.Unity;
#endif

namespace LatteGames.Analytics
{
    [OptionalDependency("Facebook.Unity.FB", "LatteGames_FB")]
    public class FacebookAnalyticsService : AnalyticsService, LevelAchievedEvent.ILogger, LevelStartedEvent.ILogger   
    {
#if LatteGames_FB
        // Awake function from Unity's MonoBehavior
        void Awake()
        {
            if (!FB.IsInitialized)
            {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
        }

        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }
#endif

        public void LevelAchieved(int levelIndex)
        {
#if LatteGames_FB
            logger.Log($"FB Log: level achieved {levelIndex}");
            _ = FBLog(AppEventName.AchievedLevel, parameters: new Dictionary<string, object>()
            {
                {AppEventParameterName.Level, levelIndex}
            });
#endif
        }

        public void LevelStarted(int levelIndex)
        {
#if LatteGames_FB
            logger.Log($"FB Log: level started {levelIndex}");            
            _ = FBLog("level_started", parameters: new Dictionary<string, object>()
            {
                {AppEventParameterName.Level, levelIndex}
            });
#endif
        }

        public override void SendEventLog(string eventKey, Dictionary<string, object> additionData)
        {
            _ = FBLog(
                eventKey,
                parameters: additionData
            );
        }

        private async Task FBLog(string key, Dictionary<string, object> parameters)
        {
#if LatteGames_FB
            logger.Log($"FB Log attempt: {key}");
            while (!FB.IsInitialized)
                await Task.Yield();
            logger.Log($"FB Log: {key}");
            FB.LogAppEvent(key, parameters: parameters);
#endif
            await Task.Yield();
        }
    }
}