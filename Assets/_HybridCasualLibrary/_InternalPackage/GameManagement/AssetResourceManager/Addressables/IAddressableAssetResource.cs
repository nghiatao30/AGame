using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif

#if UNITY_ADDRESSABLES
public enum StateFlags
{
    /// <summary>
    /// Not load in memory yet
    /// </summary>
    Unloaded,
    /// <summary>
    /// Already loaded in memory but will never used in the future any more and should be unload
    /// </summary>
    NotUsed,
    /// <summary>
    /// Alreadd loaded in memory and in using
    /// </summary>
    InUsed,
}
public interface IAddressableAssetResource
{
    StateFlags stateFlags { get; set; }

    AssetReference[] CollectResources();
    void ReleaseResources();
}
#endif