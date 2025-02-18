using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LatteGames;
using DG.Tweening;

public class ScaleUpVisibility : MonoBehaviour, IUIVisibilityController
{
    private SubscriptionEvent onStartShow = new SubscriptionEvent();
    private SubscriptionEvent onEndShow = new SubscriptionEvent();
    private SubscriptionEvent onStartHide = new SubscriptionEvent();
    private SubscriptionEvent onEndHide = new SubscriptionEvent();

    [SerializeField] private RectTransform _Container = null;
    [SerializeField] private CanvasGroup _CanvasGroup = null;
    [SerializeField] private float _AnimationTime = 0.5f;
    [SerializeField] Ease easeIn = Ease.OutQuint;
    [SerializeField] Ease easeOut = Ease.OutQuint;
    [SerializeField] private bool _ShowByDefault = false;

    public RectTransform container => _Container;
    public CanvasGroup canvasGroup => _CanvasGroup;

    private void Awake()
    {
        if (_ShowByDefault)
        {
            ShowImmediately();
        }
        else
        {
            HideImmediately();
        }
    }

    public void Hide()
    {
        onStartHide?.Invoke();
        _CanvasGroup.interactable = _CanvasGroup.blocksRaycasts = false;
        DOTween.Kill(this);
        _CanvasGroup.alpha = 1;
        _CanvasGroup.DOFade(0, _AnimationTime).SetUpdate(true).SetEase(easeIn);
        _Container.transform.localScale = Vector3.one;
        _Container.transform.DOScale(0, _AnimationTime).SetUpdate(true).SetEase(easeOut).OnComplete(() =>
        {
            _Container.gameObject.SetActive(false);
            onEndHide?.Invoke();
        });
    }

    public void HideImmediately()
    {
        onStartHide?.Invoke();
        _CanvasGroup.interactable = _CanvasGroup.blocksRaycasts = false;
        _CanvasGroup.alpha = 0;
        _Container.gameObject.SetActive(false);
        onEndHide?.Invoke();
    }

    public void Show()
    {
        onStartShow?.Invoke();
        _Container.gameObject.SetActive(true);
        DOTween.Kill(this);
        _CanvasGroup.alpha = 0;
        _CanvasGroup.DOFade(1, _AnimationTime).SetUpdate(true).SetEase(easeIn).OnComplete(() =>
        {
            _CanvasGroup.interactable = _CanvasGroup.blocksRaycasts = true;
        });
        _Container.transform.localScale = Vector3.zero;
        _Container.transform.DOScale(1, _AnimationTime).SetUpdate(true).SetEase(easeIn).OnComplete(() =>
        {
            onEndShow?.Invoke();
        });
    }

    public void ShowImmediately()
    {
        onStartShow?.Invoke();
        _CanvasGroup.interactable = _CanvasGroup.blocksRaycasts = true;
        _CanvasGroup.alpha = 1;
        _Container.gameObject.SetActive(true);
        onEndShow?.Invoke();
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
