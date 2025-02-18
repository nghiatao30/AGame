using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaItemCardRewardModule : RewardModule
{
    [SerializeField]
    protected GachaItemSO gachaItemSO;

    public GachaItemSO GachaItemSO { get => gachaItemSO; set => gachaItemSO = value; }

    public override void GrantReward()
    {
        base.GrantReward();
        gachaItemSO.UpdateNumOfCards(gachaItemSO.GetNumOfCards() + 1);
        gachaItemSO.TryUnlockItem();
    }
}