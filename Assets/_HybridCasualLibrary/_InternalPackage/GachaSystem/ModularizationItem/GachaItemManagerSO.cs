using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class GachaItemManagerSO<T> : ItemManagerSO<T> where T : GachaItemSO
{
    #region Fields
    // Event Code Fields
    [SerializeField, BoxGroup("Event Code")]
    protected EventCode m_ItemCardChangedEventCode;
    [SerializeField, BoxGroup("Event Code")]
    protected EventCode m_ItemUpgradedEventCode;
    [SerializeField, BoxGroup("Event Code")]
    protected EventCode m_ItemNewStateChangedEventCode;
    #endregion

    #region Properties
    // Event Code Props
    /// <summary>
    /// Event is raised when gacha item card is changed
    /// <para> <typeparamref name="GachaItemManagerSO_T"/>: gachaItemManagerSO&lt;T&gt; where T : GachaItemSO </para>
    /// <para> <typeparamref name="GachaItemSO"/>: gachaItemSO </para>
    /// <para> <typeparamref name="int"/>: numOfCards </para>
    /// </summary>
    public virtual EventCode itemCardChangedEventCode => m_ItemCardChangedEventCode;
    /// <summary>
    /// This event is raised when gacha item is upgraded
    /// <para> <typeparamref name="GachaItemManagerSO_T"/>: gachaItemManagerSO&lt;T&gt; where T : GachaItemSO </para>
    /// <para> <typeparamref name="GachaItemSO"/>: gachaItemSO </para>
    /// <para> <typeparamref name="int"/>: upgradeLevel </para>
    /// </summary>
    public virtual EventCode itemUpgradedEventCode => m_ItemUpgradedEventCode;
    /// <summary>
    /// This event is raised when new state of gacha item is changed
    /// <para> <typeparamref name="GachaItemManagerSO_T"/>: gachaItemManagerSO&lt;T&gt; where T : GachaItemSO </para>
    /// <para> <typeparamref name="GachaItemSO"/>: gachaItemSO </para>
    /// <para> <typeparamref name="bool"/>: isNew </para>
    /// </summary>
    public virtual EventCode itemNewStateChangedEventCode => m_ItemNewStateChangedEventCode;
    // Other Props
    #endregion

    #region Private & Protected Methods
    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (var item in items)
        {
            if (item.TryGetModule(out CardItemModule cardModule))
            {
                cardModule.onNumOfCardsChanged += OnItemCardChanged;
            }
            if (item.TryGetModule(out UpgradableItemModule upgradableModule))
            {
                upgradableModule.onUpgradeLevelChanged += OnItemUpgradeLevelChanged;
            }
            if (item.TryGetModule(out NewItemModule newItemModule))
            {
                newItemModule.onNewStateChanged += OnItemNewStateChanged;
            }
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        foreach (var item in items)
        {
            if (item.TryGetModule(out CardItemModule cardModule))
            {
                cardModule.onNumOfCardsChanged -= OnItemCardChanged;
            }
            if (item.TryGetModule(out UpgradableItemModule upgradableModule))
            {
                upgradableModule.onUpgradeLevelChanged -= OnItemUpgradeLevelChanged;
            }
            if (item.TryGetModule(out NewItemModule newItemModule))
            {
                newItemModule.onNewStateChanged -= OnItemNewStateChanged;
            }
        }
    }

    // Notify Events
    protected virtual void NotifyEventItemCardChanged(params object[] eventData)
    {
        if (m_ItemCardChangedEventCode == null || m_ItemCardChangedEventCode.eventCode == null)
            return;
        GameEventHandler.Invoke(m_ItemCardChangedEventCode, eventData);
    }

    protected virtual void NotifyEventItemUpgraded(params object[] eventData)
    {
        if (m_ItemUpgradedEventCode == null || m_ItemUpgradedEventCode.eventCode == null)
            return;
        GameEventHandler.Invoke(m_ItemUpgradedEventCode, eventData);
    }

    protected virtual void NotifyEventItemNewStateChanged(params object[] eventData)
    {
        if (m_ItemNewStateChangedEventCode == null || m_ItemNewStateChangedEventCode.eventCode == null)
            return;
        GameEventHandler.Invoke(m_ItemNewStateChangedEventCode, eventData);
    }

    // Listen Events
    protected virtual void OnItemCardChanged(CardItemModule cardModule)
    {
        // Notify event
        NotifyEventItemCardChanged(this, cardModule.itemSO, cardModule.numOfCards);
    }

    protected virtual void OnItemUpgradeLevelChanged(UpgradableItemModule upgradableModule)
    {
        // Notify event
        NotifyEventItemUpgraded(this, upgradableModule.itemSO, upgradableModule.upgradeLevel);
    }

    protected virtual void OnItemNewStateChanged(NewItemModule newItemModule)
    {
        // Notify event
        NotifyEventItemNewStateChanged(this, newItemModule.itemSO, newItemModule.isNew);
    }
    #endregion

    #region Public Methods
    public virtual void UpdateItemCard(T gachaItemSO, int numOfCards, bool isNotify = true)
    {
        gachaItemSO.UpdateNumOfCards(numOfCards, isNotify);
    }

    public virtual bool TryUpgradeItem(T gachaItemSO, bool isNotify = true)
    {
        return gachaItemSO.TryUpgrade(isNotify);
    }
    #endregion
}