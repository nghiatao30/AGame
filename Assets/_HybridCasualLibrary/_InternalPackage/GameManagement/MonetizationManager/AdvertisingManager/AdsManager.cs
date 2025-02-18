using UnityEngine;
using System;
using System.Collections.Generic;
using HyrphusQ.Events;

namespace LatteGames.Monetization
{
    public enum AdsType
    {
        Banner,
        Interstitial,
        Rewarded
    }
    public class AdsManager : Singleton<AdsManager>, IAdvertising
    {
        [SerializeField] AbstractAdsService mobileAdsService, editorAdsService;
        AbstractAdsService adsService
        {
            get
            {
#if UNITY_EDITOR
                return editorAdsService;
#else
                return mobileAdsService;
#endif
            }
        }
        [SerializeField] bool isUseBannerAd;
        [SerializeField] protected PPrefBoolVariable isRemoveAds;
        public bool HasInitialized { get => adsService.HasInitialized; }
        public bool IsReadyInterstitial { get => adsService.IsReadyInterstitial; }
        public bool IsReadyRewarded { get => adsService.IsReadyRewarded; }
        public bool IsReadyBanner { get => adsService.IsReadyBanner; }
        bool isWatchingAd;
        float originTimeScale = 1f;

        protected override void Awake()
        {
            base.Awake();
            adsService.IsRemoveAds = isRemoveAds;
            if (isUseBannerAd)
            {
                StartCoroutine(CommonCoroutine.WaitUntil(() => adsService.IsReadyBanner, () =>
                {
                    ShowBanner();
                }));
            }
        }
        private void OnEnable()
        {
            GameEventHandler.AddActionEvent(AdvertisingEventCode.OnCloseAd, OnCloseAd);
            GameEventHandler.AddActionEvent(AdvertisingEventCode.OnShowAd, OnShowAd);
            isRemoveAds.onValueChanged += OnRemoveAdsPurchased;
        }

        private void OnDisable()
        {
            GameEventHandler.AddActionEvent(AdvertisingEventCode.OnCloseAd, OnCloseAd);
            GameEventHandler.AddActionEvent(AdvertisingEventCode.OnShowAd, OnShowAd);
            isRemoveAds.onValueChanged -= OnRemoveAdsPurchased;
        }

        public void RemoveAds()
        {
            isRemoveAds.value = true;
            HideBanner();
        }

        public void ShowInterstitialAd(AdsLocation location, Action onCompleted = null, params string[] parameters)
        {
            if (isRemoveAds.value)
            {
                return;
            }
            adsService.ShowInterstitialAd(location, onCompleted, parameters);
        }

        public void ShowRewardedAd(AdsLocation location, Action onCompleted, Action onFailed = null, Action<bool> onRVAvailable = null, params string[] parameters)
        {
            adsService.ShowRewardedAd(location, onCompleted, onFailed, onRVAvailable, parameters);
        }

        protected virtual void OnRemoveAdsPurchased(ValueDataChanged<bool> data)
        {
            if (data.oldValue == false && data.newValue == true)
            {
                RemoveAds();
            }
        }

        public void ShowBanner()
        {
            if (isRemoveAds.value || !isUseBannerAd)
            {
                return;
            }
            adsService.ShowBanner();
        }

        public void HideBanner()
        {
            adsService.HideBanner();
        }
        void OnCloseAd(object[] _params)
        {
            PauseApp(false);
        }
        void OnShowAd(object[] _params)
        {
            PauseApp(true);
        }

        void PauseApp(bool isPause)
        {
            isWatchingAd = isPause;

#if MOREMOUNTAINS_NICEVIBRATIONS
            MoreMountains.NiceVibrations.MMVibrationManager.StopAllHaptics(true);
#endif
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
            if (isPause)
            {
                Lofelt.NiceVibrations.HapticController.Stop();
                // release haptic binding
                Lofelt.NiceVibrations.LofeltHaptics.Release();
            }
            else
            {
                // initializing haptic binding
                Lofelt.NiceVibrations.LofeltHaptics.Initialize();
            }
#endif
            AudioListener.pause = isPause;
            if (isPause)
            {
                originTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
                Time.timeScale = originTimeScale;
            Physics.autoSimulation = !isPause;
        }
    }
}