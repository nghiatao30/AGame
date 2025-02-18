using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VisibilityState : byte
{
    Visible,
    Hidden
}

public interface IVisibilityAnimation
{
    event Action onStartShowAnimation;
    event Action onStartHideAnimation;
    event Action onEndShowAnimation;
    event Action onEndHideAnimation;

    AnimationConfig showAnimationConfig { get; }
    AnimationConfig hideAnimationConfig { get; }

    void Show();
    void ShowImmediately();
    void Hide();
    void HideImmediately();
}