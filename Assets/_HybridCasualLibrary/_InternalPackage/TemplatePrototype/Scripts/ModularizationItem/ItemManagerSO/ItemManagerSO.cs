using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using HyrphusQ.Events;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ItemManagerSO", menuName = "HyrphusQ/ItemManagerSO/ItemManagerSO")]
public class ItemManagerSO : ItemListSO
{
    #region Fields
    // Event Code Fields
    [SerializeField, BoxGroup("Event Code")]
    protected EventCode m_ItemUsedEventCode;
    [SerializeField, BoxGroup("Event Code")]
    protected EventCode m_ItemSelectedEventCode;
    [SerializeField, BoxGroup("Event Code")]
    protected EventCode m_ItemUnlockedEventCode;
    // Other Fields
    [SerializeField]
    protected string m_DisplayNameTerm;
    [SerializeField]
    protected string m_AnalyticsName;
    [SerializeField]
    protected AdsLocation m_AdsLocation;
    [SerializeField]
    protected ResourceLocation m_ResourceLocation;
    [SerializeField]
    protected PPrefItemSOVariable m_CurrentItemInUse;
    #endregion

    #region Properties
    // Event Code Props
    /// <summary>
    /// Event is raised when item in-use is changed
    /// <para> <typeparamref name="ItemManagerSO"/>: itemManagerSO </para>
    /// <para> <typeparamref name="ItemSO"/>: itemInUse </para>
    /// </summary>
    public virtual EventCode itemUsedEventCode => m_ItemUsedEventCode;
    /// <summary>
    /// Event is raised when item is selected
    /// <para> <typeparamref name="ItemManagerSO"/>: itemManagerSO </para>
    /// <para> <typeparamref name="ItemSO"/>: currentSelectedItem </para>
    /// <para> <typeparamref name="ItemSO"/>: previousSelectedItem </para>
    /// </summary>
    public virtual EventCode itemSelectedEventCode => m_ItemSelectedEventCode;
    /// <summary>
    /// Event is raised when item is unlocked
    /// <para> <typeparamref name="ItemManagerSO"/>: itemManagerSO </para>
    /// <para> <typeparamref name="ItemSO"/>: unlockedItem </para>
    /// </summary>
    public virtual EventCode itemUnlockedEventCode => m_ItemUnlockedEventCode;
    // Other Props
    public virtual string displayName => I2LHelper.TranslateTerm(m_DisplayNameTerm);
    public virtual string analyticsName => m_AnalyticsName;
    public virtual AdsLocation adsLocation => m_AdsLocation;
    public virtual ResourceLocation resourceLocation => m_ResourceLocation;
    public virtual ItemSO currentItemInUse => m_CurrentItemInUse;
    public virtual int itemCount => items.Count;
    public virtual List<ItemSO> items => value;
    #endregion

    #region Private & Protected Methods
    protected virtual void OnEnable()
    {
        if (items == null)
            return;
        foreach (var item in items)
        {
            if (item.TryGetModule(out UnlockableItemModule unlockableModule))
            {
                unlockableModule.onItemUnlocked += OnItemUnlocked;
            }
        }
    }

    protected virtual void OnDisable()
    {
        if (items == null)
            return;
        foreach (var item in items)
        {
            if (item.TryGetModule(out UnlockableItemModule unlockableModule))
            {
                unlockableModule.onItemUnlocked -= OnItemUnlocked;
            }
        }
    }

    // Notify Events
    protected virtual void NotifyEventItemUsed(params object[] eventData)
    {
        // Global event for item in general
        GameEventHandler.Invoke(ItemManagementEventCode.OnItemUsed, eventData);
        // Global event for specificed type of item
        GameEventHandler.Invoke(m_ItemUsedEventCode, eventData);
    }
    protected virtual void NotifyEventItemSelected(params object[] eventData)
    {
        // Global event for item in general
        GameEventHandler.Invoke(ItemManagementEventCode.OnItemSelected, eventData);
        // Global event for specificed type of item
        GameEventHandler.Invoke(m_ItemSelectedEventCode, eventData);
    }
    protected virtual void NotifyEventItemUnlocked(params object[] eventData)
    {
        // Global event for item in general
        GameEventHandler.Invoke(ItemManagementEventCode.OnItemUnlocked, eventData);
        // Global event for specificed type of item
        GameEventHandler.Invoke(m_ItemUnlockedEventCode, eventData);
    }

    // Listen Events
    protected virtual void OnItemUnlocked(UnlockableItemModule unlockableModule)
    {
        // Notify event
        NotifyEventItemUnlocked(this, unlockableModule.itemSO);
    }
    #endregion

    #region Public Methods
    public virtual U Cast<U>() where U : ItemManagerSO
    {
        return this as U;
    }

    public virtual List<T> GetItems<T>() where T : ItemSO
    {
        return items.Cast<T>().ToList();
    }

    public virtual T GetCurrentItemInUsed<T>() where T : ItemSO
    {
        return currentItemInUse.Cast<T>();
    }

    public virtual bool IsAffordable(ItemSO item)
    {
        if (!item.IsPricedItem())
            return false;
        return CurrencyManager.Instance.IsAffordable(item.GetCurrencyType(), item.GetPrice());
    }

    public virtual void Use(ItemSO item)
    {
        if (!item.IsOwned())
            return;
        m_CurrentItemInUse.value = item;

        // Notify event
        NotifyEventItemUsed(this, item);
    }

    public virtual void Select(ItemSO currentSelectedItem, ItemSO previousSelectedItem = null)
    {
        // Notify event
        NotifyEventItemSelected(this, currentSelectedItem, previousSelectedItem);
    }

    public virtual bool TryUnlock(ItemSO item, bool isNotify = true)
    {
        if (item.IsOwned())
            return false;
        return item.TryUnlockItem(isNotify);
    }

    public virtual bool TryUnlockByCurrency(ItemSO item)
    {
        if (!IsAffordable(item))
            return false;
        var isSucceeded = item.TryUnlockItem();
        if (isSucceeded && item.TryGetUnlockRequirement(out Requirement_Currency requirement_Currency))
        {
            requirement_Currency.resourceLocationProvider = new ResourceLocationProvider(resourceLocation, item.GetAnalyticsName());
            requirement_Currency.ExecuteRequirement();
        }
        return isSucceeded;
    }

    public virtual bool TryUnlockByRewardedAd(ItemSO item)
    {
        if (!item.IsRVItem())
            return false;
        item.UpdateRVWatched();
        if (item.GetRVWatchedCount() == item.GetRequiredRVCount())
        {
            // Unlock by watched RV
            return TryUnlock(item);
        }
        return false;
    }

    public virtual ItemSO GetRandomItem()
    {
        return items[Random.Range(0, items.Count)];
    }
    #endregion

    // ===== FOR UNITY EDITOR ONLY =====
#if UNITY_EDITOR
    [Button(SdfIconType.Upload)]
    protected virtual void UpdateName()
    {
        if (items == null || items.Count <= 0)
            return;
        var nameDictionary = new Dictionary<string, int>();
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var itemName = GetDistinctItemName(item.GetInternalName());
            var itemOrder = GetOrderOfItem(itemName);
            var itemTargetName = GetTargetName(itemName, itemOrder);
            if (item.GetInternalName().Equals(itemTargetName))
                continue;
            Debug.Log($"Rename {item} to {itemTargetName}");
            var assetPath = AssetDatabase.GetAssetPath(item);
            assetPath = assetPath.Substring(0, assetPath.LastIndexOf("/"));
            var duplicateNameAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>($"{assetPath}/{itemTargetName}.asset");
            if (duplicateNameAsset != null)
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(duplicateNameAsset), $"{itemTargetName} (Duplicate)");
                EditorUtility.SetDirty(duplicateNameAsset);
            }
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item), itemTargetName);
            EditorUtility.SetDirty(item);
        }

        // Save assets
        AssetDatabase.SaveAssets();

        string GetDistinctItemName(string itemName)
        {
            var length = itemName.IndexOf(" ");
            return length <= 0 ? itemName : itemName.Substring(0, length);
        }
        string GetTargetName(string itemName, int itemOrder)
        {
            return $"{itemName} {itemOrder - 1}";
        }
        int GetOrderOfItem(string itemName)
        {
            if (!nameDictionary.TryGetValue(itemName, out int count))
                count = 0;
            nameDictionary.Set(itemName, ++count);
            return count;
        }
    }
#endif
}
public class ItemManagerSO<T> : ItemManagerSO where T : ItemSO
{
    #region Properties
    public virtual T currentGenericItemInUse => currentItemInUse.Cast<T>();
    public virtual List<T> genericItems => GetItems<T>();
    #endregion
}