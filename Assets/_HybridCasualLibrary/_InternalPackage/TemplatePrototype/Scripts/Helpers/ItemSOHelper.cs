using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class ItemSOHelper
{
    #region Extension Methods
    // ===== General =====
    public static T Cast<T>(this ItemSO itemSO) where T : ItemSO
    {
        return itemSO as T;
    }
    public static string GetItemID(this ItemSO itemSO)
    {
        return itemSO.guid;
    }

    // Name Module
    public static string GetDisplayName(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out NameItemModule nameItemModule))
            return nameItemModule.displayName;
        return itemSO.name;
    }
    public static string GetInternalName(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out NameItemModule nameItemModule))
            return nameItemModule.internalName;
        return itemSO.name;
    }
    public static string GetAnalyticsName(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out NameItemModule nameItemModule))
            return nameItemModule.analyticsName;
        return itemSO.name;
    }

    // Image Module
    public static Image CreateIconImage(this ItemSO itemSO, Image iconImage)
    {
        if (itemSO.TryGetModule(out ImageItemModule imageItemModule))
        {
            return imageItemModule.CreateThumbnailImage(iconImage);
        }
        return iconImage;
    }

    /// <summary>
    /// Get thumbnail image
    /// <para><i> Note: Can only get Mono Thumbnail Image </i></para>
    /// </summary>
    /// <param name="itemSO"></param>
    /// <returns></returns>
    public static Sprite GetThumbnailImage(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule<ImageItemModule>(out var imageItemModule))
        {
            return imageItemModule.thumbnailImage;
        }
        else return null;
    }

    // Unlockable Module
    public static bool IsUnlocked(this ItemSO itemSO)
    {
        return itemSO.TryGetModule(out UnlockableItemModule unlockableModule) ? unlockableModule.isUnlocked : true;
    }
    public static bool IsOwned(this ItemSO itemSO)
    {
        return itemSO.IsUnlocked();
    }
    public static bool IsMeetUnlockRequirements(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out UnlockableItemModule unlockableModule))
        {
            return unlockableModule.IsMeetRequirements();
        }
        return false;
    }
    public static void ResetUnlockToDefault(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out UnlockableItemModule unlockableModule))
        {
            unlockableModule.ResetUnlockToDefault();
        }
    }
    public static bool TryUnlockItem(this ItemSO itemSO, bool isNotify = true, bool isSetNew = true)
    {
        if (itemSO.TryGetModule(out UnlockableItemModule unlockableModule))
        {
            var isSucceeded = unlockableModule.TryUnlock(isNotify);
            if (isSucceeded && isSetNew)
                itemSO.SetNewItem(true);
            return isSucceeded;
        }
        return false;
    }
    public static bool TryUnlockIgnoreRequirement(this ItemSO itemSO, bool isNotify = true, bool isSetNew = true)
    {
        if (itemSO.TryGetModule(out UnlockableItemModule unlockableModule))
        {
            var isSucceeded = unlockableModule.TryUnlockIgnoreRequirement(isNotify);
            if (isSucceeded && isSetNew)
                itemSO.SetNewItem(true);
            return isSucceeded;
        }
        return false;
    }
    public static bool TryGetUnlockRequirement<T>(this ItemSO itemSO, out T requirement, Predicate<T> predicate = null) where T : Requirement
    {
        requirement = null;
        if (itemSO.TryGetModule(out UnlockableItemModule unlockableModule))
        {
            return unlockableModule.TryGetUnlockRequirement(out requirement, predicate);
        }
        return false;
    }
    public static T GetUnlockRequirement<T>(this ItemSO itemSO, Predicate<T> predicate = null) where T : Requirement
    {
        if (itemSO.TryGetUnlockRequirement(out T requirement, predicate))
        {
            return requirement;
        }
        return null;
    }
    public static List<Requirement> GetUnlockRequirements(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out UnlockableItemModule unlockableModule))
        {
            return unlockableModule.GetUnlockRequirements();
        }
        return null;
    }

    // ===== Price =====
    public static bool IsPricedItem(this ItemSO itemSO)
    {
        return itemSO.TryGetModule(out UnlockableItemModule unlockableModule) ? unlockableModule.TryGetUnlockRequirement<Requirement_Currency>(out _) : false;
    }
    public static float GetPrice(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out UnlockableItemModule unlockableModule))
        {
            if (unlockableModule.TryGetUnlockRequirement(out Requirement_Currency requirementCurrency))
            {
                return requirementCurrency.requiredAmountOfCurrency;
            }
        }
        return Const.IntValue.Invalid;
    }
    public static CurrencyType GetCurrencyType(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out UnlockableItemModule unlockableModule))
        {
            if (unlockableModule.TryGetUnlockRequirement(out Requirement_Currency requirementCurrency))
            {
                return requirementCurrency.currencySO.CurrencyType;
            }
        }
        return 0;
    }

    // ===== Rewarded Ad =====
    public static bool IsRVItem(this ItemSO itemSO)
    {
        return itemSO.TryGetModule(out UnlockableItemModule unlockableModule) ? unlockableModule.TryGetUnlockRequirement<Requirement_RewardedAd>(out _) : false;
    }
    public static int GetRequiredRVCount(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out UnlockableItemModule unlockableModule))
        {
            if (unlockableModule.TryGetUnlockRequirement(out Requirement_RewardedAd requirementRewardedAd))
            {
                return requirementRewardedAd.requiredRewardedAd;
            }
        }
        return Const.IntValue.Invalid;
    }
    public static int GetRVWatchedCount(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out UnlockableItemModule unlockableModule))
        {
            if (unlockableModule.TryGetUnlockRequirement(out Requirement_RewardedAd requirementRewardedAd))
            {
                return requirementRewardedAd.progressRewardedAd;
            }
        }
        return Const.IntValue.Invalid;
    }
    public static void UpdateRVWatched(this ItemSO itemSO, int rvWatched = 1)
    {
        if (itemSO.TryGetModule(out UnlockableItemModule unlockableModule))
        {
            if (unlockableModule.TryGetUnlockRequirement(out Requirement_RewardedAd requirementRewardedAd))
            {
                requirementRewardedAd.progressRewardedAd += rvWatched;
            }
        }
    }

    // ===== Progression =====
    public static bool IsProgressionItem(this ItemSO itemSO)
    {
        return false;
    }
    public static bool IsProgressionItemExposed(this ItemSO itemSO)
    {
        // Not implement yet
        return true;
    }

    // ===== IAP =====
    public static bool IsIAPItem(this ItemSO itemSO)
    {
        return itemSO.TryGetModule(out UnlockableItemModule unlockableModule) ? unlockableModule.TryGetUnlockRequirement<Requirement_IAP>(out _) : false;
    }
    public static string IAPBundleID(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out UnlockableItemModule unlockableModule))
        {
            if (unlockableModule.TryGetUnlockRequirement(out Requirement_IAP requirementIAP))
            {
                return requirementIAP.bundleID;
            }
        }
        return null;
    }

    // Rarity Module
    public static RarityType GetRarityType(this ItemSO itemSO)
    {
        return itemSO.TryGetModule(out RarityItemModule rarityItemModule) ? rarityItemModule.rarityType : RarityType.Common;
    }

    // Upgradable Module
    public static bool IsUpgradable(this ItemSO itemSO)
    {
        return itemSO.TryGetModule(out UpgradableItemModule upgradableModule) ? upgradableModule.isAbleToUpgrade : false;
    }
    public static bool IsMaxUpgradeLevel(this ItemSO itemSO)
    {
        return itemSO.TryGetModule(out UpgradableItemModule upgradableModule) ? upgradableModule.isMaxUpgradeLevel : false;
    }
    public static bool IsEnoughCardToUpgrade(this ItemSO itemSO)
    {
        return !itemSO.IsMaxUpgradeLevel() && (itemSO.TryGetCurrentUpgradeRequirement(out Requirement_GachaCard requirement) ? requirement.IsMeetRequirement() : false);
    }
    public static int GetDefaultUpgradeLevel(this ItemSO itemSO)
    {
        return itemSO.TryGetModule(out UpgradableItemModule upgradableModule) ? upgradableModule.defaultUpgradeLevel : 1;
    }
    public static int GetCurrentUpgradeLevel(this ItemSO itemSO)
    {
        return itemSO.TryGetModule(out UpgradableItemModule upgradableModule) ? upgradableModule.upgradeLevel : 1;
    }
    public static int GetNextUpgradeLevel(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out UpgradableItemModule upgradableModule))
        {
            return Mathf.Min(upgradableModule.upgradeLevel + 1, upgradableModule.maxUpgradeLevel);
        }
        return 1;
    }
    public static int GetMaxUpgradeLevel(this ItemSO itemSO)
    {
        return itemSO.TryGetModule(out UpgradableItemModule upgradableModule) ? upgradableModule.maxUpgradeLevel : 1;
    }
    public static bool TryUpgrade(this ItemSO itemSO, bool isNotify = true)
    {
        if (itemSO.TryGetModule(out UpgradableItemModule upgradableModule))
            return upgradableModule.TryUpgrade(isNotify);
        return false;
    }
    public static bool TryUpgradeIgnoreRequirement(this ItemSO itemSO, bool isNotify = true)
    {
        if (itemSO.TryGetModule(out UpgradableItemModule upgradableModule))
            return upgradableModule.TryUpgradeIgnoreRequirement(isNotify);
        return false;
    }
    public static void ExecuteCurrentUpgradeRequirements(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out UpgradableItemModule upgradableModule))
            upgradableModule.ExecuteCurrentUpgradeRequirements();
    }
    public static List<Requirement> GetUpgradeRequirementsOfLevel(this ItemSO itemSO, int upgradeLevel)
    {
        if (itemSO.TryGetModule(out UpgradableItemModule upgradableModule))
            return upgradableModule.GetUpgradeRequirementsOfLevel(upgradeLevel);
        return null;
    }
    public static bool TryGetCurrentUpgradeRequirement<T>(this ItemSO itemSO, out T requirement) where T : Requirement
    {
        requirement = null;
        if (itemSO.TryGetModule(out UpgradableItemModule upgradableModule))
            return upgradableModule.TryGetCurrentUpgradeRequirement(out requirement);
        return false;
    }
    public static bool TryGetUpgradeRequirementOfLevel<T>(this ItemSO itemSO, int upgradeLevel, out T requirement) where T : Requirement
    {
        requirement = null;
        if (itemSO.TryGetModule(out UpgradableItemModule upgradableModule))
            return upgradableModule.TryGetUpgradeRequirementOfLevel(upgradeLevel, out requirement);
        return false;
    }
    public static List<Requirement> GetCurrentUpgradeRequirements(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out UpgradableItemModule upgradableModule))
            return upgradableModule.GetCurrentUpgradeRequirements();
        return null;
    }
    public static List<Requirement> GetCurrentSatisfiedUpgradeRequirements(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out UpgradableItemModule upgradableModule))
            return upgradableModule.GetCurrentUpgradeRequirements().Where(req => req.IsMeetRequirement()).ToList();
        return null;
    }
    public static List<Requirement> GetCurrentUnsatisfiedUpgradeRequirements(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out UpgradableItemModule upgradableModule))
            return upgradableModule.GetCurrentUpgradeRequirements().Where(req => !req.IsMeetRequirement()).ToList();
        return null;
    }
    public static void ResetUpgradeLevelToDefault(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out UpgradableItemModule upgradableModule))
            upgradableModule.ResetUpgradeLevelToDefault();
    }

    // Card Module
    public static int GetNumOfCards(this ItemSO itemSO)
    {
        return itemSO.TryGetModule(out CardItemModule cardModule) ? cardModule.numOfCards : Const.IntValue.Zero;
    }
    public static void UpdateNumOfCards(this ItemSO itemSO, int numOfCards, bool isNotify = true)
    {
        if (itemSO.TryGetModule(out CardItemModule cardModule))
            cardModule.UpdateNumOfCards(numOfCards, isNotify);
    }

    // Form Module
    public static bool HasNextForm(this ItemSO itemSO)
    {
        return itemSO.TryGetModule(out FormItemModule formModule) ? formModule.hasNextForm : false;
    }
    public static int GetFormCount(this ItemSO itemSO)
    {
        return itemSO.TryGetModule(out FormItemModule formModule) ? formModule.formCount : Const.IntValue.Zero;
    }
    public static int GetCurrentFormIndex(this ItemSO itemSO)
    {
        return itemSO.TryGetModule(out FormItemModule formModule) ? formModule.currentFormIndex : Const.IntValue.Zero;
    }
    public static T GetCurrentForm<T>(this ItemSO itemSO) where T : BaseForm
    {
        return itemSO.GetForm<T>(itemSO.GetCurrentFormIndex());
    }
    public static T GetForm<T>(this ItemSO itemSO, int formIndex) where T : BaseForm
    {
        return itemSO.TryGetModule(out FormItemModule<T> formModule) ? formModule.GetForm(formIndex) : null;
    }

    // Reward Module
    public static void GrantReward(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out RewardModule rewardModule))
        {
            rewardModule.GrantReward();
        }
    }

    // New Module
    public static bool IsNew(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out NewItemModule newModule))
        {
            return newModule.isNew;
        }
        return false;
    }
    public static void SetNewItem(this ItemSO itemSO, bool isNew)
    {
        if (itemSO.TryGetModule(out NewItemModule newModule))
        {
            newModule.isNew = isNew;
        }
    }

    // Prefab Based Module
    public static GameObject GetModelPrefabAsGameObject(this ItemSO itemSO)
    {
        if (itemSO.TryGetModule(out ModelPrefabItemModule modelPrefabModule))
        {
            return modelPrefabModule.GetModelPrefabAsGameObject();
        }
        return null;
    }
    public static T GetModelPrefab<T>(this ItemSO itemSO) where T : Component
    {
        if (itemSO.TryGetModule(out ModelPrefabItemModule modelPrefabModule))
        {
            return modelPrefabModule.GetModelPrefab<T>();
        }
        return null;
    }
    #endregion
}