using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LatteGames;
using UnityEngine.Events;

namespace LatteGames.Monetization
{
        public class RestorePurchasesButton : MonoBehaviour
        {
                public UnityEvent OnShow;
                public UnityEvent OnHide;

                [SerializeField] Button button;
                private void Awake()
                {
#if UNITY_IOS
                        gameObject.SetActive(true);
                        button.onClick.AddListener(OnButtonClicked);
                        OnShow?.Invoke();
#else
                        gameObject.SetActive(false);
                        OnHide?.Invoke();
#endif
                }
                private void OnDestroy()
                {
#if UNITY_IOS
        button.onClick.RemoveListener(OnButtonClicked);
#endif
                }
                void OnButtonClicked()
                {
                        IAPManager.Instance.RestorePurchases(null);
                }
        }
}
