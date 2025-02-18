using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace LatteGames
{
    [OptionalDependency("Firebase.RemoteConfig.FirebaseRemoteConfig", "LatteGames_FirebaseRC")]
    public class FirebaseRemoteConfig : RemoteConfigServiceMono, IRemoteConfigService
    {
        [SerializeField]
        private bool developerMode;

        public float GetFloat(string key, float defaultValue)
        {
            if (!HasKey(key))
                return defaultValue;
#if LatteGames_FirebaseRC
            return (float)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(key).DoubleValue;
#else
            return defaultValue;
#endif
        }

        public int GetInt(string key, int defaultValue)
        {
            if (!HasKey(key))
                return defaultValue;
#if LatteGames_FirebaseRC
            return (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(key).DoubleValue;
#else
            return defaultValue;
#endif
        }

        public override IRemoteConfigService GetService()
        {
            return this;
        }

        public string GetString(string key, string defaultValue)
        {
            if (!HasKey(key))
                return defaultValue;

#if LatteGames_FirebaseRC
            return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(key).StringValue;
#else
            return defaultValue;
#endif
        }

        public bool GetBool(string key, bool defaultValue)
        {
            if (!HasKey(key))
                return defaultValue;
#if LatteGames_FirebaseRC
            return (int)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(key).BooleanValue;
#else
            return defaultValue;
#endif
        }

        private bool HasKey(string key)
        {
#if LatteGames_FirebaseRC
            foreach (var remoteKey in Firebase.RemoteConfig.FirebaseRemoteConfig.Keys)
            {
                if (remoteKey == key)
                    return true;
            }
            return false;
#else
            return false;
#endif
        }

#if LatteGames_FirebaseRC
        private void Awake()
        {
            System.Collections.Generic.Dictionary<string, object> defaults =
                new System.Collections.Generic.Dictionary<string, object>();

            Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
            Firebase.RemoteConfig.FirebaseRemoteConfig.Settings = new Firebase.RemoteConfig.ConfigSettings()
            {
                IsDeveloperMode = developerMode | Application.isEditor
            };
            _ = FetchDataAsync();
        }

        public async Task FetchDataAsync()
        {
            try
            {
                Debug.Log("Fetching data...");
                System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(
                    TimeSpan.Zero);
                await fetchTask;
                Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();
                Debug.Log("FetchComplete");
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }

        }
#endif
    }
}