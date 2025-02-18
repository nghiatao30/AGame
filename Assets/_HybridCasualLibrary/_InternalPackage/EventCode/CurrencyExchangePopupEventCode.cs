using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EventCode]
public enum CurrencyExchangePopupEventCode
{
    /// <summary>
    /// This event is raised to show currency exchange popup UI
    /// <para> <typeparamref name="float"/>: exchangeAmountOfMoney </para>
    /// <para> <typeparamref name="Action&lt;bool&gt;"/>: exchangeResponseCallback </para>
    /// </summary>
    OnShowExchangeCurrencyPopupUI,
    /// <summary>
    /// This event is raised to hide currency exchange popup UI
    /// </summary>
    OnHideExchangeCurrencyPopupUI
}