using System.Collections;
using System.Collections.Generic;
#if LatteGames_UnityRC
using Unity.RemoteConfig;
#endif
using UnityEngine;

namespace LatteGames
{
    [OptionalDependency("Unity.RemoteConfig", "LatteGames_UnityRC")]
    public class UnityRemoteConfigImplementation : RemoteConfigServiceMono, IRemoteConfigService
    {

        public override IRemoteConfigService GetService()
        {
            return this;
        }

#if LatteGames_UnityRC
        public struct userAttributes { }
        public struct appAttributes { }

        private void Awake()
        {
            ConfigManager.FetchCompleted += ApplyRemoteSettings;
            ConfigManager.FetchConfigs(new userAttributes(), new appAttributes());
        }
        
        void ApplyRemoteSettings(ConfigResponse configResponse)
        {
            Debug.Log($"remote config return code:: {configResponse.status}");
            switch (configResponse.requestOrigin)
            {
                case ConfigOrigin.Default:
                    Debug.Log("No settings loaded this session; using default values.");
                    break;
                case ConfigOrigin.Cached:
                    Debug.Log("No settings loaded this session; using cached values from a previous session.");
                    break;
                case ConfigOrigin.Remote:
                    Debug.Log("New settings loaded this session; update values accordingly.");
                    break;
            }
            Debug.Log($"Remote config keys count:: {ConfigManager.appConfig.GetKeys().Length}");
        }
#endif

        public float GetFloat(string key, float defaultValue)
        {
#if LatteGames_UnityRC
            return ConfigManager.appConfig.GetFloat(key, defaultValue);
#else
            return defaultValue;
#endif
        }

        public int GetInt(string key, int defaultValue)
        {
#if LatteGames_UnityRC
            return ConfigManager.appConfig.GetInt(key, defaultValue);
#else
            return defaultValue;
#endif
        }

        public string GetString(string key, string defaultValue)
        {
#if LatteGames_UnityRC
            return ConfigManager.appConfig.GetString(key, defaultValue);
#else
            return defaultValue;
#endif
        }

        public bool GetBool(string key, bool defaultValue)
        {
#if LatteGames_UnityRC
            return ConfigManager.appConfig.GetBool(key, defaultValue);
#else
            return defaultValue;
#endif
        }
    }
}