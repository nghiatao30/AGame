using UnityEngine;
using HyrphusQ.Events;

namespace LatteGames.Monetization
{
    public class CurrencyProductPurchasingHandler : MonoBehaviour
    {
        protected CurrencyBuyButton currencyBuyButton;
        private void OnEnable()
        {
            GameEventHandler.AddActionEvent(EconomyEventCode.OnPurchaseItemCompleted, OnProcessPurchase);
        }
        private void OnDisable()
        {
            GameEventHandler.RemoveActionEvent(EconomyEventCode.OnPurchaseItemCompleted, OnProcessPurchase);

        }
        protected virtual void OnProcessPurchase(object[] _params)
        {
            currencyBuyButton = (CurrencyBuyButton)_params[0];
            currencyBuyButton.currencyProductSO.IsPurchased = true;
        }
    }
}

