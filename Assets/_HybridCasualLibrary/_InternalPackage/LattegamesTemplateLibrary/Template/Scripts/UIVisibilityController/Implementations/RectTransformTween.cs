using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
public class RectTransformTween : MonoBehaviour, IUIVisibilityController
{
    private SubscriptionEvent onStartShow = new SubscriptionEvent();
    private SubscriptionEvent onEndShow = new SubscriptionEvent();
    private SubscriptionEvent onStartHide = new SubscriptionEvent();
    private SubscriptionEvent onEndHide = new SubscriptionEvent();

    public SubscriptionEvent GetOnEndHideEvent()
    {
        return onEndHide;
    }

    public SubscriptionEvent GetOnEndShowEvent()
    {
        return onEndShow;
    }

    public SubscriptionEvent GetOnStartHideEvent()
    {
        return onStartHide;
    }

    public SubscriptionEvent GetOnStartShowEvent()
    {
        return onStartShow;
    }

    [SerializeField]
    private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float transitionDuration = 0.3f;
    [SerializeField] private Vector2 hidePosition = Vector2.zero;
    [SerializeField] private bool showByDefault = false;
    [SerializeField] private RectTransform container = null;

    private Coroutine animationCr;
    private float normalizedTime = 0;
    private float targetNormalizedTime = 0;

    private void Awake()
    {
        if (showByDefault)
            ShowImmediately();
        else
            HideImmediately();
    }


    public void Hide()
    {
        if (animationCr != null)
        {
            StopCoroutine(animationCr);
        }
        targetNormalizedTime = 1;
        animationCr = StartCoroutine(AnimationCR());
    }

    public void HideImmediately()
    {
        if (animationCr != null)
        {
            StopCoroutine(animationCr);
        }
        normalizedTime = 1;
        var anchoredPosition = Vector2.Lerp(Vector2.zero, hidePosition, normalizedTime);
        container.anchoredPosition = anchoredPosition;
    }

    public void Show()
    {
        if (animationCr != null)
        {
            StopCoroutine(animationCr);
        }
        targetNormalizedTime = 0;
        animationCr = StartCoroutine(AnimationCR());
    }

    private IEnumerator AnimationCR()
    {
        if (normalizedTime == 1)
            onStartShow.Invoke();
        if (normalizedTime == 0)
            onStartHide.Invoke();
        var transitionDirection = Mathf.Sign(targetNormalizedTime - normalizedTime);
        var anchoredPosition = container.anchoredPosition;
        while (Mathf.Sign(targetNormalizedTime - normalizedTime) == transitionDirection)
        {
            yield return null;
            normalizedTime += Time.deltaTime * transitionDirection / transitionDuration;
            anchoredPosition = Vector2.Lerp(Vector2.zero, hidePosition, normalizedTime);
            container.anchoredPosition = anchoredPosition;
        }
        normalizedTime = targetNormalizedTime;
        anchoredPosition = Vector2.Lerp(Vector2.zero, hidePosition, normalizedTime);
        container.anchoredPosition = anchoredPosition;

        if (normalizedTime == 1)
            onEndHide.Invoke();
        if (normalizedTime == 0)
            onEndShow.Invoke();
    }

    public void ShowImmediately()
    {
        if (animationCr != null)
        {
            StopCoroutine(animationCr);
        }
        normalizedTime = 0;
        var anchoredPosition = Vector2.Lerp(Vector2.zero, hidePosition, normalizedTime);
        container.anchoredPosition = anchoredPosition;
    }
}
}