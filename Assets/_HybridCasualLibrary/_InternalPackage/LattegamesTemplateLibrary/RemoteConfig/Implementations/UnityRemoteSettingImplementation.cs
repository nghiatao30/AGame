using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
    public class UnityRemoteSettingImplementation : RemoteConfigServiceMono, IRemoteConfigService
    {
        public float GetFloat(string key, float defaultValue)
        {
            return RemoteSettings.GetFloat(key, defaultValue);
        }

        public int GetInt(string key, int defaultValue)
        {
            return RemoteSettings.GetInt(key, defaultValue);
        }

        public override IRemoteConfigService GetService()
        {
            return this;
        }

        public string GetString(string key, string defaultValue)
        {
            return RemoteSettings.GetString(key, defaultValue);
        }

        public bool GetBool(string key, bool defaultValue)
        {
            return RemoteSettings.GetBool(key, defaultValue);
        }

        private void Awake()
        {
            RemoteSettings.Completed += HandleRemoteSettings;
            RemoteSettings.ForceUpdate();
        }

        private void HandleRemoteSettings(bool wasUpdatedFromServer, bool settingsChanged, int serverResponse)
        {

        }
    }
}