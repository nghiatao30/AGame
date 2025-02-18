using UnityEngine;
using System;
using HyrphusQ.Events;
using System.Text.RegularExpressions;
using System.Globalization;

namespace LatteGames.Monetization
{
    public abstract class AbstractIAPService : MonoBehaviour, IInAppPurchasing
    {
        [SerializeField] protected InternalLogger logger;
        protected bool hasInitialized;
        public virtual bool HasInitialized => hasInitialized;

        public virtual void PurchaseItem(LG_IAPButton _IAPButton)
        {
            GameEventHandler.Invoke(IAPEventCode.OnPurchaseItemStarted);
            GameEventHandler.Invoke(IAPEventCode.OnProcessPurchase, _IAPButton.IAPProductSO.itemID, _IAPButton);
            GameEventHandler.Invoke(IAPEventCode.OnPurchaseItemCompleted, _IAPButton);
        }

        public virtual bool IsPurchased(IAPProductSO productSO)
        {
            return productSO.IsPurchased;
        }

        public virtual bool IsConsumable(IAPProductSO productSO)
        {
            return productSO.productType == ProductType.Consumable;
        }

        public virtual bool IsNoAdsItem(IAPProductSO productSO)
        {
            return false;
        }

        public virtual string GetLocalizedPriceString(IAPProductSO productSO)
        {
            return productSO.defaultPrice;
        }

        public virtual string GetISOCurrencySymbol(IAPProductSO productSO)
        {
            return "$";
        }

        public virtual decimal GetPriceInLocalCurrency(IAPProductSO productSO)
        {
            string numericString = Regex.Replace(GetLocalizedPriceString(productSO), @"[^\d.,]", "");

            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            if (decimal.TryParse(numericString, NumberStyles.Any, cultureInfo, out decimal result))
            {
                return result;
            }
            else
            {
                throw new FormatException("Can't convert the price string");
            }
        }
        public virtual void RestorePurchases(System.Action<bool> onPurchasesRestoredAction)
        {
            Debug.Log("IAP_Debug:: RestorePurchases");
        }
    }
}

