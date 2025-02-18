using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimationSelectionResponse : MonoBehaviour, ISelectionResponse
{
    [SerializeField]
    private AbstractTweeningVisibilityAnimation m_TweeningVisibilityAnimation;

    public void Select(bool isForceSelect = false)
    {
        m_TweeningVisibilityAnimation.Show();
    }

    public void Deselect()
    {
        m_TweeningVisibilityAnimation.Hide();
    }
}