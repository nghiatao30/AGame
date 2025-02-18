using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADDRESSABLES && UNITY_ADDRESSABLES_MODE
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif

#if !UNITY_ADDRESSABLES || !UNITY_ADDRESSABLES_MODE
public class UnloadSceneAsyncTask : IAsyncTask<AsyncOperation>
{
    public UnloadSceneAsyncTask(AsyncOperation asyncOperation)
    {
        if (asyncOperation == null)
            return;
        this.asyncOperation = asyncOperation;
        this.asyncOperation.completed += OnSceneUnloadCompleted;

        void OnSceneUnloadCompleted(AsyncOperation asyncOperation)
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
public class UnloadSceneAsyncTask : IAsyncTask<SceneInstance>
{
    public UnloadSceneAsyncTask(AsyncOperationHandle<SceneInstance> operationHandle)
    {
        m_UnloadSceneAsyncOperation = operationHandle;
        m_UnloadSceneAsyncOperation.Completed += OnSceneUnloadCompleted;

        void OnSceneUnloadCompleted(AsyncOperationHandle<SceneInstance> _)
        {
            _onCompleted?.Invoke();
        }
    }

    private event Action _onCompleted;
    private AsyncOperationHandle<SceneInstance> m_UnloadSceneAsyncOperation;

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
    public bool isCompleted => m_UnloadSceneAsyncOperation.IsDone;
    public float percentageComplete => m_UnloadSceneAsyncOperation.PercentComplete;
    public SceneInstance result => m_UnloadSceneAsyncOperation.Result;
    public AsyncOperationHandle<SceneInstance> asyncOperation => m_UnloadSceneAsyncOperation;
}
#endif