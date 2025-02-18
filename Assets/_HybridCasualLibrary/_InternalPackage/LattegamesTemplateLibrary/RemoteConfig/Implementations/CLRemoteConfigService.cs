using System.Collections;
using System.Collections.Generic;
using LatteGames;
using Sirenix.OdinInspector;
using UnityEngine;
using HyrphusQ.SerializedDataStructure;

[OptionalDependency("CL_RemoteConfigHelper", "LatteGames_CLRemoteConfig")]
public class CLRemoteConfigService : RemoteConfigServiceMono, IRemoteConfigService
{
#if !LatteGames_CLRemoteConfig
    [SerializeField, LabelText("Local config for testing purpose without CLRemoteConfig plugin")]
    private SerializedDictionary<string, string> localConfigDictionary;
#endif

    public float GetFloat(string key, float defaultValue)
    {
#if LatteGames_CLRemoteConfig
        return CL_RemoteConfigHelper.Instance.GetFloat(key, defaultValue);
#else
        return float.Parse(localConfigDictionary.Get(key, defaultValue.ToString("R")));
#endif
    }

    public int GetInt(string key, int defaultValue)
    {
#if LatteGames_CLRemoteConfig
        return CL_RemoteConfigHelper.Instance.GetInt(key, defaultValue);
#else
        return int.Parse(localConfigDictionary.Get(key, defaultValue.ToString()));
#endif
    }

    public string GetString(string key, string defaultValue)
    {
#if LatteGames_CLRemoteConfig
        return CL_RemoteConfigHelper.Instance.GetString(key, defaultValue);
#else
        return localConfigDictionary.Get(key, defaultValue);
#endif
    }

    public bool GetBool(string key, bool defaultValue)
    {
#if LatteGames_CLRemoteConfig
        return CL_RemoteConfigHelper.Instance.GetBool(key, defaultValue);
#else
        return bool.Parse(localConfigDictionary.Get(key, defaultValue.ToString()));
#endif
    }

    public override IRemoteConfigService GetService()
    {
        return this;
    }
}