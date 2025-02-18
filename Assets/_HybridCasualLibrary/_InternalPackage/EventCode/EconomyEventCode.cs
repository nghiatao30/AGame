using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EventCode]
public enum EconomyEventCode
{
    /// <summary>
    /// This event is raised when consuming currency
    /// <para> <typeparamref name="CurrencyType"/>: currencyType </para>
    /// <para> <typeparamref name="float"/>: amount </para>
    /// <para> <typeparamref name="ResourceLocation"/>: location </para>
    /// <para> <typeparamref name="string"/>: itemId </para>
    /// </summary>
    ConsumeResource,
    /// <summary>
    /// This event is raised when acquiring currency
    /// <para> <typeparamref name="CurrencyType"/>: currencyType </para>
    /// <para> <typeparamref name="float"/>: amount </para>
    /// <para> <typeparamref name="ResourceLocation"/>: location </para>
    /// <para> <typeparamref name="string"/>: itemId </para>
    /// </summary>
    AcquireResource,
    /// <summary>
    /// This event is raised when the economy product is purchased
    /// <para> <typeparamref name="CurrencyBuyButton"/>: currencyBuyButton </para>
    /// </summary>
    OnPurchaseItemCompleted,
    /// <summary>
    /// This event is raised when purchasing the unaffordable economy product
    /// <para> <typeparamref name="CurrencyBuyButton"/>: currencyBuyButton </para>
    /// </summary>
    OnPurchaseUnaffordableItem
}