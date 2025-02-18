using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Requirement_RewardedAd : Requirement
{
    [SerializeField]
    protected int m_RequiredRewardedAd = 1;
    [SerializeField]
    protected ItemSO m_ItemSO;

    public int requiredRewardedAd
    {
        get => m_RequiredRewardedAd;
        set => m_RequiredRewardedAd = value;
    }
    public int progressRewardedAd
    {
        get => PlayerPrefs.GetInt($"RewardedAdProgress_{m_ItemSO.guid}", Const.IntValue.Zero);
        set => PlayerPrefs.SetInt($"RewardedAdProgress_{m_ItemSO.guid}", Mathf.Clamp(value, Const.IntValue.Zero, requiredRewardedAd));
    }
    public ItemSO itemSO
    {
        get => m_ItemSO;
        set => m_ItemSO = value;
    }

    public override bool IsMeetRequirement()
    {
        return progressRewardedAd >= requiredRewardedAd;
    }

    public override void ExecuteRequirement()
    {
        progressRewardedAd = requiredRewardedAd;
    }
}