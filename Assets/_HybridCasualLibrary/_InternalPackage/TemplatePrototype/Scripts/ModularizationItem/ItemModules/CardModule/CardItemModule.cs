using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomInspectorName("CardModule")]
public class CardItemModule : ItemModule
{
    public event Action<CardItemModule> onNumOfCardsChanged;

    protected string saveKey => $"{nameof(CardItemModule)}_{m_ItemSO.guid}";

    public virtual int numOfCards
    {
        get => PlayerPrefs.GetInt(saveKey, Const.IntValue.Zero);
        protected set
        {
            PlayerPrefs.SetInt(saveKey, value);
        }
    }

    protected void NotifyEventNumOfCardsChanged()
    {
        onNumOfCardsChanged?.Invoke(this);
    }

    public virtual void UpdateNumOfCards(int numOfCards, bool isNotify = true)
    {
        this.numOfCards = numOfCards;
        if (isNotify)
            NotifyEventNumOfCardsChanged();
    }
}