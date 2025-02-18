using UnityEngine;
using DG.Tweening;
using System;

namespace LatteGames
{
    public class CanvasGroupVisibility : MonoBehaviour, IUIVisibilityController
    {
        private SubscriptionEvent onStartShow = new SubscriptionEvent();
        private SubscriptionEvent onEndShow = new SubscriptionEvent();
        private SubscriptionEvent onStartHide = new SubscriptionEvent();
        private SubscriptionEvent onEndHide = new SubscriptionEvent();

        [SerializeField] private CanvasGroup _CanvasGroup = null;
        [SerializeField] private float _AnimationTime = 0.5f;
        [SerializeField] private bool _ShowByDefault = false;
        [SerializeField] private bool _IsIndependentUpdate = false;

        private void Awake()
        {
            _CanvasGroup.alpha = _ShowByDefault ? 1 : 0;
            _CanvasGroup.interactable = _CanvasGroup.blocksRaycasts = _ShowByDefault;
        }

        public void Show()
        {
            if (_CanvasGroup == null)
                return;
            _CanvasGroup.DOKill();
            _CanvasGroup.interactable = _CanvasGroup.blocksRaycasts = true;
            _CanvasGroup
                .DOFade(1, _AnimationTime)
                .OnStart(onStartShow.Invoke)
                .OnComplete(onEndShow.Invoke)
                .SetUpdate(_IsIndependentUpdate);
        }

        public void ShowImmediately()
        {
            if (_CanvasGroup == null)
                return;
            _CanvasGroup.DOKill();
            _CanvasGroup.interactable = _CanvasGroup.blocksRaycasts = true;
            _CanvasGroup
                .DOFade(1, 0)
                .OnStart(onStartShow.Invoke)
                .OnComplete(onEndShow.Invoke)
                .SetUpdate(_IsIndependentUpdate);
        }

        public void Hide()
        {
            if (_CanvasGroup == null)
                return;
            _CanvasGroup.DOKill();
            _CanvasGroup.interactable = _CanvasGroup.blocksRaycasts = false;
            _CanvasGroup
                .DOFade(0, _AnimationTime)
                .OnStart(onStartHide.Invoke)
                .OnComplete(onEndHide.Invoke)
                .SetUpdate(_IsIndependentUpdate);
        }

        public void HideImmediately()
        {
            if (_CanvasGroup == null)
                return;
            _CanvasGroup.DOKill();
            _CanvasGroup.interactable = _CanvasGroup.blocksRaycasts = false;
            _CanvasGroup
                .DOFade(0, 0)
                .OnStart(onStartHide.Invoke)
                .OnComplete(onEndHide.Invoke)
                .SetUpdate(_IsIndependentUpdate);
        }

        public SubscriptionEvent GetOnStartShowEvent()
        {
            return onStartShow;
        }

        public SubscriptionEvent GetOnEndShowEvent()
        {
            return onEndShow;
        }

        public SubscriptionEvent GetOnStartHideEvent()
        {
            return onStartHide;
        }

        public SubscriptionEvent GetOnEndHideEvent()
        {
            return onEndHide;
        }
    }
}