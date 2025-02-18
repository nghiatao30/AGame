using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using UnityEngine;

public class RangePersistentFloatProgress : RangePersistentProgress<float>
{
    protected PPrefFloatVariable m_PersistentValue;

    public RangePersistentFloatProgress(RangeValue<float> rangeValue, PPrefFloatVariable persistentValue) : base(rangeValue, persistentValue.value)
    {
        m_PersistentValue = persistentValue;
        m_PersistentValue.onValueChanged -= OnValueChanged;
        m_PersistentValue.onValueChanged += OnValueChanged;
    }

    private void OnValueChanged(ValueDataChanged<float> data)
    {
        value = m_PersistentValue.value;
    }

    public override float GetPersistentValue()
    {
        return m_PersistentValue.value;
    }
    public override void SavePersistentValue(float value)
    {
        m_PersistentValue.value = value;
    }
}
[CreateAssetMenu(fileName = "RangePPrefFloatProgressSO", menuName = "HyrphusQ/RangeProgressSO/PlayerPrefs/Float")]
public class RangePPrefFloatProgressSO : RangeFloatProgressSO
{
    [SerializeField]
    protected PPrefFloatVariable m_PersistentValue;

    protected override RangeProgress<float> CreateRangeProgress()
    {
        return new RangePersistentFloatProgress(m_RangeVariable.rangeValue, m_PersistentValue);
    }
}