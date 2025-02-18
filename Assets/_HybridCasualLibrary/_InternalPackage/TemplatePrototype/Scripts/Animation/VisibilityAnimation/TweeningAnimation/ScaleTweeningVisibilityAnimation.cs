using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleTweeningVisibilityAnimation : AbstractTweeningVisibilityAnimation
{
    [SerializeField]
    private Vector3 m_OriginalLocalScale = Vector3.zero;
    [SerializeField]
    private Vector3 m_DestinationLocalScale = Vector3.one;

    public override void Show()
    {
        if (m_DefaultVisibilityState == VisibilityState.Visible)
            return;
        m_DefaultVisibilityState = VisibilityState.Visible;
        transform.DOKill(true);
        transform.localScale = m_OriginalLocalScale;

        var sequence = DOTween.Sequence();
        sequence
            .Join(showAnimationConfig.isUseCustomEasing 
            ? transform.DOScale(m_DestinationLocalScale, showAnimationConfig.duration).SetEase(showAnimationConfig.curve)
            : transform.DOScale(m_DestinationLocalScale, showAnimationConfig.duration).SetEase(showAnimationConfig.ease))
            .OnStart(RaiseStartShowAnimationEvent)
            .OnComplete(RaiseEndShowAnimationEvent)
            .Play();
    }
    public override void Hide()
    {
        if (m_DefaultVisibilityState == VisibilityState.Hidden)
            return;
        m_DefaultVisibilityState = VisibilityState.Hidden;
        transform.DOKill(true);
        transform.localScale = m_DestinationLocalScale;

        var sequence = DOTween.Sequence();
        sequence
            .Join(hideAnimationConfig.isUseCustomEasing
            ? transform.DOScale(m_DestinationLocalScale, hideAnimationConfig.duration).SetEase(hideAnimationConfig.curve)
            : transform.DOScale(m_OriginalLocalScale, hideAnimationConfig.duration).SetEase(hideAnimationConfig.ease))
            .OnStart(RaiseStartHideAnimationEvent)
            .OnComplete(RaiseEndHideAnimationEvent)
            .Play();
    }
    public override void ShowImmediately()
    {
        if (m_DefaultVisibilityState == VisibilityState.Visible)
            return;
        m_DefaultVisibilityState = VisibilityState.Visible;
        transform.DOKill(true);
        transform.localScale = m_DestinationLocalScale;
    }
    public override void HideImmediately()
    {
        if (m_DefaultVisibilityState == VisibilityState.Hidden)
            return;
        m_DefaultVisibilityState = VisibilityState.Hidden;
        transform.DOKill(true);
        transform.localScale = m_OriginalLocalScale;
    }
}