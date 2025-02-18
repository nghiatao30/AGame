using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HyrphusQ.Events;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using HyrphusQ.SerializedDataStructure;
using System;
using UnityEngine.Events;
using I2.Loc;

public class CurrencyBuyButton : ShopBuyButton
{
    public Action<bool> OnPurchaseItem;
    [TitleGroup("Mandatory"), PropertyOrder(1)]
    public ResourceLocationProvider sinkLocationProvider;

    public CurrencyProductSO currencyProductSO => (CurrencyProductSO)shopProductSO;
    CurrencySO currencySO => CurrencyManager.Instance?.GetCurrencySO(currencyProductSO.currencyType);
    public override bool IsInteractiable
    {
        get
        {
            return button.interactable;
        }
        set
        {
            if (IsLockBtn)
            {
                return;
            }
            OnInteractiveChanged?.Invoke(value);
        }
    }
    protected override void SetupView()
    {
        base.SetupView();
        if (priceTxt != null)
        {
            if (priceTxt.valueText != null)
            {
                if (currencyProductSO.price <= 0)
                {
                    SetText(priceTxt.valueText, I2LHelper.TranslateTerm(I2LTerm.RVButtonBehavior_Title_Free));
                }
                else
                {
                    SetText(priceTxt.valueText, $"{currencyProductSO.price.ToRoundedText()}");
                }
            }
            if (priceTxt.originalValueText != null)
            {
                SetText(priceTxt.originalValueText, currencyProductSO.originalPrice.ToRoundedText());
            }
            if (priceTxt.discountRateText != null)
            {
                var localizationParamsManager = priceTxt.discountRateText.GetComponent<LocalizationParamsManager>();
                if (localizationParamsManager != null)
                {
                    localizationParamsManager.SetParameterValue("PERCENT", currencyProductSO.discountRate.ToString());
                }
                else
                {
                    SetText(priceTxt.discountRateText, $"{currencyProductSO.discountRate}%");
                }
            }
        }
    }
    protected override void OnButtonClicked()
    {
        if (!CurrencyManager.Instance.Spend(currencyProductSO.currencyType, Mathf.RoundToInt(currencyProductSO.price), sinkLocationProvider.GetLocation(), sinkLocationProvider.GetItemId()))
        {
            GameEventHandler.Invoke(EconomyEventCode.OnPurchaseUnaffordableItem, this);
            OnPurchaseItem?.Invoke(false);
        }
        else
        {
            GameEventHandler.Invoke(EconomyEventCode.OnPurchaseItemCompleted, this);
            OnPurchaseItem?.Invoke(true);
        }
    }
    protected override void SubscribeEvents()
    {
        base.SubscribeEvents();
        currencySO.onValueChanged += HandleIncomeChanged;
    }
    protected override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        if (currencySO != null)
        {
            currencySO.onValueChanged -= HandleIncomeChanged;
        }
    }
    private void HandleIncomeChanged(ValueDataChanged<float> eventData)
    {
        CheckButtonInteractive();
    }
    protected override void CheckButtonInteractive()
    {
        base.CheckButtonInteractive();
        IsInteractiable = CurrencyManager.Instance.IsAffordable(currencyProductSO.currencyType, currencyProductSO.price);
    }
}
