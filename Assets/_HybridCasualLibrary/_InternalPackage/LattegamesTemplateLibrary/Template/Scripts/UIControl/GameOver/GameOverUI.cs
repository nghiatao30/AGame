using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LatteGames.Template
{
    public class GameOverUI : MonoBehaviour
    {
        public Action Replay = delegate { };
        public Action Next = delegate { };


        [SerializeField] protected Button replayButton = null;
        [SerializeField] protected Button nextButton = null;

        [SerializeField] private Text title = null;

        protected virtual void Awake()
        {
            replayButton.onClick.AddListener(() => Replay());
            nextButton.onClick.AddListener(() => Next());
        }

        public void SetTitle(string title)
        {
            this.title.text = title;
        }

        public virtual void SetButtonGroup(bool enableReplay, bool enableNext)
        {
            replayButton.gameObject.SetActive(enableReplay);
            nextButton.gameObject.SetActive(enableNext);
        }
    }
}