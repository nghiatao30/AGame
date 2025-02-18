using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HyrphusQ.Events;
using Sirenix.OdinInspector;
using UnityEngine;
#if LatteGames_UnityIAP
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;
#endif

namespace LatteGames.Monetization
{
    [OptionalDependency("UnityEngine.Purchasing.IStoreListener", "LatteGames_UnityIAP")]
#if LatteGames_UnityIAP
    public class UnityIAPService : AbstractIAPService, IDetailedStoreListener
    {
        [SerializeField] IAPProductSOContainer _IAPProductSOContainer;
        private IStoreController storeController;
        private IExtensionProvider extensionProvider;
        public override bool HasInitialized => storeController != null && extensionProvider != null;
        List<LG_IAPButton> purchasingButtons = new List<LG_IAPButton>();
        private void Awake()
        {
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Add all the product ids to the builder
            for (int i = 0; i < _IAPProductSOContainer.list.Count; i++)
            {
                var productInfo = _IAPProductSOContainer.list[i];
                builder.AddProduct(productInfo.itemID, ConvertProductType(productInfo.productType));
            }

            logger.Log($"Initializing IAP now...");

            UnityPurchasing.Initialize(this, builder);
        }
        async void Start()
        {
            try
            {
                var options = new Unity.Services.Core.InitializationOptions();

                await Unity.Services.Core.UnityServices.InitializeAsync(options);
            }
            catch (Exception exception)
            {
                // An error occurred during services initialization.
                logger.Log($"Initialization Game Services failed! Reason: {exception}");
            }
        }


        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            extensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            logger.Log($"Initialization failed! Reason: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            logger.Log($"Initialization failed! Reason: {error}, message: {message}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {

        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            logger.Log($"Purchase failed for product id: {product.definition.id}, reason: {failureDescription}");
            GameEventHandler.Invoke(IAPEventCode.OnPurchaseItemFailed);
            var purchasingButton = purchasingButtons.Find(item => item.IAPProductSO.itemID == product.definition.id);
            purchasingButtons.Remove(purchasingButton);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            bool validPurchase = true; // Presume valid for platforms with no R.V.

            // Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
            // Prepare the validator with the secrets we prepared in the Editor
            // obfuscation window.
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                AppleTangle.Data(), Application.identifier);

            try
            {
                // // On Google Play, result has a single product ID.
                // // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);
                // // For informational purposes, we list the receipt(s)
                Debug.Log("Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    Debug.Log(productReceipt.productID);
                    Debug.Log(productReceipt.purchaseDate);
                    Debug.Log(productReceipt.transactionID);
                }
            }
            catch (IAPSecurityException)
            {
                Debug.Log("Invalid receipt, not unlocking content");
                validPurchase = false;
            }
#endif

            if (validPurchase)
            {
                // Unlock the appropriate content here.
                Product product = purchaseEvent.purchasedProduct;

                logger.Log($"Purchase successful for product id: {product.definition.id}");
                var purchasingButton = purchasingButtons.Find(item => item.IAPProductSO.itemID == product.definition.id);
                GameEventHandler.Invoke(IAPEventCode.OnProcessPurchase, product.definition.id, purchasingButton);
                if (purchasingButton != null)
                {
                    GameEventHandler.Invoke(IAPEventCode.OnPurchaseItemCompleted, purchasingButton);
                    purchasingButtons.Remove(purchasingButton);
                }
            }

            return PurchaseProcessingResult.Complete;
        }

        public override void PurchaseItem(LG_IAPButton _IAPButton)
        {
            if (HasInitialized)
            {
                Product product = storeController.products.WithID(_IAPButton.IAPProductSO.itemID);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product == null)
                {
                    logger.Log("BuyProduct: product with id \"" + _IAPButton.IAPProductSO.itemID + "\" does not exist.");
                }
                else if (!product.availableToPurchase)
                {
                    logger.Log("BuyProduct: product with id \"" + _IAPButton.IAPProductSO.itemID + "\" is not available to purchase.");
                }
                else if (purchasingButtons.Contains(_IAPButton))
                {
                    logger.Log("BuyProduct: product with id \"" + _IAPButton.IAPProductSO.itemID + "\" is processing.");
                }
                else
                {
                    storeController.InitiatePurchase(product);
                    purchasingButtons.Add(_IAPButton);
                    GameEventHandler.Invoke(IAPEventCode.OnPurchaseItemStarted);
                }
            }
            else
            {
                logger.Log("BuyProduct: IAPManager not initialized.");
            }
        }

        public override bool IsPurchased(IAPProductSO productSO)
        {
            Product product = storeController.products.WithID(productSO.itemID);
            return product.hasReceipt;
        }

        public override bool IsConsumable(IAPProductSO productSO)
        {
            return productSO.productType == ProductType.Consumable;
        }

        public override string GetLocalizedPriceString(IAPProductSO productSO)
        {
            Product product = storeController.products.WithID(productSO.itemID);
            return product.metadata.localizedPriceString;
        }

        public override decimal GetPriceInLocalCurrency(IAPProductSO productSO)
        {
            Product product = storeController.products.WithID(productSO.itemID);
            return product.metadata.localizedPrice;
        }

        public override string GetISOCurrencySymbol(IAPProductSO productSO)
        {
            Product product = storeController.products.WithID(productSO.itemID);
            return product.metadata.isoCurrencyCode;
        }
        public override void RestorePurchases(System.Action<bool> onPurchasesRestoredAction)
        {
            logger.Log("IAP_logger:: RestorePurchases");
            if (HasInitialized)
            {
                if ((Application.platform == RuntimePlatform.IPhonePlayer ||
                     Application.platform == RuntimePlatform.OSXPlayer))
                {
                    extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((result, error) =>
                    {
                        if (result)
                        {
                            GameEventHandler.Invoke(IAPEventCode.OnRestorePurchasesCompleted);
                        }
                        else
                        {
                            logger.Log($"Restore purchases failed! Reason: {error}");
                        }
                        onPurchasesRestoredAction?.Invoke(result);
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
        public UnityEngine.Purchasing.ProductType ConvertProductType(ProductType productType)
        {
            return (UnityEngine.Purchasing.ProductType)(int)productType;
        }
    }
#else
    public class UnityIAPService : AbstractIAPService
    {

    }
#endif
}
