using UnityEngine;
using System.Collections;

namespace LatteGames.Template
{
    public class StateBasedWinParticleController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private StateGameController gameController = null;

        private void Awake()
        {
            gameController.StateChanged += () =>
            {
                if (gameController.CurrentState == StateGameController.State.GameEnded && gameController.CurrentSession.LevelController.IsVictory())
                {
                    particle.Play();
                }
            };
        }

        private void OnValidate()
        {
            if (gameController == null && GetComponent<StateGameController>() != null)
                gameController = GetComponent<StateGameController>();
        }
    }
}