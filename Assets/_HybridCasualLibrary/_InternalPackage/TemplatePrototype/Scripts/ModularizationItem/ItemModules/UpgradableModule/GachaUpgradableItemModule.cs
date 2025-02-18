using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GachaUpgradeRequirementData
{
    [SerializeField]
    protected List<int> m_RequiredNumOfCardsLevels;

    public virtual int RequiredNumOfCardsCount()
    {
        return m_RequiredNumOfCardsLevels.Count;
    }

    public virtual int GetRequiredNumOfCards(int upgradeLevel)
    {
        return m_RequiredNumOfCardsLevels[Mathf.Min(upgradeLevel - 1, m_RequiredNumOfCardsLevels.Count - 1)];
    }
}
[CustomInspectorName("GachaUpgradableModule")]
public abstract class GachaUpgradableItemModule<T> : UpgradableItemModule where T : GachaUpgradeRequirementData
{
    [SerializeField]
    protected T m_UpgradeRequirementData;

    public override int maxUpgradeLevel => m_UpgradeRequirementData.RequiredNumOfCardsCount();

    public override List<Requirement> GetUpgradeRequirementsOfLevel(int upgradeLevel)
    {
        return new List<Requirement>
        {
            new Requirement_GachaCard()
            {
                requiredNumOfCards = m_UpgradeRequirementData.GetRequiredNumOfCards(upgradeLevel),
                gachaItemSO = itemSO
            },
        };
    }
}