using System;
using HyrphusQ.Events;
using UnityEngine;
using UnityEngine.UI;

namespace LatteGames.Monetization
{
    public class EditorAdsService : AbstractAdsService
    {
        [SerializeField] private bool isSimulateAdsBehaviour;
        [SerializeField] private GameObject bannerAdsContainer, interstitialAdContainer, rewardedAdContainer;
        [SerializeField] private Button interstitialAdOkayBtn, rewardedAdSuccessBtn, rewardedFailBtn;
        [SerializeField] bool rewardedAdsIsReady = true;
        public override bool HasInitialized => true;
        public override bool IsReadyInterstitial => true;
        public override bool IsReadyRewarded => isSimulateAdsBehaviour ? rewardedAdsIsReady : true;
        public override bool IsReadyBanner => true;

        public override void ShowRewardedAd(AdsLocation location, Action onCompleted, Action onFailed = null, Action<bool> onRVAvailable = null, params string[] parameters)
        {
            GameEventHandler.Invoke(AdvertisingEventCode.OnShowAd, AdsType.Rewarded, location);
            if (isSimulateAdsBehaviour)
            {
                onRVAvailable?.Invoke(rewardedAdsIsReady);
                if (rewardedAdsIsReady)
                {
                    rewardedAdContainer.SetActive(true);
                    rewardedAdSuccessBtn.onClick.AddListener(RewardedAdSuccessBtnClicked);
                    rewardedFailBtn.onClick.AddListener(RewardedAdFailBtnClicked);

                    void RewardedAdSuccessBtnClicked()
                    {
                        rewardedAdContainer.SetActive(false);
                        onCompleted?.Invoke();
                        rewardedAdSuccessBtn.onClick.RemoveListener(RewardedAdSuccessBtnClicked);
                        rewardedFailBtn.onClick.RemoveListener(RewardedAdFailBtnClicked);
                        GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Rewarded, location, true);
                    }

                    void RewardedAdFailBtnClicked()
                    {
                        rewardedAdContainer.SetActive(false);
                        onFailed?.Invoke();
                        rewardedAdSuccessBtn.onClick.RemoveListener(RewardedAdSuccessBtnClicked);
                        rewardedFailBtn.onClick.RemoveListener(RewardedAdFailBtnClicked);
                        GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Rewarded, location, false);
                    }
                }
            }
            else
            {
                rewardedAdContainer.SetActive(false);
            }
        }

        public override void ShowInterstitialAd(AdsLocation location, Action onCompleted = null, params string[] parameters)
        {
            GameEventHandler.Invoke(AdvertisingEventCode.OnShowAd, AdsType.Interstitial, location);
            if (isSimulateAdsBehaviour)
            {
                interstitialAdContainer.SetActive(true);
                interstitialAdOkayBtn.onClick.AddListener(InterstitialAdOkayBtnClicked);

                void InterstitialAdOkayBtnClicked()
                {
                    interstitialAdContainer.SetActive(false);
                    onCompleted?.Invoke();
                    interstitialAdOkayBtn.onClick.RemoveListener(InterstitialAdOkayBtnClicked);
                    GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Interstitial, location, true);
                }
            }
            else
            {
                interstitialAdContainer.SetActive(false);
                onCompleted?.Invoke();
                GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Interstitial, location, false);
            }
        }

        public override void ShowBanner()
        {
            if (isSimulateAdsBehaviour)
            {
                bannerAdsContainer.SetActive(true);
            }
        }

        public override void HideBanner()
        {
            bannerAdsContainer.SetActive(false);
        }
    }
}

