using System;
using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using UnityEngine;

public class RangePersistentIntProgress : RangePersistentProgress<int>
{
    protected PPrefIntVariable m_PersistentValue;

    public RangePersistentIntProgress(RangeValue<int> rangeValue, PPrefIntVariable persistentValue) : base(rangeValue, persistentValue.value)
    {
        m_PersistentValue = persistentValue;
        m_PersistentValue.onValueChanged -= OnValueChanged;
        m_PersistentValue.onValueChanged += OnValueChanged;
    }

    private void OnValueChanged(ValueDataChanged<int> data)
    {
        value = m_PersistentValue.value;
    }

    public override int GetPersistentValue()
    {
        return m_PersistentValue.value;
    }
    public override void SavePersistentValue(int value)
    {
        m_PersistentValue.value = value;
    }
}
[CreateAssetMenu(fileName = "RangePPrefIntProgressSO", menuName = "HyrphusQ/RangeProgressSO/PlayerPrefs/Int")]
public class RangePPrefIntProgressSO : RangeIntProgressSO
{
    [SerializeField]
    protected PPrefIntVariable m_PersistentValue;

    protected override RangeProgress<int> CreateRangeProgress()
    {
        return new RangePersistentIntProgress(m_RangeVariable.rangeValue, m_PersistentValue);
    }
}