using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenericVariableReference", menuName = "HyrphusQ/VariableReferenceSO/Generic")]
public class GenericVariableReference : VariableReference<object>
{
    public T Get<T>()
    {
        try
        {
            return (T)m_RuntimeValue;
        }
        catch (System.Exception exc)
        {
            Debug.LogException(exc);
            return default(T);
        }
    }
    public void Set<T>(T value)
    {
        m_RuntimeValue = value;
    }
}