using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AbstractTweeningVisibilityAnimation : MonoBehaviour, IVisibilityAnimation
{
    public enum AnimationConfigPair : byte
    {
        SeparateConfig,
        GeneralConfig
    }

    public event Action onStartShowAnimation;
    public event Action onStartHideAnimation;
    public event Action onEndShowAnimation;
    public event Action onEndHideAnimation;

    [SerializeField]
    protected VisibilityState m_DefaultVisibilityState;
    [SerializeField]
    protected AnimationConfigPair m_ConfigPair;
    [SerializeField, DrawIf("DrawSeparateConfig")]
    protected AnimationConfig m_ShowAnimationConfig;
    [SerializeField, DrawIf("DrawSeparateConfig")]
    protected AnimationConfig m_HideAnimationConfig;
    [SerializeField, DrawIf("DrawGeneralConfig")]
    protected AnimationConfig m_GeneralAnimationConfig;

    public AnimationConfig showAnimationConfig => m_ConfigPair == AnimationConfigPair.GeneralConfig ? m_GeneralAnimationConfig : m_ShowAnimationConfig;
    public AnimationConfig hideAnimationConfig => m_ConfigPair == AnimationConfigPair.GeneralConfig ? m_GeneralAnimationConfig : m_HideAnimationConfig;

    protected virtual void RaiseStartShowAnimationEvent()
    {
        onStartShowAnimation?.Invoke();
    }
    protected virtual void RaiseStartHideAnimationEvent()
    {
        onStartHideAnimation?.Invoke();
    }
    protected virtual void RaiseEndShowAnimationEvent()
    {
        onEndShowAnimation?.Invoke();
    }
    protected virtual void RaiseEndHideAnimationEvent()
    {
        onEndHideAnimation?.Invoke();
    }

    public abstract void Show();
    public abstract void ShowImmediately();
    public abstract void Hide();
    public abstract void HideImmediately();

#if UNITY_EDITOR
    protected bool DrawGeneralConfig()
    {
        return m_ConfigPair == AnimationConfigPair.GeneralConfig;
    }
    protected bool DrawSeparateConfig()
    {
        return m_ConfigPair == AnimationConfigPair.SeparateConfig;
    }
#endif
}