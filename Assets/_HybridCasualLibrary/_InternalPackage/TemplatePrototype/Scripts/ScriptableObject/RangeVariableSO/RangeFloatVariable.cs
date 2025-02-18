using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RangeFloatValue : RangeValue<float>
{
    #region Constructors
    public RangeFloatValue(float minValue = 0f, float maxValue = 0f)
    {
        m_RuntimeMinValue = m_MinValue = minValue;
        m_RuntimeMaxValue = m_MaxValue = maxValue;
    }
    #endregion

    public override float CalcInverseLerpValue(float value)
    {
        return Mathf.InverseLerp(minValue, maxValue, value);
    }
    public override float CalcInterpolatedValue(float weight)
    {
        return Mathf.Lerp(minValue, maxValue, weight);
    }
    public override bool IsOutOfRange(float value)
    {
        return value < minValue || value > maxValue;
    }
}
[CreateAssetMenu(fileName = "RangeFloatVariable", menuName = "HyrphusQ/RangeVariableSO/Float")]
public class RangeFloatVariable : RangeVariable<float>
{
    [SerializeField]
    protected RangeFloatValue m_RangeValue;

    public override RangeValue<float> rangeValue => m_RangeValue;
}