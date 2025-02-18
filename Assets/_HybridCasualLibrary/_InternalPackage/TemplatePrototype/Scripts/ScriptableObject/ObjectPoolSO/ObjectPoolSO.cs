using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPoolSO<T> : ScriptableObject, IPool<T> where T : UnityEngine.Object
{
    protected const string k_NotInitYetExceptionMessage = "Pool has not been initialized yet!";

    [SerializeField]
    protected int m_PrewarmCount = 10;
    [SerializeField]
    protected T m_PrefabObject;

    protected Action<T> m_OnCreatePoolingItem;
    protected Action<T> m_OnTakeFromPool;
    protected Action<T> m_OnReturnToPool;
    protected Action<T> m_OnDestroyPoolingItem;
    protected Transform m_AnchoredTransform;
    protected Queue<T> m_PoolingQueue;

    public bool isInitialized => m_PoolingQueue != null;
    public int count => m_PoolingQueue.Count;

    /// <summary>
    /// Define a method to prewarm(fill) items into pool when initialized a pool
    /// </summary>
    protected virtual void RefillPool()
    {
        for (int i = 0; i < m_PrewarmCount; i++)
        {
            var item = InstantiateMethod();
            m_OnCreatePoolingItem?.Invoke(item);
            Release(item);
        }
    }
    /// <summary>
    /// Define a method to instantiate an object(item)
    /// </summary>
    /// <returns>Return a object(item)</returns>
    protected virtual T InstantiateMethod()
    {
        var instance = Instantiate(m_PrefabObject, m_AnchoredTransform);
        return instance;
    }
    /// <summary>
    /// Define a method to destroy an object or release some resource
    /// </summary>
    /// <param name="item">Object(item) to destroy(release)</param>
    protected virtual void DestroyMethod(T item)
    {
        Destroy(item);
    }

    /// <summary>
    /// Initialized a pool, need to initialzed a pool before use it.
    /// </summary>
    /// <param name="anchoredTransform">An anchored transform (parent) for all item</param>
    /// <param name="onCreatePoolingItem">Create an item callback</param>
    /// <param name="onTakeFromPool">Take an item from pool callback</param>
    /// <param name="onReturnToPool">Return an item to pool callback</param>
    /// <param name="onDestroyPoolingItem">Destroy an item callback</param>
    public virtual void InitializePool(Transform anchoredTransform, Action<T> onCreatePoolingItem = null, Action<T> onTakeFromPool = null, Action<T> onReturnToPool = null, Action<T> onDestroyPoolingItem = null)
    {
        if (isInitialized)
            return;
        m_PoolingQueue = new Queue<T>();
        m_AnchoredTransform = anchoredTransform;
        m_OnCreatePoolingItem = onCreatePoolingItem;
        m_OnTakeFromPool = onTakeFromPool;
        m_OnReturnToPool = onReturnToPool;
        m_OnDestroyPoolingItem = onDestroyPoolingItem;
        RefillPool();
    }
    /// <summary>
    /// Destroy(release) all items(resources) in pool
    /// </summary>
    public virtual void Clear()
    {
        if (!isInitialized)
            throw new Exception(k_NotInitYetExceptionMessage);
        foreach (var item in m_PoolingQueue)
        {
            m_OnDestroyPoolingItem?.Invoke(item);
            DestroyMethod(item);
        }
        m_PoolingQueue.Clear();
    }
    /// <summary>
    /// Take an item from pool
    /// </summary>
    /// <returns>Return an item from pool</returns>
    public virtual T Get()
    {
        if (!isInitialized)
            throw new Exception(k_NotInitYetExceptionMessage);
        if (m_PoolingQueue.Count <= 0)
            RefillPool();
        var item = m_PoolingQueue.Dequeue();
        m_OnTakeFromPool?.Invoke(item);
        return item;
    }
    /// <summary>
    /// Return an item to pool
    /// </summary>
    /// <param name="item">Item to return</param>
    public virtual void Release(T item)
    {
        if (!isInitialized)
            throw new Exception(k_NotInitYetExceptionMessage);
        m_PoolingQueue.Enqueue(item);
        m_OnReturnToPool?.Invoke(item);
    }
}