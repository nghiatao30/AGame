using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "HashtableBundleVariableReference", menuName = "HyrphusQ/VariableReferenceSO/HashtableBundle")]
public class HashtableBundleVariableReference : VariableReference<IBundle>
{
    public override IBundle value
    {
        get
        {
            if (m_RuntimeValue == null)
                m_RuntimeValue = new HashtableBundle();
            return m_RuntimeValue;
        }
        set => base.value = value;
    }

    public bool Contains(string key)
    {
        var bundle = value;
        return bundle.Contains(key);
    }
    public void AddData<T>(string key, T value)
    {
        var bundle = this.value;
        bundle.Add(key, value);
    }
    public void RemoveData(string key)
    {
        var bundle = value;
        bundle.Remove(key);
    }
    public T GetData<T>(string key)
    {
        var bundle = value;
        return bundle.Get<T>(key);
    }
    public bool TryGetData<T>(string key, out T value)
    {
        var bundle = this.value;
        return bundle.TryGet<T>(key, out value);
    }
    public void ClearAll()
    {
        m_RuntimeValue.ClearAll();
        m_RuntimeValue = null;
    }
}