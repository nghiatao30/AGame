using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Requirement_GachaCard : Requirement
{
    [SerializeField]
    protected int m_RequiredNumOfCards;
    [SerializeField]
    protected ItemSO m_GachaItemSO;

    public int requiredNumOfCards
    {
        get => m_RequiredNumOfCards;
        set => m_RequiredNumOfCards = value;
    }
    public int currentNumOfCards
    {
        get => m_GachaItemSO.GetNumOfCards();
    }
    public ItemSO gachaItemSO
    {
        get => m_GachaItemSO;
        set => m_GachaItemSO = value;
    }

    public override bool IsMeetRequirement()
    {
        return currentNumOfCards >= requiredNumOfCards;
    }

    public override void ExecuteRequirement()
    {
        m_GachaItemSO.UpdateNumOfCards(currentNumOfCards - requiredNumOfCards);
    }
}