using UnityEngine;
using System;
using HyrphusQ.Events;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace LatteGames.Monetization
{
    [OptionalDependency("IronSource", "LatteGames_IronSource")]
    public class IronSourceAdsService : AbstractMobileAdsService
    {
#if LatteGames_IronSource
        #region Properties
        [Title("Custom Fields")]
        [SerializeField] IronSourceBannerPosition bannerPosition = IronSourceBannerPosition.BOTTOM;
        public override bool IsReadyInterstitial => IronSource.Agent.isInterstitialReady();
        public override bool IsReadyRewarded => IronSource.Agent.isRewardedVideoAvailable();
        public override bool IsReadyBanner => isLoadBannerSuccess;
        Action onCompleted;
        Action onFailed;
        bool isLoadBannerSuccess;
        AdsLocation currentLocation;
        #endregion

        protected override void Initialize()
        {
            //Add Init Event
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;

            //Add ImpressionSuccess Event
            IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;

            //Add AdInfo Rewarded Video Events
            IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
            IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
            IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;

            //Add AdInfo Interstitial Events
            IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
            IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
            IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
            IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
            IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;

            //Add AdInfo Banner Events
            IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
            IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
            IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
            IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
            IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
            IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;

            Debug.Log("unity-script: IronSource.Agent.validateIntegration");
            IronSource.Agent.validateIntegration();

            Debug.Log("unity-script: unity version" + IronSource.unityVersion());

            // SDK init
            Debug.Log("unity-script: IronSource.Agent.init");
            IronSource.Agent.init(Key);
        }

        public override void ShowRewardedAd(
            AdsLocation location,
            Action onCompleted,
            Action onFailed = null,
            Action<bool> onRVAvailable = null,
            params string[] parameters)
        {
            if (IsReadyRewarded)
            {
                this.onCompleted = onCompleted;
                this.onFailed = onFailed;
                onRVAvailable?.Invoke(true);
                currentLocation = location;
                IronSource.Agent.showRewardedVideo(Placements.Rewarded);
            }
            else
            {
                onRVAvailable?.Invoke(false);
                IronSource.Agent.loadRewardedVideo();
            }
        }

        public override void ShowInterstitialAd(AdsLocation location, Action onCompleted = null, params string[] parameters)
        {
            if (IsReadyInterstitial)
            {
                this.onCompleted = onCompleted;
                currentLocation = location;
                IronSource.Agent.showInterstitial(Placements.Interstitial);
            }
            else
            {
                onCompleted?.Invoke();
                IronSource.Agent.loadInterstitial();
            }
        }
        public override void ShowBanner()
        {
            // Show the loaded Banner Ad Unit:
            IronSource.Agent.displayBanner();
        }

        public override void HideBanner()
        {
            IronSource.Agent.destroyBanner();
        }

        #region Init callback handlers

        void SdkInitializationCompletedEvent()
        {
            Debug.Log("unity-script: I got SdkInitializationCompletedEvent");
            Initialized = true;
            if (Placements.LoadRewarded && Placements.Rewarded != "") IronSource.Agent.loadRewardedVideo();
            if (!IsRemoveAds.value)
            {
                if (Placements.LoadInterstitial && Placements.Interstitial != "") IronSource.Agent.loadInterstitial();
                if (Placements.LoadBanner && Placements.Banner != "")
                {
                    IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, bannerPosition);
                }
            }
        }

        #endregion

        #region AdInfo Rewarded Video
        void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got RewardedVideoOnAdOpenedEvent With AdInfo " + adInfo);
            GameEventHandler.Invoke(AdvertisingEventCode.OnShowAd, AdsType.Rewarded, currentLocation);
        }

        void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got RewardedVideoOnAdClosedEvent With AdInfo " + adInfo);
        }

        void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got RewardedVideoOnAdAvailable With AdInfo " + adInfo);
        }

        void RewardedVideoOnAdUnavailable()
        {
            Debug.Log("unity-script: I got RewardedVideoOnAdUnavailable");
        }

        void RewardedVideoOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent With Error" + ironSourceError + "And AdInfo " + adInfo);
            GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Rewarded, currentLocation, false);
            onFailed?.Invoke();
        }

        void RewardedVideoOnAdRewardedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got RewardedVideoOnAdRewardedEvent With Placement" + ironSourcePlacement + "And AdInfo " + adInfo);
            GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Rewarded, currentLocation, true);
            onCompleted?.Invoke();
            if (Placements.LoadRewarded && Placements.Rewarded != "") IronSource.Agent.loadRewardedVideo();
        }

        void RewardedVideoOnAdClickedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got RewardedVideoOnAdClickedEvent With Placement" + ironSourcePlacement + "And AdInfo " + adInfo);
        }

        #endregion

        #region AdInfo Interstitial

        void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got InterstitialOnAdReadyEvent With AdInfo " + adInfo);
        }

        void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
        {
            Debug.Log("unity-script: I got InterstitialOnAdLoadFailed With Error " + ironSourceError);
        }

        void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got InterstitialOnAdOpenedEvent With AdInfo " + adInfo);
        }

        void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got InterstitialOnAdClickedEvent With AdInfo " + adInfo);
        }

        void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got InterstitialOnAdShowSucceededEvent With AdInfo " + adInfo);
            GameEventHandler.Invoke(AdvertisingEventCode.OnShowAd, AdsType.Interstitial, currentLocation);
        }

        void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got InterstitialOnAdShowFailedEvent With Error " + ironSourceError + " And AdInfo " + adInfo);
            GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Interstitial, currentLocation, false);
            onFailed?.Invoke();
        }

        void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got InterstitialOnAdClosedEvent With AdInfo " + adInfo);
            GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Interstitial, currentLocation, true);
            onCompleted?.Invoke();
            if (!IsRemoveAds.value)
            {
                if (Placements.LoadInterstitial && Placements.Interstitial != "") IronSource.Agent.loadInterstitial();
            }
        }

        #endregion

        #region Banner AdInfo

        void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got BannerOnAdLoadedEvent With AdInfo " + adInfo);
            isLoadBannerSuccess = true;
        }

        void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
        {
            Debug.Log("unity-script: I got BannerOnAdLoadFailedEvent With Error " + ironSourceError);
            isLoadBannerSuccess = false;
        }

        void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got BannerOnAdClickedEvent With AdInfo " + adInfo);
        }

        void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got BannerOnAdScreenPresentedEvent With AdInfo " + adInfo);
        }

        void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got BannerOnAdScreenDismissedEvent With AdInfo " + adInfo);
        }

        void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("unity-script: I got BannerOnAdLeftApplicationEvent With AdInfo " + adInfo);
        }

        #endregion

        #region ImpressionSuccess callback handler

        void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            Debug.Log("unity - script: I got ImpressionDataReadyEvent ToString(): " + impressionData.ToString());
            Debug.Log("unity - script: I got ImpressionDataReadyEvent allData: " + impressionData.allData);
        }
        #endregion

        void OnApplicationPause(bool isPaused)
        {
            Debug.Log("unity-script: OnApplicationPause = " + isPaused);
            IronSource.Agent.onApplicationPause(isPaused);
        }

#else
        protected override void Initialize()
        {
        }
#endif
    }
}

