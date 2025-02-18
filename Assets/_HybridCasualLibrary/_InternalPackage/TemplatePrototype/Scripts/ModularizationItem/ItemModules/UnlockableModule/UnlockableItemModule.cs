using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CustomInspectorName("UnlockableModule")]
public class UnlockableItemModule : ItemModule
{
    public event Action<UnlockableItemModule> onItemUnlocked;

    [SerializeField]
    protected bool m_DefaultIsUnlocked;
    [SerializeReference]
    protected Requirement m_UnlockRequirementTree;

    protected string unlockSaveKey => $"{nameof(UnlockableItemModule)}_{m_ItemSO.guid}";

    public virtual bool isAbleToUnlock => IsMeetRequirements() && !isUnlocked;
    public virtual bool isUnlocked
    {
        get
        {
            return PlayerPrefs.GetInt(unlockSaveKey, m_DefaultIsUnlocked ? 1 : 0) == 1;
        }
        protected set
        {
            PlayerPrefs.SetInt(unlockSaveKey, value ? 1 : 0);
        }
    }
    public virtual bool defaultIsUnlocked => m_DefaultIsUnlocked;

    public virtual void ResetUnlockToDefault()
    {
        PlayerPrefs.DeleteKey(unlockSaveKey);
    }
    public virtual bool TryUnlock(bool isNotify = true)
    {
        if (!isAbleToUnlock)
            return false;
        isUnlocked = true;
        if (isNotify)
            onItemUnlocked?.Invoke(this);
        return true;
    }
    public virtual bool TryUnlockIgnoreRequirement(bool isNotify = true)
    {
        if (isUnlocked)
            return false;
        isUnlocked = true;
        if (isNotify)
            onItemUnlocked?.Invoke(this);
        return true;
    }
    public virtual bool IsMeetRequirements() => GetUnlockRequirements()?.TrueForAll(item => item.IsMeetRequirement()) ?? true;
    public virtual bool TryGetUnlockRequirement<T>(out T requirement, Predicate<T> predicate = null) where T : Requirement
    {
        requirement = GetUnlockRequirements()?.Find(item => item is T genericItem && (predicate?.Invoke(genericItem) ?? true)) as T;
        return requirement != null;
    }
    public virtual Requirement GetUnlockRequirementTree() => m_UnlockRequirementTree;
    public virtual List<Requirement> GetUnlockRequirements() => Requirement.GetAvailableLeafRequirements(m_UnlockRequirementTree);
}