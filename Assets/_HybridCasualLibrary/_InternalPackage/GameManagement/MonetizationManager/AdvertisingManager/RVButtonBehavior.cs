using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using LatteGames.UI;
using I2.Loc;
using HyrphusQ.Events;

namespace LatteGames.Monetization
{
    public class RVButtonBehavior : MonoBehaviour
    {
        public event Action OnStartWatchAds = delegate { };
        public event Action<RewardGrantedEventData> OnRewardGranted = delegate { };
        public event Action OnFailedWatchAds = delegate { };

        [SerializeField] protected Button button;
        [SerializeField] protected AdsLocation location;
        [SerializeField] protected List<string> parameters = new();
        [SerializeField] protected ResourceLocationProvider ticketSinkLocationProvider;
        public AdsLocation Location
        {
            get => location;
            set
            {
                location = value;
            }
        }
        public List<string> Params
        {
            get => parameters;
            set
            {
                parameters = value;
            }
        }
        public ResourceLocationProvider TicketSinkLocationProvider
        {
            get => ticketSinkLocationProvider;
            set
            {
                ticketSinkLocationProvider = value;
            }
        }

        #region Button Properties
        public bool interactable
        {
            get => button.interactable;
            set => button.interactable = value;
        }
        #endregion

        protected virtual void Awake()
        {
            button.onClick.AddListener(HandleButtonClicked);
        }

        protected virtual void HandleButtonClicked()
        {
            if (CurrencyManager.Instance.IsAffordable(CurrencyType.RVTicket, RVButtonBehaviorConfigs.RV_TICKET_CONVERSION_RATE))
            {
                if (ticketSinkLocationProvider.GetLocation() != ResourceLocation.None && string.IsNullOrEmpty(ticketSinkLocationProvider.GetItemId()))
                {
                    CurrencyManager.Instance.Spend(CurrencyType.RVTicket, RVButtonBehaviorConfigs.RV_TICKET_CONVERSION_RATE, ticketSinkLocationProvider.GetLocation(), ticketSinkLocationProvider.GetItemId());
                }
                else
                {
                    CurrencyManager.Instance.SpendWithoutLogEvent(CurrencyType.RVTicket, RVButtonBehaviorConfigs.RV_TICKET_CONVERSION_RATE);
                }
                OnStartWatchAds();
                GrantReward();
                return;
            }

            if (RVButtonBehaviorConfigs.IS_CHECK_INTERNET && Application.internetReachability == NetworkReachability.NotReachable)
            {
                MessageManager.Title = I2LHelper.TranslateTerm(I2LTerm.RVButtonBehavior_Title_NoConnection);
                MessageManager.Message = I2LHelper.TranslateTerm(I2LTerm.RVButtonBehavior_Desc_NoConnection);
                MessageManager.Show();
                return;
            }

            if (Input.GetKey(KeyCode.R)) // Grant reward when clicked while holding R, for debug purpose
            {
                OnStartWatchAds();
                GrantReward();
                return;
            }

            AdsManager.Instance?.ShowRewardedAd(location,
                () =>
                {
                    GrantReward();
                },
                () =>
                {
                    OnFailedWatchAds?.Invoke();
                },
                onRVAvailable: isAvailable =>
                {
                    if (isAvailable)
                    {
                        OnStartWatchAds?.Invoke();
                        return;
                    }
                    if (RVButtonBehaviorConfigs.IS_SHOW_DEFAULT_RV_NOT_AVAILABLE_NOTICE)
                    {
                        MessageManager.Title = I2LHelper.TranslateTerm(I2LTerm.RVButtonBehavior_Title_Oops);
                        MessageManager.Message = I2LHelper.TranslateTerm(I2LTerm.RVButtonBehavior_Desc_Oops);
                        MessageManager.Show();
                    }
                    GameEventHandler.Invoke(AdvertisingEventCode.OnShowAdNotAvailableNotice);
                },
                parameters: Params.ToArray());
        }
        protected void GrantReward()
        {
            OnRewardGranted(new RewardGrantedEventData());
        }
        public class RewardGrantedEventData
        {

        }
    }
}
