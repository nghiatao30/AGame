using System;
using System.Collections;
using UnityEngine;

namespace LatteGames
{
    public class StateGameController : MonoBehaviour
    {
        public event Action StateChanged = delegate { };

        public enum State
        {
            Prepare,
            Playing,
            Pause,
            GameEnded
        }

        protected State currentState = State.GameEnded;
        public State CurrentState
        {
            get => currentState;
            protected set
            {
                State oldState = currentState;
                currentState = value;
                if (currentState != oldState)
                    StateChanged.Invoke();
            }
        }
        [SerializeField]
        protected LevelStorage levelStorage = null;
        public LevelStorage LevelStorage => levelStorage;
        public PlayerPrefPersistent.Int playerAchievedLevel = new PlayerPrefPersistent.Int("PLAYER_ACHIEVED_LEVEL", -1);
        protected GameSession currentSession = null;
        public GameSession CurrentSession => currentSession;

        protected CoroutineRunner levelLoadingRunner;

        protected virtual void Awake()
        {
            levelLoadingRunner = CoroutineRunner.CreateCoroutineRunner(false);
            levelLoadingRunner.transform.SetParent(transform);
        }

        protected virtual IEnumerator UnloadLevel()
        {
            if (currentSession != null)
            {
                var unload = currentSession.LevelAsset.UnLoadLevelAsync(currentSession.LevelController);
                yield return new WaitUntil(() => unload.Finished());
            }
        }

        protected virtual IEnumerator StartGameLoop(LevelAsset level)
        {
            yield return UnloadLevel();
            var loading = level.LoadLevelAsync();
            yield return new WaitUntil(() => loading.Finished());
            var newSession = new GameSession(level, loading.GetLevelController());
            StartCoroutine(GameLoopCR(newSession));
        }

        protected virtual IEnumerator GameLoopCR(GameSession session)
        {
            currentSession = session;
            CurrentState = State.Playing;
            bool gameEnded = false;
            Action<LevelController> gameEndListener = _ => gameEnded = true;
            session.LevelController.LevelEnded += gameEndListener;
            yield return new WaitUntil(() => gameEnded);
            session.LevelController.LevelEnded -= gameEndListener;
            CurrentState = State.GameEnded;
            if (session.LevelController.IsVictory())
                playerAchievedLevel.Value = levelStorage.GetLevelIndex(session.LevelAsset);
        }

        public virtual void Prepare()
        {
            if (CurrentState != State.GameEnded) return;
            CurrentState = State.Prepare;
        }

        public virtual void Pause()
        {
            if (CurrentState != State.Playing)
                return;
            currentSession.LevelController.PauseLevel();
            CurrentState = State.Pause;
        }

        public virtual void Resume()
        {
            if (CurrentState != State.Pause)
                return;
            currentSession.LevelController.ResumeLevel();
            CurrentState = State.Playing;
        }

        public virtual void StartNextLevel()
        {
            if (levelStorage == null)
            {
                Debug.LogWarning("LevelStorage is null");
                return;
            }
            var nextLevel = levelStorage.GetLevel(playerAchievedLevel.Value + 1, LevelStorage.EndOfLevelBehaviour.LoopBack);
            StartLevel(nextLevel);
        }

        public virtual void StartLevel(LevelAsset level)
        {
            levelLoadingRunner.StartManagedCoroutine(
                StartGameLoop(level),
                CoroutineRunner.InteruptBehaviour.Ignore
            );
        }
    }
}