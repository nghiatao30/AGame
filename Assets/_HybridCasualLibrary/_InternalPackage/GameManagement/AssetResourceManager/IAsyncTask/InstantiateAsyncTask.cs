using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_2022_3_OR_NEWER
public class InstantiateAsyncTask<T> : IAsyncTask<T[]> where T : UnityEngine.Object
{
    public InstantiateAsyncTask(AsyncInstantiateOperation<T> asyncInstantiateOperation)
    {
        if (asyncInstantiateOperation == null)
            return;
        this.asyncInstantiateOperation = asyncInstantiateOperation;
        this.asyncInstantiateOperation.completed += OnInstantiateCompleted;

        void OnInstantiateCompleted(AsyncOperation asyncOperation)
        {
            _onCompleted?.Invoke();
        }
    }

    private event Action _onCompleted;
    public event Action onCompleted
    {
        add
        {
            if (isCompleted)
                value?.Invoke();
            _onCompleted += value;
        }
        remove
        {
            _onCompleted -= value;
        }
    }

    public bool isCompleted => asyncInstantiateOperation.isDone;
    public float percentageComplete => asyncInstantiateOperation.progress;
    public T[] result => asyncInstantiateOperation.Result;
    public AsyncInstantiateOperation<T> asyncInstantiateOperation { get; protected set; }
}
#endif