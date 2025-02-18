using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADDRESSABLES && UNITY_ADDRESSABLES_MODE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace LatteGames.GameManagement
{
    /// <summary>
    /// Game management at run-time.
    /// It's automatically spawned at run-time. It acts like the manager holder which holds all Managers using games such as (CurrencyManager, SceneManager, SoundManager, HapticManager, etc.)
    /// There are two versions (it will automatically switch versions depending on whether the Project is installed Addressables or not)
    ///  + Resources version : It only works when the Project doesn't use Addressables (because we shouldn't use Resources folder with Addressables, used with caution)
    ///  + Addressables version
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        private const string k_DebugTag = nameof(GameManager);
        private const string k_GameManagerName = "GameManager";
        private const string k_GameManagerKeyId = "de64808d4a589460bbdff927f4f6b4ec";

        private static event Action _onSpawnCompleted;
        public static event Action onSpawnCompleted
        {
            add
            {
                if (isSpawned)
                    value?.Invoke();
                _onSpawnCompleted += value;
            }
            remove
            {
                _onSpawnCompleted -= value;
            }
        }
        public static bool isSpawned => Instance != null;

        private static void NotifyEventSpawnCompleted()
        {
            _onSpawnCompleted?.Invoke();
            if (Instance.m_Verbose)
                LGDebug.Log("GameManager is spawned completely", k_DebugTag);
        }

#if !UNITY_ADDRESSABLES || !UNITY_ADDRESSABLES_MODE
        /// <summary>
        /// Auto spawn GameManager before scene is loaded (Resources version)
        /// *Note: It only works when the Project doesn't use Addressables (because we shouldn't use Resources folder with Addressables, used with caution)
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInstantiate_Resources()
        {
            Instantiate(Resources.Load(k_GameManagerName)).name = k_GameManagerName;
            NotifyEventSpawnCompleted();
        }
#else
        /// <summary>
        /// Auto spawn GameManager before scene is loaded (Addressables version)
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInstantiate_Addressables()
        {
            var asyncOperation = Addressables.InstantiateAsync(k_GameManagerKeyId);
            asyncOperation.Completed += OnInstantiateCompleted;

            void OnInstantiateCompleted(AsyncOperationHandle<GameObject> asyncOperation)
            {
                asyncOperation.Result.name = k_GameManagerName;
                NotifyEventSpawnCompleted();
            }
        }
#endif
    }
}