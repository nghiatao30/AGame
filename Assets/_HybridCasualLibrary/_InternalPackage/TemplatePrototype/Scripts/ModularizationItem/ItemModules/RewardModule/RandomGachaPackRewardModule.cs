using System.Collections;
using System.Collections.Generic;
using GachaSystem.Core;
using HyrphusQ.Events;
using UnityEngine;

public class RandomGachaPackRewardModule : RewardModule
{
    [SerializeField]
    protected GachaPacksCollection m_GachaPackCollection;

    public GachaPacksCollection gachaPackCollection => m_GachaPackCollection;
    public IResourceLocationProvider resourceLocationProvider { get; set; }

    public override void GrantReward()
    {
        var gachaPack = m_GachaPackCollection.GetRandom();
        gachaPack.ResourceLocationProvider = resourceLocationProvider;
        GameEventHandler.Invoke(UnpackEventCode.OnUnpackStart, null, new List<GachaPack>() { gachaPack }, null);
    }
}