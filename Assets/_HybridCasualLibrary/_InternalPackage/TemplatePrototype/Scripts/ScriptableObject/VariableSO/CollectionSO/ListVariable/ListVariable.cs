using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ListVariable<T> : CollectionVariable<List<T>, T>, IEnumerable<T>
{
    public override event Action<T> onItemAdded;
    public override event Action<T> onItemRemoved;
    public override event Action onItemCleared;
    
    public override int count => value.Count;
    public override List<T> value 
    { 
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return m_InitialValue;
#endif
            if (m_RuntimeValue == null)
                m_RuntimeValue = m_InitialValue == null ? new List<T>() : new List<T>(m_InitialValue);
            return m_RuntimeValue;
        }
        set => base.value = value;
    }

    public virtual IEnumerator GetEnumerator()
    {
        return value.GetEnumerator();
    }
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return value.GetEnumerator();
    }
    public virtual void Add(T item)
    {
        // Add item
        value.Add(item);
        // Raise event add new item
        onItemAdded?.Invoke(item);
    }
    public virtual void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }
    public virtual bool Remove(T item)
    {
        // Remove item
        var isSuccess = value.Remove(item);
        // Raise event remove item if remove success
        if (isSuccess)
            onItemRemoved?.Invoke(item);
        return isSuccess;
    }
    public virtual void RemoveAt(int index)
    {
        if (index <= 0 || index >= value.Count)
            return;
        var item = value[index];
        // Remove item
        value.RemoveAt(index);
        // Raise event remove item
        onItemRemoved?.Invoke(item);
    }
    public virtual void Clear()
    {
        value.Clear();

        // Raise event clear all items
        onItemCleared?.Invoke();
    }
    public virtual bool Contains(T item)
    {
        return value.Contains(item);
    }
    public virtual int IndexOf(T item)
    {
        return value.IndexOf(item);
    }
    public virtual T FirstOrDefault(Predicate<T> predicate)
    {
        return value.FirstOrDefault(predicate.Invoke);
    }
    public virtual T Find(Predicate<T> predicate)
    {
        return value.Find(predicate);
    }
    public virtual T FindLast(Predicate<T> predicate)
    {
        return value.FindLast(predicate);
    }
    public virtual IEnumerable<T> Where(Predicate<T> predicate)
    {
        return value.Where(predicate.Invoke);
    }
    public virtual List<T> GetItemsAlloc()
    {
        return new List<T>(value);
    }
    public virtual T this[int i]
    {
        get => value[i];
        set => this.value[i] = value;
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
        m_RuntimeValue = new List<T>(m_InitialValue);
    }
    public static implicit operator List<T>(ListVariable<T> variable)
    {
        return variable.value;
    }
}