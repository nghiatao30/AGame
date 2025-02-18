using System;
using System.Collections;
using System.Collections.Generic;
using HyrphusQ.SerializedDataStructure;
using UnityEngine;

public class DictionaryVariable<TKey, TValue> : CollectionVariable<SerializedDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
{
    public override event Action<KeyValuePair<TKey, TValue>> onItemAdded;
    public override event Action<KeyValuePair<TKey, TValue>> onItemRemoved;
    public override event Action onItemCleared;

    public override int count => value.Count;
    public override SerializedDictionary<TKey, TValue> value
    {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return m_InitialValue;
#endif
            if (m_RuntimeValue == null)
                m_RuntimeValue = m_InitialValue == null ? new SerializedDictionary<TKey, TValue>() : new SerializedDictionary<TKey, TValue>(m_InitialValue);
            return m_RuntimeValue;
        }
        set => base.value = value;
    }

    public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return value.GetEnumerator();
    }
    public virtual void Add(TKey key, TValue value)
    {
        Add(new KeyValuePair<TKey, TValue>(key, value));
    }
    public virtual void Add(KeyValuePair<TKey, TValue> item)
    {
        value.Add(item.Key, item.Value);
        // Raise event add new item
        onItemAdded?.Invoke(item);
    }
    public virtual void Clear()
    {
        value.Clear();
        // Raise event clear all items
        onItemCleared?.Invoke();
    }
    public virtual bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (this.value.TryGetValue(item.Key, out TValue value))
            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        return false;
    }
    public virtual bool ContainsKey(TKey key)
    {
        return value.ContainsKey(key);
    }
    public virtual bool Remove(TKey key)
    {
        return Remove(new KeyValuePair<TKey, TValue>(key, value.Get(key)));
    }
    public virtual bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (value.Remove(item.Key))
        {
            // Raise event remove item
            onItemRemoved?.Invoke(item);
            return true;
        }
        return false;
    }
    public virtual bool TryGetValue(TKey key, out TValue value)
    {
        return this.value.TryGetValue(key, out value);
    }
    public virtual TValue Get(TKey key)
    {
        return this[key];
    }
    public virtual void Set(TKey key, TValue value)
    {
        this[key] = value;
    }
    public virtual TValue this[TKey key]
    {
        get => value.Get(key);
        set
        {
            if (!ContainsKey(key))
                Add(key, value);
            else
                this.value[key] = value;
        }
    }
    public override void OnAfterDeserialize()
    {
        // Do nothing
    }
    public override void OnBeforeSerialize()
    {
        if (m_InitialValue == null)
            return;
        // Assign serialized value for runtime value because runtime value is not serialized.
        // *Note: For reference type shallow copy collection instead of assign
        m_RuntimeValue = new SerializedDictionary<TKey, TValue>(m_InitialValue);
    }
    public static implicit operator Dictionary<TKey, TValue>(DictionaryVariable<TKey, TValue> variable) => variable.value;
}