using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObjectVariable", menuName = "HyrphusQ/VariableSO/ScriptableObject")]
public class ScriptableObjectVariable : Variable<ScriptableObject>
{
    public T Get<T>() where T : ScriptableObject
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

    public void Set<T>(T value) where T : ScriptableObject
    {
        m_RuntimeValue = value;
    }
}