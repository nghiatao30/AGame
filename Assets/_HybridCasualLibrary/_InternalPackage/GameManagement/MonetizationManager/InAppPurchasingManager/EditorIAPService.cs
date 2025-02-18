using System;
using System.Collections;
using System.Collections.Generic;
using HyrphusQ.Events;
using UnityEngine;
using UnityEngine.UI;

namespace LatteGames.Monetization
{
    public class EditorIAPService : AbstractIAPService
    {
        [SerializeField] private bool isSimulateAdsBehaviour;
        [SerializeField] private Button buyBtn, cancelBtn;
        [SerializeField] private GameObject IAPPanel;
        public override bool HasInitialized => true;

        public override void PurchaseItem(LG_IAPButton _IAPButton)
        {
            if (isSimulateAdsBehaviour)
            {
                IAPPanel.SetActive(true);
                buyBtn.onClick.AddListener(OnBuyClicked);
                cancelBtn.onClick.AddListener(OnCancelClicked);
                GameEventHandler.Invoke(IAPEventCode.OnPurchaseItemStarted);
                void OnBuyClicked()
                {
                    buyBtn.onClick.RemoveListener(OnBuyClicked);
                    cancelBtn.onClick.RemoveListener(OnCancelClicked);
                    IAPPanel.SetActive(false);
                    GameEventHandler.Invoke(IAPEventCode.OnProcessPurchase, _IAPButton.IAPProductSO.itemID, _IAPButton);
                    GameEventHandler.Invoke(IAPEventCode.OnPurchaseItemCompleted, _IAPButton);
                }
                void OnCancelClicked()
                {
                    buyBtn.onClick.RemoveListener(OnBuyClicked);
                    cancelBtn.onClick.RemoveListener(OnCancelClicked);
                    IAPPanel.SetActive(false);
                    GameEventHandler.Invoke(IAPEventCode.OnPurchaseItemFailed);
                }
            }
            else
            {
                base.PurchaseItem(_IAPButton);
            }
        }
    }
}
