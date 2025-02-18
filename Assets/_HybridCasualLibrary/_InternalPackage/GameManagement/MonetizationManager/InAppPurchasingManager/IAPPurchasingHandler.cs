using UnityEngine;
using HyrphusQ.Events;

namespace LatteGames.Monetization
{
    public class IAPPurchasingHandler : MonoBehaviour
    {
        [SerializeField] IAPProductSOContainer _IAPProductSOContainer;
        [SerializeField] PPrefBoolVariable isRemoveAds;
        protected LG_IAPButton _IAPButton;
        private void OnEnable()
        {
            GameEventHandler.AddActionEvent(IAPEventCode.OnProcessPurchase, OnProcessPurchase);
        }
        private void OnDisable()
        {
            GameEventHandler.RemoveActionEvent(IAPEventCode.OnProcessPurchase, OnProcessPurchase);

        }
        protected virtual void OnProcessPurchase(object[] _params)
        {
            string productID = _params[0].ToString();
            _IAPButton = (LG_IAPButton)_params[1];
            bool processFromRestorePurchases = _IAPButton == null;
            var productSO = _IAPProductSOContainer.GetIAPProductSO(productID);
            if (productSO != null && !productSO.IsPurchased)
            {
                HandleProcessPurchase(productSO, processFromRestorePurchases);
            }
        }
        protected virtual void HandleProcessPurchase(IAPProductSO productSO, bool processFromRestorePurchases)
        {
            productSO.IsPurchased = true;
            if (processFromRestorePurchases && productSO.productType == ProductType.Consumable)
            {
                return;
            }
            if (productSO.isRemoveAds)
            {
                isRemoveAds.value = true;
            }
        }
    }
}

