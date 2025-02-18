using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace LatteGames
{
    public class TabbarController : MonoBehaviour
    {
        public UnityEngine.Events.UnityEvent TabIndexChanged;

        [Serializable]
        public class TabButton
        {
            public Button btn;
            public Image bg;
        }

        [SerializeField]
        private List<TabButton> tabButtons = new List<TabButton>();
        private Func<int, bool> ChangeTabIndex;
        private Action<int> TabButtonClicked = delegate {};
        private int currentIndex;

        public void Init(Func<int, bool> ChangeTabIndex)
        {
            this.ChangeTabIndex = ChangeTabIndex;
        }

        public void ChangeTab(int newIndex)
        {
            if (currentIndex == newIndex)
                return;
            var ableToChange = ChangeTabIndex?.Invoke(newIndex) ?? false;
            if (!ableToChange)
                return;
            currentIndex = newIndex;
            UpdateActiveBtnBg(tabButtons[currentIndex]);
            TabIndexChanged.Invoke();
        }

        private void Awake()
        {
            foreach (var button in tabButtons)
            {
                button.btn.onClick.AddListener(() =>
                {
                    ChangeTab(tabButtons.IndexOf(button));
                    TabButtonClicked?.Invoke(tabButtons.IndexOf(button));
                });
            }
        }

        private void UpdateActiveBtnBg(TabButton activeBtn)
        {
            foreach (var button in tabButtons)
            {
                button.bg.gameObject.SetActive(button == activeBtn);
            }
        }
    }
}