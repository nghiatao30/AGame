using System;
using System.Collections.Generic;
using HyrphusQ.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ADDRESSABLES && UNITY_ADDRESSABLES_MODE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif

namespace LatteGames.GameManagement
{
    public class SceneManager : Singleton<SceneManager>
    {
        public enum Status
        {
            Succeeded,
            Failed
        }
        public class LoadSceneRequest
        {
            public bool isPushToStack { get; set; }
            public bool isSetActiveScene { get; set; }
            public LoadSceneMode mode { get; set; }
            public string originSceneName { get; set; }
            public string destinationSceneName { get; set; }
        }
        public class LoadSceneResponse
        {
            public Status status { get; set; }
            public string errorMessage { get; set; }
        }

        private const string k_DebugTag = nameof(SceneManager);
        private static bool s_IsLoadingScene;
        private static Stack<LoadSceneRequest> s_LoadSceneRequestStack = new Stack<LoadSceneRequest>();
        private static Dictionary<LoadSceneRequest, LoadSceneAsyncTask> s_LoadSceneAsyncTaskDictionary = new Dictionary<LoadSceneRequest, LoadSceneAsyncTask>();

        private static void PreLoadScene(LoadSceneRequest request)
        {
            if (Instance.m_Verbose)
                LGDebug.Log($"StartLoadScene {request.originSceneName} -> {request.destinationSceneName}", k_DebugTag);
            s_IsLoadingScene = true;
            if (request.isPushToStack)
            {
                s_LoadSceneRequestStack.Push(request);
            }
            GameEventHandler.Invoke(SceneManagementEventCode.OnStartLoadScene, request.destinationSceneName, request.originSceneName, request);
        }

        private static void PostLoadScene(LoadSceneRequest request, LoadSceneAsyncTask task)
        {
            if (Instance.m_Verbose)
                LGDebug.Log($"LoadSceneStarted {request.originSceneName} -> {request.destinationSceneName}", k_DebugTag);
            if (task != null)
            {
                s_LoadSceneAsyncTaskDictionary.Add(request, task);
            }
            GameEventHandler.Invoke(SceneManagementEventCode.OnLoadSceneStarted, request.destinationSceneName, request.originSceneName, request, task);
        }

        private static void PreUnloadScene(LoadSceneRequest request)
        {
            if (Instance.m_Verbose)
                LGDebug.Log($"StartUnloadScene {request.destinationSceneName}", k_DebugTag);
            GameEventHandler.Invoke(SceneManagementEventCode.OnStartUnloadScene, request.destinationSceneName, request);
        }

        private static void PostUnloadScene(LoadSceneRequest request, UnloadSceneAsyncTask task)
        {
            if (Instance.m_Verbose)
                LGDebug.Log($"UnloadSceneStarted {request.destinationSceneName}", k_DebugTag);
            GameEventHandler.Invoke(SceneManagementEventCode.OnUnloadSceneStarted, request.destinationSceneName, request, task);
        }

        private static void NotifyEventLoadSceneCompleted(LoadSceneRequest request, Action<LoadSceneResponse> callback)
        {
            if (Instance.m_Verbose)
                LGDebug.Log($"LoadSceneCompleted {request.destinationSceneName}", k_DebugTag);
            s_IsLoadingScene = false;
            callback?.Invoke(new LoadSceneResponse()
            {
                status = Status.Succeeded,
                errorMessage = string.Empty,
            });
            GameEventHandler.Invoke(SceneManagementEventCode.OnLoadSceneCompleted, request.destinationSceneName, request.originSceneName, request);
        }

        private static void NotifyEventUnloadSceneCompleted(LoadSceneRequest request, Action callback)
        {
            if (Instance.m_Verbose)
                LGDebug.Log($"UnloadSceneCompleted {request.destinationSceneName}", k_DebugTag);
            callback?.Invoke();
            GameEventHandler.Invoke(SceneManagementEventCode.OnUnloadSceneCompleted, request.destinationSceneName, request);
        }

        private static void SetActiveSceneInternal(string sceneName)
        {
            var targetScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
            if (targetScene.isLoaded)
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(targetScene);
        }

        private static void OnLoadSceneCompleted(LoadSceneRequest request, Action<LoadSceneResponse> callback = null)
        {
            if (request.mode == LoadSceneMode.Additive && request.isSetActiveScene)
                SetActiveSceneInternal(request.destinationSceneName);
            NotifyEventLoadSceneCompleted(request, callback);
        }

        private static void LoadSceneInternal(LoadSceneRequest request, Action<LoadSceneResponse> callback = null)
        {
#if !UNITY_ADDRESSABLES || !UNITY_ADDRESSABLES_MODE
            UnityEngine.SceneManagement.SceneManager.LoadScene(request.destinationSceneName, request.mode);
            Instance.StartCoroutine(CommonCoroutine.Wait(null, OnLoadCompleted));

            void OnLoadCompleted()
            {
                OnLoadSceneCompleted(request, callback);
            }
#endif
        }

        private static LoadSceneAsyncTask LoadSceneAsyncInternal(LoadSceneRequest request, Action<LoadSceneResponse> callback = null)
        {
#if !UNITY_ADDRESSABLES || !UNITY_ADDRESSABLES_MODE
            var asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(request.destinationSceneName, request.mode);
            asyncOperation.allowSceneActivation = true;
            asyncOperation.priority = 100;
            asyncOperation.completed += OnLoadCompleted;

            void OnLoadCompleted(AsyncOperation _)
            {
                OnLoadSceneCompleted(request, callback);
            }
#else
            var asyncOperation = Addressables.LoadSceneAsync(AddressablesUtility.FindAssetKeyBySceneName(request.destinationSceneName), request.mode, true);
            asyncOperation.Completed += OnLoadCompleted;

            void OnLoadCompleted(AsyncOperationHandle<SceneInstance> _)
            {
                OnLoadSceneCompleted(request, callback);
            }
#endif
            return new LoadSceneAsyncTask(asyncOperation);
        }

        private static UnloadSceneAsyncTask UnloadSceneAsyncInternal(LoadSceneRequest request, Action callback = null)
        {
#if !UNITY_ADDRESSABLES || !UNITY_ADDRESSABLES_MODE
            var asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(request.destinationSceneName);
            asyncOperation.completed += OnUnloadSceneCompleted;

            void OnUnloadSceneCompleted(AsyncOperation _)
            {
                Resources.UnloadUnusedAssets();
                SetActiveSceneInternal(request.originSceneName);
                NotifyEventUnloadSceneCompleted(request, callback);
            }
#else
            var asyncOperation = Addressables.UnloadSceneAsync(s_LoadSceneAsyncTaskDictionary.Get(request).asyncOperation);
            asyncOperation.Completed += OnUnloadSceneCompleted;

            void OnUnloadSceneCompleted(AsyncOperationHandle<SceneInstance> _)
            {
                SetActiveSceneInternal(request.originSceneName);
                NotifyEventUnloadSceneCompleted(request, callback);
            }
#endif
            return new UnloadSceneAsyncTask(asyncOperation);
        }

        private static bool ValidateRequest(LoadSceneRequest request, Action<LoadSceneResponse> callback)
        {
            if (s_IsLoadingScene)
            {
                if (Instance.m_Verbose)
                    LGDebug.Log($"Failed to load scene because another scene is being loaded in-progress");
                callback?.Invoke(new LoadSceneResponse()
                {
                    status = Status.Failed,
                    errorMessage = "Another scene is being loaded in-progress"
                });
                return false;
            }
            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(request.destinationSceneName).isLoaded)
            {
                if (Instance.m_Verbose)
                    LGDebug.Log($"Failed to load scene because this scene ({request.destinationSceneName}) has already been loaded");
                callback?.Invoke(new LoadSceneResponse()
                {
                    status = Status.Failed,
                    errorMessage = $"This scene ({request.destinationSceneName}) has already been loaded"
                });
                return false;
            }
            return true;
        }

        public static LoadSceneRequest CreateLoadSceneRequest(string destinationSceneName, LoadSceneMode mode = LoadSceneMode.Single, bool isPushToStack = true, bool isSetActiveScene = true)
        {
            return new LoadSceneRequest()
            {
                mode = mode,
                isPushToStack = isPushToStack,
                isSetActiveScene = isSetActiveScene,
                destinationSceneName = destinationSceneName,
                originSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            };
        }

        public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool isPushToStack = true, bool isSetActiveScene = true, Action<LoadSceneResponse> callback = null)
        {
            LoadScene(CreateLoadSceneRequest(sceneName, mode, isPushToStack, isSetActiveScene), callback);
        }

        public static void LoadScene(SceneName sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool isPushToStack = true, bool isSetActiveScene = true, Action<LoadSceneResponse> callback = null)
        {
            LoadScene(CreateLoadSceneRequest(sceneName.ToString(), mode, isPushToStack, isSetActiveScene), callback);
        }

        public static void LoadScene(LoadSceneRequest request, Action<LoadSceneResponse> callback = null)
        {
            if (!ValidateRequest(request, callback))
                return;
            PreLoadScene(request);
#if !UNITY_ADDRESSABLES || !UNITY_ADDRESSABLES_MODE
            LoadSceneInternal(request, callback);
            PostLoadScene(request, null);
#else
            var loadSceneAsyncTask = LoadSceneAsyncInternal(request, callback);
            PostLoadScene(request, loadSceneAsyncTask);
#endif
        }

        public static LoadSceneAsyncTask LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool isPushToStack = true, bool isSetActiveScene = true, Action<LoadSceneResponse> callback = null)
        {
            return LoadSceneAsync(CreateLoadSceneRequest(sceneName, mode, isPushToStack, isSetActiveScene), callback);
        }

        public static LoadSceneAsyncTask LoadSceneAsync(LoadSceneRequest request, Action<LoadSceneResponse> callback = null)
        {
            if (!ValidateRequest(request, callback))
                return null;
            PreLoadScene(request);
            var loadSceneAsyncTask = LoadSceneAsyncInternal(request, callback);
            PostLoadScene(request, loadSceneAsyncTask);
            return loadSceneAsyncTask;
        }

        public static UnloadSceneAsyncTask UnloadSceneAsync(LoadSceneRequest request, Action callback = null)
        {
            PreUnloadScene(request);
            var unloadSceneAsyncTask = UnloadSceneAsyncInternal(request, callback);
            PostUnloadScene(request, unloadSceneAsyncTask);
            return unloadSceneAsyncTask;
        }

        public static void BackToPreviousScene(Action callback = null)
        {
            if (s_LoadSceneRequestStack.Count <= 0)
                return;
            var request = s_LoadSceneRequestStack.Pop();
            if (request == null)
                return;
            if (request.mode == LoadSceneMode.Single)
            {
                LoadScene(request.originSceneName, isPushToStack: false, callback: _ => callback?.Invoke());
            }
            else
            {
                UnloadSceneAsync(request, callback);
            }
            s_LoadSceneAsyncTaskDictionary.Remove(request);
        }

        public static IAsyncTask BackToPreviousSceneAsync(Action callback = null)
        {
            if (s_LoadSceneRequestStack.Count <= 0)
                return null;
            var request = s_LoadSceneRequestStack.Pop();
            if (request == null)
                return null;
            IAsyncTask asyncTask = request.mode == LoadSceneMode.Single ? LoadSceneAsync(request.originSceneName, isPushToStack: false, callback: _ => callback?.Invoke()) : UnloadSceneAsync(request, callback);
            s_LoadSceneAsyncTaskDictionary.Remove(request);
            return asyncTask;
        }

        public static Scene GetActiveScene()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        }
    }
}