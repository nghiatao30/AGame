using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class ObjectPool<T> : MonoBehaviour, IPooling<T> where T : MonoBehaviour
{   
    [SerializeField] protected int initialNum;
    [SerializeField] protected int prewarmCount = 10;
    protected const string k_NotInitYetExceptionMessage = "Pool has not been initialized yet!";
    protected Queue<T> poolingQueue;
    public bool isInitialized => poolingQueue != null;

    public int count => poolingQueue.Count;

    protected T prefabObject;

    protected Transform anchoredTransform;

    protected Action<T> m_OnReturnToPool;

    protected virtual void Initialize()
    {
        poolingQueue = new Queue<T>();
        for (int i = 0; i < initialNum; i++)
        {
            var item = InstantiateMethod();
            PullIn(item);
        }
    }

    protected virtual T InstantiateMethod()
    {
        var instance = Instantiate(prefabObject, anchoredTransform);
        return instance;
    }
    
    protected virtual void RefillPool()
    {
        for (int i = 0; i < prewarmCount; i++)
        {
            var item = InstantiateMethod();
            PullIn(item);
        }
    }

    public virtual void Clear()
    {
        if (!isInitialized)
            throw new Exception(k_NotInitYetExceptionMessage);
        foreach (var item in poolingQueue)
        {
            Destroy(item);
        }
        poolingQueue.Clear();
    }

    public virtual T PullOut()
    {
        if (!isInitialized)
            throw new Exception(k_NotInitYetExceptionMessage);
        if (poolingQueue.Count <= 0)
            RefillPool();
        var item = poolingQueue.Dequeue();
        return item;
    }

    public virtual void PullIn(T item)
    {
        if (!isInitialized)
            throw new Exception(k_NotInitYetExceptionMessage);
        poolingQueue.Enqueue(item);
    }


}
