using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class TranslateTweeningVisibilityAnimation : AbstractTweeningVisibilityAnimation
{
    public enum PositionProperty : byte
    {
        LocalPosition,
        WoldPosition,
        AnchoredPosition
    }

    [SerializeField]
    private PositionProperty m_PositionProperty;
    [SerializeField]
    private Vector3 m_OriginalPosition;
    [SerializeField]
    private Vector3 m_DestinationPosition;

    private RectTransform rectTransform => transform as RectTransform;

    protected void OnValidate()
    {
        if (m_OriginalPosition == Vector3.zero && m_DestinationPosition == Vector3.zero)
        {
            switch (m_PositionProperty)
            {
                case PositionProperty.LocalPosition:
                    m_OriginalPosition = transform.localPosition;
                    m_DestinationPosition = transform.localPosition;
                    break;
                case PositionProperty.WoldPosition:
                    m_OriginalPosition = transform.position;
                    m_DestinationPosition = transform.position;
                    break;
                case PositionProperty.AnchoredPosition:
                    m_OriginalPosition = rectTransform.anchoredPosition3D;
                    m_DestinationPosition = rectTransform.anchoredPosition3D;
                    break;
                default:
                    break;
            }
        }
    }

    public override void Show()
    {
        if (m_DefaultVisibilityState == VisibilityState.Visible)
            return;
        m_DefaultVisibilityState = VisibilityState.Visible;
        transform.DOKill(true);

        if (m_PositionProperty == PositionProperty.LocalPosition)
        {
            transform.localPosition = m_OriginalPosition;
            var sequence = DOTween.Sequence();
            sequence
                .Join(showAnimationConfig.isUseCustomEasing
                ? transform.DOLocalMove(m_DestinationPosition, showAnimationConfig.duration).SetEase(showAnimationConfig.curve)
                : transform.DOLocalMove(m_DestinationPosition, showAnimationConfig.duration).SetEase(showAnimationConfig.ease))
                .OnStart(RaiseStartShowAnimationEvent)
                .OnComplete(RaiseEndShowAnimationEvent)
                .Play();
            return;
        }
        if (m_PositionProperty == PositionProperty.WoldPosition)
        {
            transform.position = m_OriginalPosition;
            var sequence = DOTween.Sequence();
            sequence
                .Join(showAnimationConfig.isUseCustomEasing
                ? transform.DOMove(m_DestinationPosition, showAnimationConfig.duration).SetEase(showAnimationConfig.curve)
                : transform.DOMove(m_DestinationPosition, showAnimationConfig.duration).SetEase(showAnimationConfig.ease))
                .OnStart(RaiseStartShowAnimationEvent)
                .OnComplete(RaiseEndShowAnimationEvent)
                .Play();
            return;
        }
        if (m_PositionProperty == PositionProperty.AnchoredPosition)
        {
            rectTransform.anchoredPosition3D = m_OriginalPosition;
            var sequence = DOTween.Sequence();
            sequence
                .Join(showAnimationConfig.isUseCustomEasing
                ? rectTransform.DOAnchorPos3D(m_DestinationPosition, showAnimationConfig.duration).SetEase(showAnimationConfig.curve)
                : rectTransform.DOAnchorPos3D(m_DestinationPosition, showAnimationConfig.duration).SetEase(showAnimationConfig.ease))
                .OnStart(RaiseStartShowAnimationEvent)
                .OnComplete(RaiseEndShowAnimationEvent)
                .Play();
            return;
        }
    }
    public override void Hide()
    {
        if (m_DefaultVisibilityState == VisibilityState.Hidden)
            return;
        m_DefaultVisibilityState = VisibilityState.Hidden;
        transform.DOKill(true);

        if (m_PositionProperty == PositionProperty.LocalPosition)
        {
            transform.localPosition = m_DestinationPosition;
            var sequence = DOTween.Sequence();
            sequence
                .Join(hideAnimationConfig.isUseCustomEasing
                ? transform.DOLocalMove(m_OriginalPosition, hideAnimationConfig.duration).SetEase(hideAnimationConfig.curve)
                : transform.DOLocalMove(m_OriginalPosition, hideAnimationConfig.duration).SetEase(hideAnimationConfig.ease))
                .OnStart(RaiseStartHideAnimationEvent)
                .OnComplete(RaiseEndHideAnimationEvent)
                .Play();
            return;
        }
        if (m_PositionProperty == PositionProperty.WoldPosition)
        {
            transform.position = m_DestinationPosition;
            var sequence = DOTween.Sequence();
            sequence
                .Join(hideAnimationConfig.isUseCustomEasing
                ? transform.DOMove(m_OriginalPosition, hideAnimationConfig.duration).SetEase(hideAnimationConfig.curve)
                : transform.DOMove(m_OriginalPosition, hideAnimationConfig.duration).SetEase(hideAnimationConfig.ease))
                .OnStart(RaiseStartHideAnimationEvent)
                .OnComplete(RaiseEndHideAnimationEvent)
                .Play();
            return;
        }
        if (m_PositionProperty == PositionProperty.AnchoredPosition)
        {
            rectTransform.anchoredPosition3D = m_DestinationPosition;
            var sequence = DOTween.Sequence();
            sequence
                .Join(hideAnimationConfig.isUseCustomEasing
                ? rectTransform.DOAnchorPos3D(m_OriginalPosition, hideAnimationConfig.duration).SetEase(hideAnimationConfig.curve)
                : rectTransform.DOAnchorPos3D(m_OriginalPosition, hideAnimationConfig.duration).SetEase(hideAnimationConfig.ease))
                .OnStart(RaiseStartHideAnimationEvent)
                .OnComplete(RaiseEndHideAnimationEvent)
                .Play();
            return;
        }
    }

    public override void ShowImmediately()
    {
        if (m_DefaultVisibilityState == VisibilityState.Visible)
            return;
        m_DefaultVisibilityState = VisibilityState.Visible;
        transform.DOKill(true);

        if (m_PositionProperty == PositionProperty.LocalPosition)
        {
            transform.localPosition = m_DestinationPosition;
            return;
        }
        if (m_PositionProperty == PositionProperty.WoldPosition)
        {
            transform.position = m_DestinationPosition;
            return;
        }
        if (m_PositionProperty == PositionProperty.AnchoredPosition)
        {
            rectTransform.anchoredPosition3D = m_DestinationPosition;
            return;
        }
    }

    public override void HideImmediately()
    {
        if (m_DefaultVisibilityState == VisibilityState.Hidden)
            return;
        m_DefaultVisibilityState = VisibilityState.Hidden;
        transform.DOKill(true);

        if (m_PositionProperty == PositionProperty.LocalPosition)
        {
            transform.localPosition = m_OriginalPosition;
            return;
        }
        if (m_PositionProperty == PositionProperty.WoldPosition)
        {
            transform.position = m_OriginalPosition;
            return;
        }
        if (m_PositionProperty == PositionProperty.AnchoredPosition)
        {
            rectTransform.anchoredPosition3D = m_OriginalPosition;
            return;
        }
    }
}