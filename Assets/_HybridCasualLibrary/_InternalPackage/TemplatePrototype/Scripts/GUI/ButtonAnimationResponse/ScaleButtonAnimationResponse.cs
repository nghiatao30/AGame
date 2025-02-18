using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ScaleButtonAnimationResponse : ButtonAnimationResponse
{
    [SerializeField]
    private Vector3 m_MouseDownScale = Vector3.one * 0.95f;
    [SerializeField]
    private AnimationConfig m_AnimationConfig;

    protected override void OnPointerDown_Internal(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(m_MouseDownScale, m_AnimationConfig.duration);
    }

    protected override void OnPointerUp_Internal(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(Vector3.one, m_AnimationConfig.duration);
    }
}