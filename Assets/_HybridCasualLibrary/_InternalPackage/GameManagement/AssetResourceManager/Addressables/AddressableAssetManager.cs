using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HyrphusQ.Events;
using UnityEngine;
#if UNITY_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

public enum AddressableAssetManagementEventCode
{
    OnStartLoadAssetResources,
    OnEndLoadAssetResources,
    OnStartUnloadAssetResources,
    OnEndUnloadAssetResources,
}
[OptionalDependency("UnityEngine.AddressableAssets.Addressables", "UNITY_ADDRESSABLES")]
public class AddressableAssetManager : Singleton<AddressableAssetManager>
{
#if UNITY_ADDRESSABLES
    private const string k_DebugTag = nameof(AddressableAssetManager);
    private Dictionary<string, AsyncOperationHandle> m_LoadedAssetReferenceDictionary = new Dictionary<string, AsyncOperationHandle>();
    private Dictionary<IAddressableAssetResource, int> m_AddressableAssetResourceDictionary = new Dictionary<IAddressableAssetResource, int>();

    private void NotifyEventStartLoadAssetResources()
    {
        GameEventHandler.Invoke(AddressableAssetManagementEventCode.OnStartLoadAssetResources);
    }

    private void NotifyEventEndLoadAssetResources()
    {
        GameEventHandler.Invoke(AddressableAssetManagementEventCode.OnEndLoadAssetResources);
    }

    private void NotifyEventStartUnloadAssetResources()
    {
        GameEventHandler.Invoke(AddressableAssetManagementEventCode.OnStartUnloadAssetResources);
    }

    private void NotifyEventEndUnloadAssetResources()
    {
        GameEventHandler.Invoke(AddressableAssetManagementEventCode.OnEndUnloadAssetResources);
    }

    public void AddManipulateAssetResources(IAddressableAssetResource assetResource, int sceneFlags)
    {
        if (!m_AddressableAssetResourceDictionary.ContainsKey(assetResource))
        {
            m_AddressableAssetResourceDictionary.Add(assetResource, sceneFlags);
        }
        else
        {
            m_AddressableAssetResourceDictionary.Set(assetResource, m_AddressableAssetResourceDictionary.Get(assetResource) | sceneFlags);
        }
    }

    public void RemoveManipulateAssetResources(IAddressableAssetResource assetResource)
    {
        if (m_AddressableAssetResourceDictionary.ContainsKey(assetResource))
        {
            m_AddressableAssetResourceDictionary.Remove(assetResource);
        }
    }

    public IAssetAsyncTask<T[]> GetOrLoadAssetResources<T>(IAddressableAssetResource assetResource, Action<IAssetAsyncTask<T[]>> callback = null, bool autoChangeState = true) where T : UnityEngine.Object
    {
        if (autoChangeState)
            assetResource.stateFlags = assetResource.stateFlags == StateFlags.Unloaded ? StateFlags.NotUsed : assetResource.stateFlags;
        return GetOrLoadAssetResources(assetResource.CollectResources(), callback);
    }

    public IAssetAsyncTask<IAssetAsyncTask<T[]>[]> GetOrLoadAssetResources<T>(IAddressableAssetResource[] assetResources, Action<IAssetAsyncTask<IAssetAsyncTask<T[]>[]>> callback = null, bool autoChangeState = true) where T : UnityEngine.Object
    {
        var loadingAsyncTasks = new IAssetAsyncTask<T[]>[assetResources.Length];
        for (int i = 0; i < assetResources.Length; i++)
        {
            loadingAsyncTasks[i] = GetOrLoadAssetResources<T>(assetResources[i], null, autoChangeState);
        }
        return new CompositeAssetAsyncTask<T>(loadingAsyncTasks, OnOperationCompleted);

        void OnOperationCompleted(IAssetAsyncTask<IAssetAsyncTask<T[]>[]> asyncTask)
        {
            callback?.Invoke(asyncTask);
        }
    }

    public IAssetAsyncTask<T> GetOrLoadAssetResources<T>(AssetReference assetReference, Action<IAssetAsyncTask<T>> callback = null) where T : UnityEngine.Object
    {
        AsyncOperationHandle<UnityEngine.Object> operationHandle;
        if (m_LoadedAssetReferenceDictionary.TryGetValue(assetReference.AssetGUID, out AsyncOperationHandle typelessOperationHandle))
        {
            operationHandle = typelessOperationHandle.Convert<UnityEngine.Object>();
        }
        else
        {
            operationHandle = Addressables.LoadAssetAsync<UnityEngine.Object>(assetReference.RuntimeKey);
            m_LoadedAssetReferenceDictionary.Add(assetReference.AssetGUID, operationHandle);
            if (m_Verbose)
            {
#if UNITY_EDITOR
                LGDebug.Log($"Load asset - {assetReference.editorAsset}", k_DebugTag);
#else
                LGDebug.Log($"Load asset - {assetReference.AssetGUID}", k_DebugTag);

#endif
            }
        }
        return new LoadAssetAsyncTask<T>(operationHandle, OnAssetLoadFailed, OnAssetLoadCompleted);

        void OnAssetLoadFailed(AsyncOperationHandle<UnityEngine.Object> failedOperationHandle)
        {
            m_LoadedAssetReferenceDictionary.Remove(assetReference.AssetGUID);
            LGDebug.LogError($"Load asset failed", k_DebugTag);
            LGDebug.LogException(failedOperationHandle.OperationException);
            Addressables.Release(failedOperationHandle);
        }
        void OnAssetLoadCompleted(IAssetAsyncTask<T> asyncTask)
        {
            callback?.Invoke(asyncTask);
        }
    }

    public IAssetAsyncTask<T[]> GetOrLoadAssetResources<T>(AssetReference[] assetReferences, Action<IAssetAsyncTask<T[]>> callback = null) where T : UnityEngine.Object
    {
        if (assetReferences == null || assetReferences.Length <= 0)
            return null;
        var operationHandles = new AsyncOperationHandle<UnityEngine.Object>[assetReferences.Length];
        for (int i = 0; i < assetReferences.Length; i++)
        {
            AssetReference assetReference = assetReferences[i];
            AsyncOperationHandle<UnityEngine.Object> operationHandle;
            if (m_LoadedAssetReferenceDictionary.TryGetValue(assetReference.AssetGUID, out AsyncOperationHandle typelessOperationHandle))
            {
                operationHandle = typelessOperationHandle.Convert<UnityEngine.Object>();
            }
            else
            {
                operationHandle = Addressables.LoadAssetAsync<UnityEngine.Object>(assetReference.RuntimeKey);
                m_LoadedAssetReferenceDictionary.Add(assetReference.AssetGUID, operationHandle);
                if (m_Verbose)
                {
#if UNITY_EDITOR
                    LGDebug.Log($"Load asset - {assetReference.editorAsset}", k_DebugTag);
#else
                    LGDebug.Log($"Load asset - {assetReference.AssetGUID}", k_DebugTag);
#endif
                }
            }
            operationHandles[i] = operationHandle;
        }
        return new LoadMultiAssetAsyncTask<T>(operationHandles, OnAssetLoadFailed, OnAssetLoadCompleted);

        void OnAssetLoadFailed(AsyncOperationHandle<UnityEngine.Object> failedOperationHandle)
        {
            var index = Array.IndexOf(operationHandles, failedOperationHandle);
            m_LoadedAssetReferenceDictionary.Remove(assetReferences[index].AssetGUID);
            LGDebug.LogError($"Load asset failed", k_DebugTag);
            LGDebug.LogException(failedOperationHandle.OperationException);
            Addressables.Release(failedOperationHandle);
        }
        void OnAssetLoadCompleted(IAssetAsyncTask<T[]> asyncTask)
        {
            callback?.Invoke(asyncTask);
        }
    }

    public void LoadAllAssetResourcesByScene(int sceneIndex)
    {
        NotifyEventStartLoadAssetResources();
        foreach (var keyValuePair in m_AddressableAssetResourceDictionary)
        {
            var sceneFlags = keyValuePair.Value;
            if ((sceneFlags & sceneIndex) != sceneIndex)
                continue;
            var assetResource = keyValuePair.Key;
            if (assetResource.stateFlags != StateFlags.Unloaded)
                continue;
            GetOrLoadAssetResources<UnityEngine.Object>(assetResource, null);
        }
        NotifyEventEndLoadAssetResources();
    }

    public void LoadAllAssetResources()
    {
        LoadAllAssetResourcesByScene(-1);
    }

    public void ReleaseAssetResources(params string[] assetGUIDs)
    {
        if (assetGUIDs == null || assetGUIDs.Length <= 0)
            return;
        foreach (var unloadedAssetGUID in assetGUIDs)
        {
            if (m_LoadedAssetReferenceDictionary.TryGetValue(unloadedAssetGUID, out AsyncOperationHandle operationHandle))
            {
                if (m_Verbose)
                    LGDebug.Log($"Unload asset - {operationHandle.Result}", k_DebugTag);
                Addressables.Release(operationHandle);
                m_LoadedAssetReferenceDictionary.Remove(unloadedAssetGUID);
            }
        }
    }

    public void ReleaseAssetResources(params AssetReference[] assetReferences)
    {
        if (assetReferences == null || assetReferences.Length <= 0)
            return;
        foreach (var assetReference in assetReferences)
        {
            ReleaseAssetResources(assetReference.AssetGUID);
        }
    }

    public void ReleaseAssetResources(IAddressableAssetResource assetResource)
    {
        if (assetResource == null || assetResource.stateFlags == StateFlags.Unloaded)
            return;
        assetResource.stateFlags = StateFlags.Unloaded;
        assetResource.ReleaseResources();
        ReleaseAssetResources(assetResource.CollectResources());
    }

    public void ReleaseAllUnusedAssetResources()
    {
        NotifyEventStartUnloadAssetResources();
        var inUsedAssetGUIDList = new List<string>();
        var notUsedAssetGUIDList = new List<string>();
        foreach (var keyValuePair in m_AddressableAssetResourceDictionary)
        {
            var assetResource = keyValuePair.Key;
            var assetResourceStateFlags = assetResource.stateFlags;
            var assetReferences = assetResource.CollectResources();
            foreach (var assetReference in assetReferences)
            {
                if (assetResourceStateFlags == StateFlags.NotUsed && !notUsedAssetGUIDList.Contains(assetReference.AssetGUID))
                    notUsedAssetGUIDList.Add(assetReference.AssetGUID);
                else if (assetResourceStateFlags == StateFlags.InUsed && !inUsedAssetGUIDList.Contains(assetReference.AssetGUID))
                {
                    inUsedAssetGUIDList.Add(assetReference.AssetGUID);
                    if (m_Verbose)
                    {
#if UNITY_EDITOR
                        LGDebug.Log($"Keep asset in memory - {assetReference.editorAsset}", k_DebugTag);
#else
                        LGDebug.Log($"Keep asset in memory - {assetReference.AssetGUID}", k_DebugTag);
#endif
                    }
                }
            }
            if (assetResourceStateFlags == StateFlags.NotUsed)
            {
                assetResource.stateFlags = StateFlags.Unloaded;
                assetResource.ReleaseResources();
            }
        }
        notUsedAssetGUIDList.RemoveAll(assetGUID => inUsedAssetGUIDList.Contains(assetGUID));
        ReleaseAssetResources(notUsedAssetGUIDList.ToArray());
        NotifyEventEndUnloadAssetResources();
    }

    public interface IAssetAsyncTask<T> : IAsyncTask<T>
    {
        public AsyncOperationStatus status { get; }
    }

    public class LoadAssetAsyncTask<T> : IAssetAsyncTask<T> where T : UnityEngine.Object
    {
        public LoadAssetAsyncTask(AsyncOperationHandle<UnityEngine.Object> operationHandle, Action<AsyncOperationHandle<UnityEngine.Object>> onOperationFailed, Action<IAssetAsyncTask<T>> onOperationCompleted)
        {
            _onCompleted += () => onOperationCompleted?.Invoke(this);
            m_Status = AsyncOperationStatus.None;
            m_LoadAssetAsyncOperation = operationHandle;
            // Check if operation has already completed and succeeded => invoke callback immediately to prevent delay callback Completed of AsyncOperationBase invoke callback in Update interval
            // Succeeded => Loaded completed
            // None => In loading process
            // Failed => Loaded failed (unexpected behaviour)
            if (m_LoadAssetAsyncOperation.Status == AsyncOperationStatus.Succeeded)
            {
                OnCompleted(m_LoadAssetAsyncOperation);
                return;
            }
            m_LoadAssetAsyncOperation.Completed += OnCompleted;

            void OnCompleted(AsyncOperationHandle<UnityEngine.Object> operationHandle)
            {
                m_Status = operationHandle.Status;
                if (m_Status == AsyncOperationStatus.Failed)
                {
                    onOperationFailed?.Invoke(operationHandle);
                }
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

        private AsyncOperationStatus m_Status;
        private AsyncOperationHandle<UnityEngine.Object> m_LoadAssetAsyncOperation;

        public T result => m_LoadAssetAsyncOperation.IsValid() ? m_LoadAssetAsyncOperation.Result as T : null;
        public bool isCompleted => m_LoadAssetAsyncOperation.IsDone;
        public float percentageComplete => m_LoadAssetAsyncOperation.IsValid() ? m_LoadAssetAsyncOperation.PercentComplete : 0f;
        public AsyncOperationStatus status => m_Status;
    }

    public class LoadMultiAssetAsyncTask<T> : IAssetAsyncTask<T[]> where T : UnityEngine.Object
    {
        public LoadMultiAssetAsyncTask(AsyncOperationHandle<UnityEngine.Object>[] operationHandles, Action<AsyncOperationHandle<UnityEngine.Object>> onOperationFailed, Action<IAssetAsyncTask<T[]>> onOperationCompleted)
        {
            _onCompleted += () => onOperationCompleted?.Invoke(this);
            m_Status = AsyncOperationStatus.None;
            m_NumOfCompletedOperations = 0;
            m_LoadAssetAsyncOperations = operationHandles;
            foreach (var operationHandle in m_LoadAssetAsyncOperations)
            {
                // Check if operation has already completed and succeeded => invoke callback immediately to prevent delay callback Completed of AsyncOperationBase invoke callback in Update interval
                // Succeeded => Loaded completed
                // None => In loading process
                // Failed => Loaded failed (unexpected behaviour)
                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    OnCompleted(operationHandle);
                    continue;
                }
                operationHandle.Completed += OnCompleted;
            }

            void OnCompleted(AsyncOperationHandle<UnityEngine.Object> operationHandle)
            {
                var status = operationHandle.Status;
                if (status == AsyncOperationStatus.Failed)
                {
                    m_Status = AsyncOperationStatus.Failed;
                    onOperationFailed?.Invoke(operationHandle);
                }
                m_NumOfCompletedOperations++;
                if (m_NumOfCompletedOperations >= m_LoadAssetAsyncOperations.Length)
                {
                    if (m_Status == AsyncOperationStatus.None)
                        m_Status = AsyncOperationStatus.Succeeded;
                    _onCompleted?.Invoke();
                }
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

        private AsyncOperationStatus m_Status;
        private int m_NumOfCompletedOperations;
        private AsyncOperationHandle<UnityEngine.Object>[] m_LoadAssetAsyncOperations;

        public T[] result => m_LoadAssetAsyncOperations.Select(operation => operation.IsValid() ? operation.Result as T : null).ToArray();
        public bool isCompleted => m_NumOfCompletedOperations >= m_LoadAssetAsyncOperations.Length;
        public float percentageComplete => m_LoadAssetAsyncOperations.Average(operation => operation.IsValid() ? operation.PercentComplete : 0f);
        public AsyncOperationStatus status => m_Status;
    }

    public class CompositeAssetAsyncTask<T> : IAssetAsyncTask<IAssetAsyncTask<T[]>[]>
    {
        public CompositeAssetAsyncTask(IAssetAsyncTask<T[]>[] asyncTasks, Action<IAssetAsyncTask<IAssetAsyncTask<T[]>[]>> onOperationCompleted)
        {
            _onCompleted += () => onOperationCompleted?.Invoke(this);
            m_Status = AsyncOperationStatus.None;
            m_NumOfCompletedTasks = 0;
            m_LoadAssetAsyncTasks = asyncTasks;
            foreach (var asyncTask in m_LoadAssetAsyncTasks)
            {
                asyncTask.onCompleted += OnCompleted;

                void OnCompleted()
                {
                    var status = asyncTask.status;
                    if (status == AsyncOperationStatus.Failed)
                    {
                        m_Status = AsyncOperationStatus.Failed;
                    }
                    m_NumOfCompletedTasks++;
                    if (m_NumOfCompletedTasks >= m_LoadAssetAsyncTasks.Length)
                    {
                        if (m_Status == AsyncOperationStatus.None)
                            m_Status = AsyncOperationStatus.Succeeded;
                        _onCompleted?.Invoke();
                    }
                }
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

        private AsyncOperationStatus m_Status;
        private int m_NumOfCompletedTasks;
        private IAssetAsyncTask<T[]>[] m_LoadAssetAsyncTasks;

        public IAssetAsyncTask<T[]>[] result => m_LoadAssetAsyncTasks;
        public bool isCompleted => m_NumOfCompletedTasks >= m_LoadAssetAsyncTasks.Length;
        public float percentageComplete => m_LoadAssetAsyncTasks.Average(task => task.percentageComplete);
        public AsyncOperationStatus status => m_Status;
    }
#endif
}