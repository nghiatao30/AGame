using System;
using LatteGames.Monetization;
using UnityEngine;

namespace LatteGames.Monetization
{
    public interface IInAppPurchasing
    {
        bool HasInitialized { get; }
        void PurchaseItem(LG_IAPButton _IAPButton);
        bool IsPurchased(IAPProductSO productSO);
        bool IsConsumable(IAPProductSO productSO);
        bool IsNoAdsItem(IAPProductSO productSO);
        string GetLocalizedPriceString(IAPProductSO productSO);
        string GetISOCurrencySymbol(IAPProductSO productSO);
        decimal GetPriceInLocalCurrency(IAPProductSO productSO);
        void RestorePurchases(Action<bool> onPurchasesRestoredAction);
    }
}