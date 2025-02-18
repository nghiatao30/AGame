using UnityEngine;

namespace LatteGames.Template
{
    public class StateBasedGameLoopManager : MonoBehaviour
    {
        [SerializeField] protected StateGameController gameController = null;
        [SerializeField] protected StateUIController stateUI = null;

        protected virtual void Awake()
        {
            gameController.StateChanged += () => stateUI.SetState(gameController.CurrentState);
            gameController.StateChanged +=
                () => stateUI.GameOverUI.SetButtonGroup(
                    !gameController.CurrentSession?.LevelController.IsVictory() ?? false,
                    gameController.CurrentSession?.LevelController.IsVictory() ?? false);

            stateUI.GameOverUI.Replay +=
                () => gameController.StartLevel(gameController.CurrentSession.LevelAsset);
            stateUI.GameOverUI.Next +=
                () => gameController.StartNextLevel();
        }

        protected virtual void Start()
        {
            gameController.StartNextLevel();
        }

        protected virtual void OnValidate()
        {
            if (gameController == null && GetComponent<StateGameController>() != null)
                gameController = GetComponent<StateGameController>();
        }
    }
}