using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
    public class RemoteConfigManager : MonoBehaviour
    {
        protected static RemoteConfigManager Instance;

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            Instance = this;
        }

        public static float GetFloat(string key, float defaultValue)
        {
            return GetService()?.GetFloat(key, defaultValue) ?? defaultValue;
        }

        public static int GetInt(string key, int defaultValue)
        {
            return GetService()?.GetInt(key, defaultValue) ?? defaultValue;
        }

        public static string GetString(string key, string defaultValue)
        {
            return GetService()?.GetString(key, defaultValue) ?? defaultValue;
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            return GetService()?.GetBool(key, defaultValue) ?? defaultValue;
        }

        private static IRemoteConfigService GetService()
        {
            var service = Instance?.serviceRef.GetService();
            return service;
        }

#pragma warning disable 0649
        [SerializeField]
        private RemoteConfigServiceMono serviceRef;
#pragma warning restore 0649
    }
}