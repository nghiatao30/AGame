using UnityEngine;
using System;

namespace LatteGames.Monetization
{
    public class IAPManager : Singleton<IAPManager>, IInAppPurchasing
    {
        [SerializeField] AbstractIAPService mobileService, editorService;
        AbstractIAPService service
        {
            get
            {
#if UNITY_EDITOR
                return editorService;
#else
                return mobileService;
#endif
            }
        }
        public bool HasInitialized { get => service.HasInitialized; }

        protected override void Awake()
        {
            base.Awake();
        }

        public void PurchaseItem(LG_IAPButton _IAPButton)
        {
            service.PurchaseItem(_IAPButton);
        }

        public bool IsPurchased(IAPProductSO productSO)
        {
            return service.IsPurchased(productSO);
        }

        public bool IsConsumable(IAPProductSO productSO)
        {
            return service.IsConsumable(productSO);
        }

        public bool IsNoAdsItem(IAPProductSO productSO)
        {
            return service.IsNoAdsItem(productSO);
        }

        public string GetLocalizedPriceString(IAPProductSO productSO)
        {
            return service.GetLocalizedPriceString(productSO);
        }

        public string GetISOCurrencySymbol(IAPProductSO productSO)
        {
            return service.GetISOCurrencySymbol(productSO);
        }

        public decimal GetPriceInLocalCurrency(IAPProductSO productSO)
        {
            return service.GetPriceInLocalCurrency(productSO);
        }
        public void RestorePurchases(System.Action<bool> onPurchasesRestoredAction)
        {
            service.RestorePurchases(onPurchasesRestoredAction);
        }
    }
}

