using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if LatteGames_CLIK
using Tabtale.TTPlugins;
#endif

namespace LatteGames.Analytics{
    [OptionalDependency("Tabtale.TTPlugins.TTPCore", "LatteGames_CLIK")]
    public class CLIKAnalyticsService : AnalyticsService
    #if LatteGames_CLIK
    ,LevelStartedEvent.ILogger, LevelAchievedEvent.ILogger, LevelFailedEvent.ILogger //add level fail event
    #endif
    {
#if LatteGames_CLIK
        private void Awake()
        {
            TTPCore.OnShouldAskForIDFA += OnShouldAskForIDEA;
            TTPCore.Setup();
        }

        private void OnDestroy() {
            TTPCore.OnShouldAskForIDFA -= OnShouldAskForIDEA;
        }

        private void OnShouldAskForIDEA(bool askForIDEA)
        {
            if (askForIDEA)
                TTPCore.AskForIDFA();
        }

        public void LevelStarted(int levelIndex)
        {
            TTPGameProgression.FirebaseEvents.MissionStarted(levelIndex + 1, new Dictionary<string, object>());
        }

        public void LevelAchieved(int levelIndex)
        {
            TTPGameProgression.FirebaseEvents.MissionComplete(new Dictionary<string, object>());
        }

        public void LevelFailed(int levelIndex)
        {
            TTPGameProgression.FirebaseEvents.MissionFailed(new Dictionary<string, object>());
        }
#endif
        public override void SendEventLog(string eventKey, Dictionary<string, object> additionData)
        {
#if LatteGames_CLIK
                
                //No implementation for now as we have no clue what target to send
#endif
        }

        

        
    }
}