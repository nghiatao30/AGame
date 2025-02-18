using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Requirement_Currency : Requirement
{
    [SerializeField]
    protected float m_RequiredAmountOfCurrency;
    [SerializeField]
    protected CurrencyType m_CurrencyType;

    public float requiredAmountOfCurrency
    {
        get => m_RequiredAmountOfCurrency;
        set => m_RequiredAmountOfCurrency = value;
    }
    public float currentAmountOfCurrency
    {
        get => currencySO.value;
    }
    public CurrencyType currencyType
    {
        get => m_CurrencyType;
        set => m_CurrencyType = value;
    }
    public CurrencySO currencySO
    {
        get => CurrencyManager.Instance[currencyType];
    }
    public IResourceLocationProvider resourceLocationProvider { get; set; }

    public override bool IsMeetRequirement()
    {
        return currentAmountOfCurrency >= requiredAmountOfCurrency;
    }

    public override void ExecuteRequirement()
    {
        if (Equals(resourceLocationProvider, null))
            currencySO.SpendWithoutLogEvent(requiredAmountOfCurrency);
        else
            currencySO.Spend(requiredAmountOfCurrency, resourceLocationProvider.GetLocation(), resourceLocationProvider.GetItemId());
    }
}