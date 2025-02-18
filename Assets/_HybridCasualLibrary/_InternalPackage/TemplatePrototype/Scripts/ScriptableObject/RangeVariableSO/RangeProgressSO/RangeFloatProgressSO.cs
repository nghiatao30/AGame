using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangeFloatProgressSO", menuName = "HyrphusQ/RangeProgressSO/Float")]
public class RangeFloatProgressSO : RangeProgressSO<float>
{
    [SerializeField]
    protected RangeFloatVariable m_RangeVariable;

    protected override RangeProgress<float> CreateRangeProgress()
    {
        return m_RangeVariable.CreateRangeProgress(m_RangeVariable.minValue);
    }
}