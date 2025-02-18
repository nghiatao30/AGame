using System.Collections;
using System.Collections.Generic;
using LatteGames;
using UnityEngine;

public class TweeningAnimationVisibility : MonoBehaviour, IUIVisibilityController
{
    private SubscriptionEvent onStartShow = null;
    private SubscriptionEvent onEndShow = null;
    private SubscriptionEvent onStartHide = null;
    private SubscriptionEvent onEndHide = null;

    [SerializeField]
    private AbstractTweeningVisibilityAnimation m_TweeningVisibilityAnimation;

    public void Show()
    {
        m_TweeningVisibilityAnimation.Show();
    }

    public void ShowImmediately()
    {
        m_TweeningVisibilityAnimation.Show();
    }

    public void Hide()
    {
        m_TweeningVisibilityAnimation.Hide();
    }

    public void HideImmediately()
    {
        m_TweeningVisibilityAnimation.Hide();
    }

    public SubscriptionEvent GetOnStartShowEvent()
    {
        if (onStartShow == null)
        {
            onStartShow = new SubscriptionEvent();
            m_TweeningVisibilityAnimation.onStartShowAnimation += onStartShow.Invoke;
        }
        return onStartShow;
    }

    public SubscriptionEvent GetOnEndShowEvent()
    {
        if (onEndShow == null)
        {
            onEndShow = new SubscriptionEvent();
            m_TweeningVisibilityAnimation.onEndShowAnimation += onEndShow.Invoke;
        }
        return onEndShow;
    }

    public SubscriptionEvent GetOnStartHideEvent()
    {
        if (onStartHide == null)
        {
            onStartHide = new SubscriptionEvent();
            m_TweeningVisibilityAnimation.onStartHideAnimation += onStartHide.Invoke;
        }
        return onStartHide;
    }

    public SubscriptionEvent GetOnEndHideEvent()
    {
        if (onEndHide == null)
        {
            onEndHide = new SubscriptionEvent();
            m_TweeningVisibilityAnimation.onEndHideAnimation += onEndHide.Invoke;
        }
        return onEndHide;
    }
}