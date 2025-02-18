using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyPunishmentModule : PunishmentModule
{
    [SerializeField]
    protected float m_Amount;
    [SerializeField]
    protected CurrencyType m_CurrencyType;

    public float amount
    {
        get => m_Amount;
        set => m_Amount = value;
    }
    public CurrencyType currencyType
    {
        get => m_CurrencyType;
        set => m_CurrencyType = value;
    }

    public override void TakePunishment()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance[currencyType].SpendWithoutLogEvent(amount);
        }
    }
}