using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// GachaItemSO represents for item that can be upgraded (Gacha mechanics).
/// <para> Ex: Collect some resources (usually card,...) to upgrade this item. </para>
/// </summary>
public abstract class GachaItemSO : ItemSO
{
    #region Fields
    [SerializeField, BoxGroup("GachaItem")]
    protected int m_FoundInArena = 1;
    #endregion

    #region Properties
    public virtual int foundInArena { get => m_FoundInArena; set => m_FoundInArena = value; }
    #endregion

    #region Methods

    #endregion

    #region Editor Methods
#if UNITY_EDITOR
    [ButtonGroup, Button(SdfIconType.Plus, "Add 1 Card"), PropertyOrder(-1)]
    private void Add1Card()
    {
        this.UpdateNumOfCards(this.GetNumOfCards() + 10);
    }
    [ButtonGroup, Button(SdfIconType.Plus, "Add 10 Card"), PropertyOrder(-1)]
    private void Add10Card()
    {
        this.UpdateNumOfCards(this.GetNumOfCards() + 10);
    }
    [ButtonGroup, Button(SdfIconType.Plus, "Add 100 Card"), PropertyOrder(-1)]
    private void Add100Card()
    {
        this.UpdateNumOfCards(this.GetNumOfCards() + 100);
    }
    [ButtonGroup, Button(SdfIconType.ArrowUp, "Update 1 level"), PropertyOrder(-1)]
    private void Upgrade1Level()
    {
        this.TryUpgradeIgnoreRequirement();
    }
    [ButtonGroup, Button(SdfIconType.Unlock, "Unlock Item"), PropertyOrder(-1)]
    private void UnlockItem()
    {
        this.TryUnlockIgnoreRequirement();
    }
    [ButtonGroup, Button(SdfIconType.Newspaper, "Set New Item"), PropertyOrder(-1)]
    private void SetItemNew()
    {
        this.SetNewItem(true);
    }
#endif
    #endregion
}