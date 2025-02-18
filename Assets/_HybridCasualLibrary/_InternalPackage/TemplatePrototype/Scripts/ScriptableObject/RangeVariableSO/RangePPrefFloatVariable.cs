using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangePPrefFloatVariable", menuName = "HyrphusQ/RangeVariableSO/PlayerPrefs/Float")]
public class RangePPrefFloatVariable : RangeFloatVariable
{
    [SerializeField]
    protected PPrefFloatVariable m_PPrefMinVariable, m_PPrefMaxVariable;

    public override float minValue
    {
        get
        {
            m_RangeValue.minValue = m_PPrefMinVariable.value;
            return m_RangeValue.minValue;
        }
        set
        {
            m_RangeValue.minValue = value;
            m_PPrefMinVariable.value = value;
        }
    }
    public override float maxValue
    {
        get
        {
            m_RangeValue.maxValue = m_PPrefMaxVariable.value;
            return m_RangeValue.maxValue;
        }
        set
        {
            m_RangeValue.maxValue = value;
            m_PPrefMaxVariable.value = value;
        }
    }
}