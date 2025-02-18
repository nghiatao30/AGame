using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvPBoosterRewardModule : RewardModule
{
    [SerializeField]
    protected PvPBoosterType boosterType;
    [SerializeField]
    protected PPrefIntVariable boosterSavingSO;

    public virtual PvPBoosterType BoosterType { get => boosterType; set => boosterType = value; }
    public virtual PPrefIntVariable BoosterSavingSO { get => boosterSavingSO; set => boosterSavingSO = value; }

    public override void GrantReward()
    {
        base.GrantReward();
        // Increase booster count
        boosterSavingSO.value++;
    }
}