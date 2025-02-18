using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyrphusQ.Events;

public abstract class RangePersistentProgress<T> : RangeProgress<T>
{
    public RangePersistentProgress(RangeValue<T> rangeValue, T value) : base(rangeValue, value)
    {
    }

    public override T value
    {
        get
        {
            m_Value = GetPersistentValue();
            return m_Value;
        }
        set
        {
            base.value = value;
            SavePersistentValue(value);
        }

    }

    public abstract T GetPersistentValue();
    public abstract void SavePersistentValue(T value);
}
public class RangeProgress<T>
{
    public event Action<ValueDataChanged<T>> onValueChanged;

    protected T m_Value;
    protected float m_InverseLerpValue;
    protected RangeValue<T> m_RangeValue;

    #region Properties
    public virtual T minValue { get => m_RangeValue.minValue; set => m_RangeValue.minValue = value; }
    public virtual T maxValue { get => m_RangeValue.maxValue; set => m_RangeValue.maxValue = value; }
    public virtual T value
    {
        get => m_Value;
        set
        {
            var oldValue = m_Value;
            m_Value = value;
            m_InverseLerpValue = m_RangeValue.CalcInverseLerpValue(value);
            if (!EqualityComparer<T>.Default.Equals(oldValue, value))
                onValueChanged?.Invoke(new ValueDataChanged<T>(oldValue, m_Value));
        }
    }
    public float inverseLerpValue => m_InverseLerpValue;
    #endregion

    public RangeProgress(RangeValue<T> rangeValue, T value)
    {
        m_Value = value;
        m_RangeValue = rangeValue;
        m_InverseLerpValue = rangeValue.CalcInverseLerpValue(value);
        onValueChanged = delegate { };
    }
    public float CalcInverseLerpValue(T value) => m_RangeValue.CalcInverseLerpValue(value);
    public T CalcInterpolatedValue(float weight) => m_RangeValue.CalcInterpolatedValue(weight);
    public void NotifyDatasetChanged()
    {
        onValueChanged?.Invoke(new ValueDataChanged<T>(value, value));
    }
}
[Serializable]
public abstract class RangeValue<T> : ISerializationCallbackReceiver
{
    #region Constructors
    public RangeValue()
    {

    }
    public RangeValue(T minValue, T maxValue)
    {
        m_RuntimeMinValue = m_MinValue = minValue;
        m_RuntimeMaxValue = m_MaxValue = maxValue;
    }
    #endregion

    [SerializeField]
    protected T m_MinValue;
    [SerializeField]
    protected T m_MaxValue;

    [NonSerialized]
    protected T m_RuntimeMinValue;
    [NonSerialized]
    protected T m_RuntimeMaxValue;

    public T minValue
    {
        get => m_RuntimeMinValue;
        set => m_RuntimeMinValue = value;
    }
    public T maxValue
    {
        get => m_RuntimeMaxValue;
        set => m_RuntimeMaxValue = value;
    }

    /// <summary>
    /// Calculate inverse interpolated value range (0 - 1)
    /// </summary>
    /// <param name="value">Current value in range (min-max)</param>
    /// <returns>Return inverse interpolated value range (0 - 1)</returns>
    public abstract float CalcInverseLerpValue(T value);
    /// <summary>
    /// Calculate interpolated value range (min - max) base on weight (0 - 1)
    /// </summary>
    /// <param name="weight">Weight factor range (0 - 1)</param>
    /// <returns>Return interpolated value range (min - max)</returns>
    public abstract T CalcInterpolatedValue(float weight);
    /// <summary>
    /// Check whether value is out of range or not
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>Return true if value out of range otherwise false</returns>
    public abstract bool IsOutOfRange(T value);

    public override string ToString()
    {
        return $"{base.ToString()} [{minValue}, {maxValue}]";
    }
    public virtual void OnBeforeSerialize()
    {
        // Do nothing
    }
    public virtual void OnAfterDeserialize()
    {
        // Assign serialized value for runtime value because runtime value is not serialized.
        m_RuntimeMinValue = m_MinValue;
        m_RuntimeMaxValue = m_MaxValue;
    }
}
public abstract class RangeVariable<T> : ScriptableObject
{
    public virtual T minValue
    {
        get => rangeValue.minValue;
        set => rangeValue.minValue = value;
    }
    public virtual T maxValue
    {
        get => rangeValue.maxValue;
        set => rangeValue.maxValue = value;
    }
    public abstract RangeValue<T> rangeValue { get; }

#if ODIN_INSPECTOR_3 && UNITY_EDITOR
    [Sirenix.OdinInspector.OnInspectorGUI]
    protected virtual void OnInspectorGUI()
    {
        UnityEditor.EditorGUILayout.LabelField($"Current value: {rangeValue}");
    }
#endif

    /// <summary>
    /// Create range progress
    /// </summary>
    /// <param name="value">Inital value</param>
    /// <returns>Return range progress</returns>
    public virtual RangeProgress<T> CreateRangeProgress(T value)
    {
        return new RangeProgress<T>(rangeValue, value);
    }
    /// <summary>
    /// Calculate inverse interpolated value range (0 - 1)
    /// </summary>
    /// <param name="value">Current value in range (min-max)</param>
    /// <returns>Return inverse interpolated value range (0 - 1)</returns>
    public virtual float CalcInverseLerpValue(T value) => rangeValue.CalcInverseLerpValue(value);
    /// <summary>
    /// Calculate interpolated value range (min - max) base on weight (0 - 1)
    /// </summary>
    /// <param name="weight">Weight factor range (0 - 1)</param>
    /// <returns>Return interpolated value range (min - max)</returns>
    public virtual T CalcInterpolatedValue(float weight) => rangeValue.CalcInterpolatedValue(weight);
    /// <summary>
    /// Check whether value is out of range or not
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>Return true if value out of range otherwise false</returns>
    public virtual bool IsOutOfRange(T value) => rangeValue.IsOutOfRange(value);
}