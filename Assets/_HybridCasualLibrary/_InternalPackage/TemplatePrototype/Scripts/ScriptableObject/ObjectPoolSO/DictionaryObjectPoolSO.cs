using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DictionaryObjectPoolSO<TKey, TValue> : ScriptableObject, IDictionaryPool<TKey, TValue>
{
    protected const string k_NotInitYetExceptionMessage = "Pool has not been initialized yet!";

    [SerializeField]
    protected int m_PrewarmCount = 10;

    protected Action<TValue> m_OnCreatePoolingItem;
    protected Action<TValue> m_OnTakeFromPool;
    protected Action<TValue> m_OnReturnToPool;
    protected Action<TValue> m_OnDestroyPoolingItem;
    protected Transform m_AnchoredTransform;
    protected Dictionary<TKey, Queue<TValue>> m_PoolDictionary;

    public bool isInitialized => m_PoolDictionary != null;
    public int dictionaryCount => m_PoolDictionary.Count;
    public int countAll => m_PoolDictionary.Sum(keyValuePair => keyValuePair.Value.Count);

    /// <summary>
    /// Initialized a pool, need to initialzed a pool before use it.
    /// </summary>
    /// <param name="anchoredTransform">An anchored transform (parent) for all item</param>
    /// <param name="onCreatePoolingItem">Create an item callback</param>
    /// <param name="onTakeFromPool">Take an item from pool callback</param>
    /// <param name="onReturnToPool">Return an item to pool callback</param>
    /// <param name="onDestroyPoolingItem">Destroy an item callback</param>
    public virtual void InitializePool(Transform anchoredTransform, Action<TValue> onCreatePoolingItem = null, Action<TValue> onTakeFromPool = null, Action<TValue> onReturnToPool = null, Action<TValue> onDestroyPoolingItem = null)
    {
        if (isInitialized)
            return;
        m_PoolDictionary = new Dictionary<TKey, Queue<TValue>>();
        m_AnchoredTransform = anchoredTransform;
        m_OnCreatePoolingItem = onCreatePoolingItem;
        m_OnTakeFromPool = onTakeFromPool;
        m_OnReturnToPool = onReturnToPool;
        m_OnDestroyPoolingItem = onDestroyPoolingItem;
        RefillAllPool();
    }
    /// <summary>
    /// Destroy(release) all items(resources) in pool
    /// </summary>
    public void Clear()
    {
        if (!isInitialized)
            throw new Exception(k_NotInitYetExceptionMessage);
        foreach (var keyValuePair in m_PoolDictionary)
        {
            var pool = keyValuePair.Value;
            foreach (var item in pool)
            {
                m_OnDestroyPoolingItem?.Invoke(item);
                DestroyMethod(keyValuePair.Key, item);
            }
            pool.Clear();
        }
        m_PoolDictionary.Clear();
    }
    /// <summary>
    /// Destroy(release) all items(resources) in pool of specific key
    /// </summary>
    /// <param name="key">Key of pool</param>
    public void Clear(TKey key)
    {
        if (!isInitialized)
            throw new Exception(k_NotInitYetExceptionMessage);
        if (m_PoolDictionary.TryGetValue(key, out Queue<TValue> pool))
        {
            foreach (var item in pool)
            {
                m_OnDestroyPoolingItem?.Invoke(item);
                DestroyMethod(key, item);
            }
            pool.Clear();
            m_PoolDictionary.Remove(key);
        }
    }
    /// <summary>
    /// Take an item from the pool of specific key
    /// </summary>
    /// <param name="key">Key of pool</param>
    /// <returns>Return an item from pool</returns>
    public TValue Get(TKey key)
    {
        if (!isInitialized)
            throw new Exception(k_NotInitYetExceptionMessage);
        if (!m_PoolDictionary.TryGetValue(key, out Queue<TValue> pool) || pool.Count <= Const.IntValue.Zero)
        {
            RefillPool(key);
        }
        var item = m_PoolDictionary[key].Dequeue();
        m_OnTakeFromPool?.Invoke(item);
        return item;
    }
    /// <summary>
    /// Return an item to pool of specific
    /// </summary>
    /// <param name="key">Key of pool</param>
    /// <param name="item">Item to return</param>
    public void Release(TKey key, TValue item)
    {
        if (!isInitialized)
            throw new Exception(k_NotInitYetExceptionMessage);
        if (!m_PoolDictionary.ContainsKey(key))
            m_PoolDictionary.Add(key, new Queue<TValue>());
        m_OnReturnToPool?.Invoke(item);
        var poolingQueue = m_PoolDictionary[key];
        poolingQueue.Enqueue(item);
    }
    /// <summary>
    /// Get total count of objects(items) in the pool of specific key
    /// </summary>
    /// <param name="key">Key of pool</param>
    /// <returns>Return total count of items</returns>
    public int Count(TKey key)
    {
        if (!isInitialized)
            throw new Exception(k_NotInitYetExceptionMessage);
        if (m_PoolDictionary.TryGetValue(key, out Queue<TValue> pool))
        {
            return pool.Count;
        }
        return 0;
    }

    /// <summary>
    /// Define a method to prewarm(fill) items into specificed pool when initialized a pool
    /// </summary>
    /// <param name="key">Key of pool</param>
    protected virtual void RefillPool(TKey key)
    {
        for (int i = 0; i < m_PrewarmCount; i++)
        {
            var item = InstantiateMethod(key);
            m_OnCreatePoolingItem?.Invoke(item);
            Release(key, item);
        }
    }
    /// <summary>
    /// Define a method to prewarm(fill) items into pool when initialized a pool
    /// </summary>
    protected abstract void RefillAllPool();
    /// <summary>
    /// Define a method to instantiate an object(item)
    /// </summary>
    /// <param name="key">Key of pool</param>
    /// <returns>Return a object(item)</returns>
    protected abstract TValue InstantiateMethod(TKey key);
    /// <summary>
    /// Define a method to destroy an object or release some resources
    /// </summary>
    /// <param name="key">Key of pool</param>
    /// <param name="item">Object(item) to destroy(release)</param>
    protected abstract void DestroyMethod(TKey key, TValue item);
}