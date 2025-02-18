using System;
using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using UnityEngine;
using UnityEditor;

public class VariableReference<T> : ScriptableObject
{
    public event Action<ValueDataChanged<T>> onValueChanged;

    [NonSerialized]
    protected T m_RuntimeValue;
    public virtual T value
    {
        get => m_RuntimeValue;
        set
        {
            var oldValue = m_RuntimeValue;
            m_RuntimeValue = value;
            if (!IsEquals(oldValue, m_RuntimeValue))
                NotifyValueChanged(new ValueDataChanged<T>(oldValue, value));
        }
    }

#if ODIN_INSPECTOR_3 && UNITY_EDITOR
    [Sirenix.OdinInspector.OnInspectorGUI]
    protected virtual void OnInspectorGUI()
    {
        if (value is UnityEngine.Object)
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Current value", value as UnityEngine.Object, typeof(T), false);
            GUI.enabled = true;
        }
        else
            EditorGUILayout.LabelField($"Current value: {(value == null ? "Null" : value)}");
    }
#endif

    /// <summary>
    /// Define override function if generic type is your class without default equality operator
    /// </summary>
    /// <returns>Return true if two value is equal</returns>
    protected virtual bool IsEquals(T valueA, T valueB)
    {
        return EqualityComparer<T>.Default.Equals(valueA, valueB);
    }

    public virtual void NotifyValueChanged(ValueDataChanged<T> valueDataChanged)
    {
        onValueChanged?.Invoke(valueDataChanged);
    }
    
    public static implicit operator T(VariableReference<T> variable)
    {
        return variable.value;
    }
}