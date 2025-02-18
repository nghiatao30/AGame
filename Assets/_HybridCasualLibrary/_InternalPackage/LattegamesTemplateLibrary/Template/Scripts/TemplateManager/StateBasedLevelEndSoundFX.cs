using UnityEngine;

namespace LatteGames.Template
{
    public class StateBasedLevelEndSoundFX : MonoBehaviour
    {
        private StateGameController _gameController;
        private StateGameController gameController
        {
            get
            {
                if (_gameController == null)
                    _gameController = GetComponent<StateGameController>();
                return _gameController;
            }
        }

        [SerializeField]
        private AudioSource winSound = null;
        [SerializeField]
        private AudioSource loseSound = null;

        private void Awake()
        {
            gameController.StateChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            gameController.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged()
        {
            if (gameController.CurrentState == StateGameController.State.GameEnded)
            {
                if (gameController.CurrentSession.LevelController.IsVictory())
                {
                    if (winSound != null)
                        winSound.Play();
                }
                else
                {
                    if (loseSound != null)
                        loseSound.Play();
                }
            }
        }
    }
}