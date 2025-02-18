using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using LatteGames;

public class ZoomingVisibility : MonoBehaviour, IUIVisibilityController
{
    private SubscriptionEvent onStartShow = new SubscriptionEvent();
    private SubscriptionEvent onEndShow = new SubscriptionEvent();
    private SubscriptionEvent onStartHide = new SubscriptionEvent();
    private SubscriptionEvent onEndHide = new SubscriptionEvent();

    [SerializeField] private CanvasGroup _CanvasGroup;
    [SerializeField] private float _AnimationTime = 0.25f;
    [SerializeField] private bool _ShowByDefault = false;

    private void Awake()
    {
        _CanvasGroup.alpha = _ShowByDefault ? 1 : 0;
        _CanvasGroup.interactable = _CanvasGroup.blocksRaycasts = _ShowByDefault;
    }

    public void Show()
    {
        if (_CanvasGroup == null)
            return;
        _CanvasGroup.alpha = 1f;
        _CanvasGroup.interactable = _CanvasGroup.blocksRaycasts = true;
        transform.DOKill();
        transform.localScale = Vector3.zero;
        transform
            .DOScale(1f, _AnimationTime)
            .OnStart(onStartShow.Invoke)
            .OnComplete(onEndShow.Invoke);
    }

    public void ShowImmediately()
    {
        if (_CanvasGroup == null)
            return;
        _CanvasGroup.alpha = 1f;
        _CanvasGroup.interactable = _CanvasGroup.blocksRaycasts = true;
        _CanvasGroup.DOKill();
        transform.localScale = Vector3.zero;
        transform
            .DOScale(1, 0)
            .OnStart(onStartShow.Invoke)
            .OnComplete(onEndShow.Invoke);
    }

    public void Hide()
    {
        if (_CanvasGroup == null)
            return;
        _CanvasGroup.interactable = _CanvasGroup.blocksRaycasts = false;
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform
            .DOScale(0, _AnimationTime)
            .OnStart(onStartHide.Invoke)
            .OnComplete(() => {
                _CanvasGroup.alpha = 0f;
                onEndHide.Invoke();
            });
    }

    public void HideImmediately()
    {
        if (_CanvasGroup == null)
            return;
        _CanvasGroup.interactable = _CanvasGroup.blocksRaycasts = false;
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform
            .DOScale(0, 0)
            .OnStart(onStartHide.Invoke)
            .OnComplete(() => {
                _CanvasGroup.alpha = 0f;
                onEndHide.Invoke();
            });
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