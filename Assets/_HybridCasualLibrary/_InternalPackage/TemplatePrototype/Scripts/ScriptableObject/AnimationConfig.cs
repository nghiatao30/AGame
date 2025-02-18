using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationConfig", menuName = "HyrphusQ/AnimationConfig", order = 0)]
public class AnimationConfig : ScriptableObject
{
    [SerializeField]
    private Ease m_Ease;
    [SerializeField, MinValue(0f), PropertyOrder(-1)]
    private float m_Duration = 0.5f;
    [SerializeField, ShowIf("m_Ease", Ease.INTERNAL_Custom)]
    private AnimationCurve m_Curve;
    
    public bool isUseCustomEasing => ease == Ease.INTERNAL_Custom;
    public float duration => m_Duration;
    public Ease ease => m_Ease;
    public AnimationCurve curve => m_Curve;
}