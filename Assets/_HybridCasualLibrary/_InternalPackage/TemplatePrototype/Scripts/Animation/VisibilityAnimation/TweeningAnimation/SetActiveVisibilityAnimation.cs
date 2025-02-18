using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveVisibilityAnimation : AbstractTweeningVisibilityAnimation
{
    public override void Show()
    {
        if (m_DefaultVisibilityState == VisibilityState.Visible)
            return;
        m_DefaultVisibilityState = VisibilityState.Visible;
        gameObject.SetActive(true);
    }
    public override void Hide()
    {
        if (m_DefaultVisibilityState == VisibilityState.Hidden)
            return;
        m_DefaultVisibilityState = VisibilityState.Hidden;
        gameObject.SetActive(false);
    }
    public override void ShowImmediately()
    {
        if (m_DefaultVisibilityState == VisibilityState.Visible)
            return;
        m_DefaultVisibilityState = VisibilityState.Visible;
        gameObject.SetActive(true);
    }
    public override void HideImmediately()
    {
        if (m_DefaultVisibilityState == VisibilityState.Hidden)
            return;
        m_DefaultVisibilityState = VisibilityState.Hidden;
        gameObject.SetActive(false);
    }
}