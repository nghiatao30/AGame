using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RangeIntValue : RangeValue<int>
{
    #region Constructors
    public RangeIntValue(int minValue = 0, int maxValue = 0)
    {
        m_RuntimeMinValue = m_MinValue = minValue;
        m_RuntimeMaxValue = m_MaxValue = maxValue;
    }
    #endregion

    public override float CalcInverseLerpValue(int value)
    {
        return Mathf.InverseLerp(minValue, maxValue, value);
    }
    public override int CalcInterpolatedValue(float weight)
    {
        return Mathf.RoundToInt(Mathf.Lerp(minValue, maxValue, weight));
    }
    public override bool IsOutOfRange(int value)
    {
        return value < minValue || value > maxValue;
    }
}
[CreateAssetMenu(fileName = "RangeIntVariable", menuName = "HyrphusQ/RangeVariableSO/Int")]
public class RangeIntVariable : RangeVariable<int>
{
    [SerializeField]
    protected RangeIntValue m_RangeValue;

    public override RangeValue<int> rangeValue => m_RangeValue;
}