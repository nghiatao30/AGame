using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomInspectorName("UpgradableModule")]
public abstract class UpgradableItemModule : ItemModule
{
    public event Action<UpgradableItemModule> onUpgradeLevelChanged;

    protected string saveKey => $"{nameof(UpgradableItemModule)}_{m_ItemSO.guid}";

    public virtual bool isAbleToUpgrade => IsMeetCurrentRequirements() && !isMaxUpgradeLevel;
    public virtual bool isMaxUpgradeLevel => upgradeLevel >= maxUpgradeLevel;
    public virtual int maxUpgradeLevel => int.MaxValue;
    public virtual int defaultUpgradeLevel => Const.IntValue.One;
    public virtual int upgradeLevel
    {
        get
        {
            return PlayerPrefs.GetInt(saveKey, defaultUpgradeLevel);
        }
        protected set
        {
            PlayerPrefs.SetInt(saveKey, value);
        }
    }

    public virtual void ResetUpgradeLevelToDefault()
    {
        upgradeLevel = defaultUpgradeLevel;
    }
    public virtual bool TryUpgrade(bool isNotify = true)
    {
        if (!isAbleToUpgrade)
            return false;
        ExecuteCurrentUpgradeRequirements();
        upgradeLevel++;
        if (isNotify)
            onUpgradeLevelChanged?.Invoke(this);
        return true;
    }
    public virtual bool TryUpgradeIgnoreRequirement(bool isNotify = true)
    {
        if (isMaxUpgradeLevel)
            return false;
        upgradeLevel++;
        if (isNotify)
            onUpgradeLevelChanged?.Invoke(this);
        return true;
    }

    public virtual bool IsMeetCurrentRequirements() => IsMeetRequirementsOfLevel(upgradeLevel + 1);
    public virtual bool IsMeetRequirementsOfLevel(int upgradeLevel) => GetUpgradeRequirementsOfLevel(upgradeLevel).TrueForAll(item => item.IsMeetRequirement());
    public virtual Requirement[][] GetAllUpgradeRequirements()
    {
        var requirementArr2D = new Requirement[maxUpgradeLevel][];
        for (int i = upgradeLevel + 1; i <= maxUpgradeLevel; i++)
        {
            requirementArr2D[i - 2] = GetUpgradeRequirementsOfLevel(i).ToArray();
        }
        return requirementArr2D;
    }
    public virtual bool TryGetUpgradeRequirementOfLevel<T>(int upgradeLevel, out T requirement) where T : Requirement
    {
        requirement = GetUpgradeRequirementsOfLevel(upgradeLevel).Find(item => item is T) as T;
        return requirement != null;
    }
    public virtual bool TryGetCurrentUpgradeRequirement<T>(out T requirement) where T : Requirement
    {
        requirement = GetCurrentUpgradeRequirements().Find(item => item is T) as T;
        return requirement != null;
    }
    public virtual void ExecuteCurrentUpgradeRequirements() => ExecuteUpgradeRequirementsOfLevel(upgradeLevel + 1);
    public virtual void ExecuteUpgradeRequirementsOfLevel(int upgradeLevel) => GetUpgradeRequirementsOfLevel(upgradeLevel).ForEach(requirement => requirement.ExecuteRequirement());
    public virtual List<Requirement> GetCurrentUpgradeRequirements() => GetUpgradeRequirementsOfLevel(upgradeLevel + 1);
    public abstract List<Requirement> GetUpgradeRequirementsOfLevel(int upgradeLevel);
}