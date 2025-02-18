using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangeProgressSO<T> : ScriptableObject
{
    [NonSerialized]
    protected RangeProgress<T> m_RangeProgress;
    public virtual RangeProgress<T> rangeProgress
    {
        get
        {
            if(m_RangeProgress == null)
            {
                m_RangeProgress = CreateRangeProgress();
            }
            return m_RangeProgress;
        }
    }

    protected abstract RangeProgress<T> CreateRangeProgress();
}