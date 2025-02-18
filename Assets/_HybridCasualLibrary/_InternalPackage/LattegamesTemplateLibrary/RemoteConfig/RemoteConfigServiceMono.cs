using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LatteGames
{
    public abstract class RemoteConfigServiceMono : MonoBehaviour
    {
        public abstract IRemoteConfigService GetService();
    }
}