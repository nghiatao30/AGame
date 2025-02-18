using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using HyrphusQ.Events;
using I2.Loc;

namespace LatteGames.Monetization
{
    public class LG_IAPButton : ShopBuyButton
    {
        [TitleGroup("Mandatory"), PropertyOrder(1)]
        [SerializeField] CartType cartType;

        public IAPProductSO IAPProductSO { get => (IAPProductSO)shopProductSO; }
        public CartType CartType { get => cartType; set => cartType = value; }

        protected override void OnButtonClicked()
        {
            if (IAPManager.Instance.HasInitialized)
            {
                IAPManager.Instance.PurchaseItem(this);
            }
        }
        protected override void OnEnable()
        {
            if (IAPManager.Instance != null && IAPManager.Instance.HasInitialized)
            {
                base.OnEnable();
            }
            else
            {
                StartCoroutine(CommonCoroutine.WaitUntil(() => IAPManager.Instance != null && IAPManager.Instance.HasInitialized, () =>
                {
                    base.OnEnable();
                }));
            }
        }

        public override void OverrideSetup(ShopProductSO shopProductSO)
        {
            if (IAPManager.Instance != null && IAPManager.Instance.HasInitialized)
            {
                base.OverrideSetup(shopProductSO);
            }
            else
            {
                StartCoroutine(CommonCoroutine.WaitUntil(() => IAPManager.Instance != null && IAPManager.Instance.HasInitialized, () =>
                {
                    base.OverrideSetup(shopProductSO);
                }));
            }
        }

        protected override void CheckButtonInteractive()
        {
            base.CheckButtonInteractive();
            IsInteractiable = IAPManager.Instance.IsConsumable(IAPProductSO) || !IAPProductSO.IsPurchased;
        }
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            GameEventHandler.AddActionEvent(IAPEventCode.OnPurchaseItemCompleted, CheckButtonInteractive);
            GameEventHandler.AddActionEvent(IAPEventCode.OnRestorePurchasesCompleted, CheckButtonInteractive);
        }
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            GameEventHandler.RemoveActionEvent(IAPEventCode.OnPurchaseItemCompleted, CheckButtonInteractive);
            GameEventHandler.RemoveActionEvent(IAPEventCode.OnRestorePurchasesCompleted, CheckButtonInteractive);
        }
        protected override void SetupView()
        {
            base.SetupView();
            if (priceTxt != null)
            {
                if (priceTxt.valueText != null)
                {
                    var localizedPriceString = IAPManager.Instance.GetLocalizedPriceString(this.IAPProductSO);
                    SetText(priceTxt.valueText, String.IsNullOrEmpty(localizedPriceString) ? this.IAPProductSO.defaultPrice : localizedPriceString);
                }
                if (priceTxt.originalValueText != null)
                {
                    var priceString = IAPManager.Instance.GetLocalizedPriceString(this.IAPProductSO);
                    if (String.IsNullOrEmpty(priceString))
                    {
                        SetText(priceTxt.originalValueText, this.IAPProductSO.defaultOriginalPrice);
                    }
                    else
                    {
                        bool symbolAtFront = !Char.IsDigit(priceString[0]);
                        List<Char> currencySymbol = new List<Char>();
                        if (symbolAtFront)
                        {
                            for (var i = 0; i < priceString.Length; i++)
                            {
                                var symbolChar = priceString[i];
                                if (!Char.IsDigit(symbolChar))
                                {
                                    currencySymbol.Add(symbolChar);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (var i = priceString.Length - 1; i >= 0; i--)
                            {
                                var symbolChar = priceString[i];
                                if (!Char.IsDigit(symbolChar))
                                {
                                    currencySymbol.Add(symbolChar);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        var symbol = new string(currencySymbol.ToArray());
                        // var defaultOriginalPriceMatch = Regex.Match(this.IAPProductSO.defaultOriginalPrice, @"([-+]?[0-9]*\.?[0-9]+)");
                        // var defaultPriceMatch = Regex.Match(this.IAPProductSO.defaultPrice, @"([-+]?[0-9]*\.?[0-9]+)");
                        // var defaultOriginalPrice = Convert.ToSingle(defaultOriginalPriceMatch.Groups[1].Value);
                        // var defaultPrice = Convert.ToSingle(defaultPriceMatch.Groups[1].Value);
                        var localizedOriginalPrice = (float)IAPManager.Instance.GetPriceInLocalCurrency(this.IAPProductSO) * this.IAPProductSO.originalPrice / this.IAPProductSO.price;
                        var localizedOriginalPriceString = (localizedOriginalPrice >= 1000) ? localizedOriginalPrice.ToString("N0") : localizedOriginalPrice.ToString("N2");
                        if (symbolAtFront)
                        {
                            SetText(priceTxt.originalValueText, $"{symbol}{localizedOriginalPriceString}");
                        }
                        else
                        {
                            SetText(priceTxt.originalValueText, $"{localizedOriginalPriceString}{symbol}");
                        }
                    }
                }
                if (priceTxt.discountRateText != null)
                {
                    var localizationParamsManager = priceTxt.discountRateText.GetComponent<LocalizationParamsManager>();
                    if (localizationParamsManager != null)
                    {
                        localizationParamsManager.SetParameterValue("PERCENT", IAPProductSO.discountRate.ToString());
                    }
                    else
                    {
                        SetText(priceTxt.discountRateText, $"{IAPProductSO.discountRate}%");
                    }
                }
            }
        }
    }
}
