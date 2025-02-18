using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangeIntProgressSO", menuName = "HyrphusQ/RangeProgressSO/Int")]
public class RangeIntProgressSO : RangeProgressSO<int>
{
    [SerializeField]
    protected RangeIntVariable m_RangeVariable;

    protected override RangeProgress<int> CreateRangeProgress()
    {
        return m_RangeVariable.CreateRangeProgress(m_RangeProgress.minValue);
    }
}