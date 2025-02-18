using UnityEngine;
using LatteGames.Analytics;

namespace LatteGames.Template{
public class StateBasedAnalyticsEmitter : MonoBehaviour
{
    [SerializeField] private StateGameController gameController = null;
    private void Awake() {
        gameController.StateChanged +=
            ()=>{
                if(gameController.CurrentState == StateGameController.State.Playing)
                    AnalyticsManager.Instance?.LevelStarted(gameController.LevelStorage.GetLevelIndex(gameController.CurrentSession.LevelAsset));
            };
        gameController.StateChanged +=
            ()=>{
                if(gameController.CurrentState == StateGameController.State.GameEnded)
                {
                    if(gameController.CurrentSession.LevelController.IsVictory())
                        AnalyticsManager.Instance?.LevelAchieved(gameController.LevelStorage.GetLevelIndex(gameController.CurrentSession.LevelAsset));
                    else
                        AnalyticsManager.Instance?.LevelFailed(gameController.LevelStorage.GetLevelIndex(gameController.CurrentSession.LevelAsset));
                }
            };
    }

    private void OnValidate() {
        if(gameController == null && GetComponent<StateGameController>() != null)
            gameController = GetComponent<StateGameController>();
    }
}
}