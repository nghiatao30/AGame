using System.Collections;
using System.Collections.Generic;
using LatteGames;
using UnityEngine;

public class EZAnimVisibility : MonoBehaviour, IUIVisibilityController
{
    private SubscriptionEvent onStartShow = new SubscriptionEvent();
    private SubscriptionEvent onEndShow = new SubscriptionEvent();
    private SubscriptionEvent onStartHide = new SubscriptionEvent();
    private SubscriptionEvent onEndHide = new SubscriptionEvent();

    [SerializeField]
    private bool m_ShowByDefault;
    [SerializeField]
    private EZAnimBase m_ShowAnim;
    [SerializeField]
    private EZAnimBase m_HideAnim;

    private void Awake()
    {
        if (m_ShowByDefault)
            ShowImmediately();
        else
            HideImmediately();
    }

    public void Show()
    {
        GetOnStartShowEvent().Invoke();
        m_ShowAnim.Play(GetOnEndShowEvent().Invoke);
    }

    public void ShowImmediately()
    {
        GetOnStartShowEvent().Invoke();
        m_ShowAnim.SetToEnd();
        GetOnEndShowEvent().Invoke();
    }

    public void Hide()
    {
        GetOnStartHideEvent().Invoke();
        m_HideAnim.Play(GetOnEndHideEvent().Invoke);
    }

    public void HideImmediately()
    {
        GetOnStartHideEvent().Invoke();
        m_ShowAnim.SetToStart();
        GetOnEndHideEvent().Invoke();
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