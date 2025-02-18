using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variable<T> : VariableReference<T>, ISerializationCallbackReceiver
{
    [SerializeField]
    protected T m_InitialValue;
    public T initialValue => m_InitialValue;

    public virtual void ResetValue()
    {
        m_RuntimeValue = m_InitialValue;
    }

    public virtual void OnBeforeSerialize()
    {
        // Do Nothing
    }

    public virtual void OnAfterDeserialize()
    {
        // Assign serialized value for runtime value because runtime value is not serialized.
        m_RuntimeValue = m_InitialValue;
    }
    
    public static implicit operator T(Variable<T> variable)
    {
        return variable.value;
    }
}