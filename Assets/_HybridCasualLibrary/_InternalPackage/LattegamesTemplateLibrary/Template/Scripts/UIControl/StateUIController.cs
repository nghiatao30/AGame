using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames.Template
{
    public class StateUIController : MonoBehaviour
    {
        [SerializeField] private GameOverUI gameOverUI = null;
        public GameOverUI GameOverUI => gameOverUI;
        public void SetState(StateGameController.State state)
        {
            switch (state)
            {
                case StateGameController.State.Prepare:
                    SetPrepareUI();
                    break;
                case StateGameController.State.Playing:
                    SetPlayingUI();
                    break;
                case StateGameController.State.Pause:
                    SetPauseUI();
                    break;
                case StateGameController.State.GameEnded:
                    SetGameOverUI();
                    break;
            }
        }

        private void SetGameOverUI()
        {
            gameOverUI.GetComponent<IUIVisibilityController>().Show();
        }

        private void SetPauseUI()
        {
            gameOverUI.GetComponent<IUIVisibilityController>().Hide();
        }

        private void SetPlayingUI()
        {
            gameOverUI.GetComponent<IUIVisibilityController>().Hide();
        }

        private void SetPrepareUI()
        {
            gameOverUI.GetComponent<IUIVisibilityController>().Hide();
        }
    }
}