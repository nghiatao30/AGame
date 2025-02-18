using System;
using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Helpers;
using UnityEngine;

public enum RarityType
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythic,
}
[Serializable, CustomInspectorName("RarityModule")]
public class RarityItemModule : ItemModule
{
    [SerializeField]
    protected RarityType m_RarityType;

    public virtual RarityType rarityType
    {
        get => m_RarityType;
        set => m_RarityType = value;
    }
}