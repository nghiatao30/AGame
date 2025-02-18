using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HyrphusQ.Events;
using Sirenix.OdinInspector;
using UnityEngine;
#if LatteGames_CLIK
using Tabtale.TTPlugins;
#endif

namespace LatteGames.Monetization
{
    [OptionalDependency("Tabtale.TTPlugins.TTPCore", "LatteGames_CLIK")]
#if LatteGames_CLIK
    public class CLIKIAPService : AbstractIAPService
    {
        [SerializeField] IAPProductSOContainer _IAPProductSOContainer;
        public override bool HasInitialized => hasInitialized;
        List<LG_IAPButton> purchasingButtons = new List<LG_IAPButton>();
        void Start()
        {
            TTPBilling.OnBillingInitEvent += TTPBilling_OnBillingInitEvent;
            TTPBilling.OnItemPurchasedEvent += TTPBilling_OnItemPurchasedEvent;
            TTPCore.Setup();
        }

        // Called when initialization complete
        private void TTPBilling_OnBillingInitEvent(BillerErrors billerErrors)
        {
            if (billerErrors == BillerErrors.NO_ERROR)
            {
                hasInitialized = true;
            }
            else
            {
                Debug.LogError("Billing failed to init. Error = " + billerErrors);
            }
        }

        // Called when item is purchased or restored
        private void TTPBilling_OnItemPurchasedEvent(PurchaseIAPResult purchaseIAPResult)
        {
            if (purchaseIAPResult.result == PurchaseIAPResultCode.Success)
            {
                logger.Log($"Purchase successful for product id: {purchaseIAPResult.purchasedItem.id}");
                var purchasingButton = purchasingButtons.Find(item => item.IAPProductSO.itemID == purchaseIAPResult.purchasedItem.id);
                GameEventHandler.Invoke(IAPEventCode.OnProcessPurchase, purchaseIAPResult.purchasedItem.id, purchasingButton);
                if (purchasingButton != null)
                {
                    GameEventHandler.Invoke(IAPEventCode.OnPurchaseItemCompleted, purchasingButton);
                    purchasingButtons.Remove(purchasingButton);
                }
            }
            else if (purchaseIAPResult.result == PurchaseIAPResultCode.Failed)
            {
                logger.Log($"Purchase failed for product id: {purchaseIAPResult.purchasedItem.id}, reason: {purchaseIAPResult.error}");
                GameEventHandler.Invoke(IAPEventCode.OnPurchaseItemFailed);
                var purchasingButton = purchasingButtons.Find(item => item.IAPProductSO.itemID == purchaseIAPResult.purchasedItem.id);
                purchasingButtons.Remove(purchasingButton);
            }
            else if (purchaseIAPResult.result == PurchaseIAPResultCode.Cancelled)
            {
                logger.Log($"Purchase cancelled for product id: {purchaseIAPResult.purchasedItem.id}");
                GameEventHandler.Invoke(IAPEventCode.OnPurchaseItemCanceled);
                var purchasingButton = purchasingButtons.Find(item => item.IAPProductSO.itemID == purchaseIAPResult.purchasedItem.id);
                purchasingButtons.Remove(purchasingButton);
            }
        }

        public override void PurchaseItem(LG_IAPButton _IAPButton)
        {
            if (HasInitialized)
            {
                TTPBilling.PurchaseItem(_IAPButton.IAPProductSO.itemID, TTPBilling_OnItemPurchasedEvent);

                purchasingButtons.Add(_IAPButton);
                GameEventHandler.Invoke(IAPEventCode.OnPurchaseItemStarted);
            }
            else
            {
                logger.Log("BuyProduct: IAPManager not initialized.");
            }
        }

        public override bool IsPurchased(IAPProductSO productSO)
        {
            return TTPBilling.IsPurchased(productSO.itemID);
        }

        public override bool IsConsumable(IAPProductSO productSO)
        {
            return TTPBilling.IsConsumable(productSO.itemID);
        }

        public override decimal GetPriceInLocalCurrency(IAPProductSO productSO)
        {
            return TTPBilling.GetPriceInLocalCurrency(productSO.itemID);
        }

        public override string GetLocalizedPriceString(IAPProductSO productSO)
        {
            return TTPBilling.GetLocalizedPriceString(productSO.itemID);
        }

        public override string GetISOCurrencySymbol(IAPProductSO productSO)
        {
            return TTPBilling.GetISOCurrencySymbol(productSO.itemID);
        }
        public override void RestorePurchases(System.Action<bool> onPurchasesRestoredAction)
        {
            logger.Log("IAP_logger:: RestorePurchases");
            if (HasInitialized)
            {
                if ((Application.platform == RuntimePlatform.IPhonePlayer ||
                     Application.platform == RuntimePlatform.OSXPlayer))
                {
                    TTPBilling.RestorePurchases((isSuccess) =>
                    {
                        if (isSuccess)
                        {
                            GameEventHandler.Invoke(IAPEventCode.OnRestorePurchasesCompleted);
                        }
                        else
                        {
                            logger.Log($"Restore purchases failed!");
                        }
                        onPurchasesRestoredAction?.Invoke(isSuccess);
                    });
                }
                else
                {
                    logger.Log("RestorePurchases: Device is not iOS, no need to call this method.");
                }
            }
            else
            {
                logger.Log("RestorePurchases: IAPManager not initialized.");
            }
        }
    }
#else
    public class CLIKIAPService : AbstractIAPService
    {

    }
#endif
}
