using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LatteGames;
using HyrphusQ.Events;
using HyrphusQ.GUI;

public class CurrencyExchangePopupUI : ComposeCanvasElementVisibilityController
{
    [SerializeField]
    protected Button m_ExchangeButton;
    [SerializeField]
    protected VariableReference<float> m_ExchangeRatio;
    [SerializeField]
    protected TextAdapter m_AmountOfStandardCurrencyText;
    [SerializeField]
    protected TextAdapter m_AmountOfPremiumCurrencyText;
    [SerializeField]
    protected ResourceLocationProvider m_ResourceLocationProvider;

    protected float m_ExchangeAmountOfStandardCurrency;
    protected Action<bool> m_ExchangeResponseCallback;

    protected virtual void Awake()
    {
        m_ExchangeButton.onClick.AddListener(OnExchangeButtonClicked);
        GameEventHandler.AddActionEvent(CurrencyExchangePopupEventCode.OnShowExchangeCurrencyPopupUI, OnShowPopup);
        GameEventHandler.AddActionEvent(CurrencyExchangePopupEventCode.OnHideExchangeCurrencyPopupUI, OnHidePopup);
    }

    protected virtual void OnDestroy()
    {
        m_ExchangeButton.onClick.RemoveListener(OnExchangeButtonClicked);

        GameEventHandler.RemoveActionEvent(CurrencyExchangePopupEventCode.OnShowExchangeCurrencyPopupUI, OnShowPopup);
        GameEventHandler.RemoveActionEvent(CurrencyExchangePopupEventCode.OnHideExchangeCurrencyPopupUI, OnHidePopup);
    }

    protected virtual float CalcRequiredAmountOfPremiumCurrency(float exchangeAmountOfStandardCurrency)
    {
        return Mathf.CeilToInt(exchangeAmountOfStandardCurrency * m_ExchangeRatio.value);
    }

    protected virtual void OnExchangeButtonClicked()
    {
        var premiumCurrencySO = CurrencyManager.Instance.GetCurrencySO(CurrencyType.Premium);
        var standardCurrencySO = CurrencyManager.Instance.GetCurrencySO(CurrencyType.Standard);
        var requiredAmountOfPremiumCurrency = CalcRequiredAmountOfPremiumCurrency(m_ExchangeAmountOfStandardCurrency);
        if (premiumCurrencySO.value >= requiredAmountOfPremiumCurrency)
        {
            // Exchange Premium currency to Standard currency
            premiumCurrencySO.Spend(requiredAmountOfPremiumCurrency, m_ResourceLocationProvider.GetLocation(), m_ResourceLocationProvider.GetItemId());
            standardCurrencySO.Acquire(m_ExchangeAmountOfStandardCurrency, m_ResourceLocationProvider.GetLocation(), m_ResourceLocationProvider.GetItemId());
            m_ExchangeResponseCallback?.Invoke(true);
        }
        else
        {
            m_ExchangeResponseCallback?.Invoke(false);

            // Go to IAP shop
            GameEventHandler.Invoke(IAPEventCode.OnJumpToPremiumCurrencyPackSector);
        }
        GameEventHandler.Invoke(CurrencyExchangePopupEventCode.OnHideExchangeCurrencyPopupUI);
    }

    protected virtual void OnHidePopup()
    {
        Hide();
    }

    protected virtual void OnShowPopup(object[] parameters)
    {
        if (parameters == null || parameters.Length <= 0)
            return;
        m_ExchangeAmountOfStandardCurrency = (float)parameters[0];
        m_ExchangeResponseCallback = parameters.Length >= 2 ? parameters[1] as Action<bool> : null;
        m_AmountOfStandardCurrencyText.SetText(m_AmountOfStandardCurrencyText.blueprintText.Replace(Const.StringValue.PlaceholderValue, m_ExchangeAmountOfStandardCurrency.ToRoundedText()));
        m_AmountOfPremiumCurrencyText.SetText(m_AmountOfPremiumCurrencyText.blueprintText.Replace(Const.StringValue.PlaceholderValue, CalcRequiredAmountOfPremiumCurrency(m_ExchangeAmountOfStandardCurrency).ToRoundedText()));
        Show();
    }
}