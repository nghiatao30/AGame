using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RangeVector3Value : RangeValue<Vector3>
{
    #region Constructors
    public RangeVector3Value(Vector3 minValue = default, Vector3 maxValue = default)
    {
        m_RuntimeMinValue = m_MinValue = minValue;
        m_RuntimeMaxValue = m_MaxValue = maxValue;
    }
    #endregion

    public override float CalcInverseLerpValue(Vector3 value)
    {
        if (IsOutOfRange(value))
            return 0f;
        var ba = maxValue - minValue;
        var ca = value - minValue;
        return Mathf.Clamp01(Vector3.Dot(ca, ba) / Vector3.Dot(ba, ba));
    }
    public override Vector3 CalcInterpolatedValue(float weight)
    {
        return Vector3.Lerp(minValue, maxValue, weight);
    }
    public override bool IsOutOfRange(Vector3 value)
    {
        // Check if vector value is collinear with vector min and max by calculate the angle of 2 vector
        var ba = maxValue - minValue;
        var ca = value - minValue;
        if (!Mathf.Approximately(Vector3.Angle(ca, ba), 0f))
            return true;
        // Check whether magnitude of vector CA(min-value) is greater than BA(min-max) or not
        if (ca.magnitude > ba.magnitude)
            return true;
        return false;
    }
}
[CreateAssetMenu(fileName = "RangeVector3Variable", menuName = "HyrphusQ/RangeVariableSO/Vector3")]
public class RangeVector3Variable : RangeVariable<Vector3>
{
    [SerializeField]
    protected RangeVector3Value m_RangeValue;

    public override RangeValue<Vector3> rangeValue => m_RangeValue;
}