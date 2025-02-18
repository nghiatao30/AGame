using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ADDRESSABLES && UNITY_ADDRESSABLES_MODE
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif

#if !UNITY_ADDRESSABLES || !UNITY_ADDRESSABLES_MODE
public class LoadSceneAsyncTask : IAsyncTask<AsyncOperation>
{
    public LoadSceneAsyncTask (AsyncOperation asyncOperation)
    {
        if (asyncOperation == null)
            return;
        this.asyncOperation = asyncOperation;
        this.asyncOperation.completed += OnSceneLoadCompleted;

        void OnSceneLoadCompleted(AsyncOperation asyncOperation)
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

    public bool isCompleted => asyncOperation.isDone;
    public float percentageComplete => asyncOperation.progress;
    public AsyncOperation result => asyncOperation;
    public AsyncOperation asyncOperation { get; protected set; }
}
#else
public class LoadSceneAsyncTask : IAsyncTask<SceneInstance>
{
    public LoadSceneAsyncTask(AsyncOperationHandle<SceneInstance> operationHandle)
    {
        m_LoadSceneAsyncOperation = operationHandle;
        m_LoadSceneAsyncOperation.Completed += OnSceneLoadCompleted;

        void OnSceneLoadCompleted(AsyncOperationHandle<SceneInstance> _)
        {
            _onCompleted?.Invoke();
        }
    }

    private event Action _onCompleted;
    private AsyncOperationHandle<SceneInstance> m_LoadSceneAsyncOperation;

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
    public bool isCompleted => m_LoadSceneAsyncOperation.IsDone;
    public float percentageComplete => m_LoadSceneAsyncOperation.PercentComplete;
    public SceneInstance result => m_LoadSceneAsyncOperation.Result;
    public AsyncOperationHandle<SceneInstance> asyncOperation => m_LoadSceneAsyncOperation;
}
#endif