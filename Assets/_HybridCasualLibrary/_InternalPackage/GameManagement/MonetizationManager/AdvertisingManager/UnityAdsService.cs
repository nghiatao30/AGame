using UnityEngine;
using System;
using HyrphusQ.Events;
using UnityEngine.Advertisements;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace LatteGames.Monetization
{
    [OptionalDependency("UnityEngine.Advertisements.Advertisement", "LatteGames_UnityAds")]
    public class UnityAdsService : AbstractMobileAdsService
#if LatteGames_UnityAds
    , IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
#endif
    {
#if LatteGames_UnityAds
        #region Properties
        [Title("Custom Fields")]
        [SerializeField] BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;
        public override bool IsReadyInterstitial => adsLoaded.Contains(Placements.Interstitial);
        public override bool IsReadyRewarded => adsLoaded.Contains(Placements.Rewarded);
        public override bool IsReadyBanner => Advertisement.Banner.isLoaded;
        Action onCompleted;
        Action onFailed;
        List<string> adsLoaded { get; } = new List<string>();
        AdsLocation currentLocation;
        #endregion

        protected override void Initialize()
        {
            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(Key, isTestMode, this);
            }
            Advertisement.Banner.SetPosition(bannerPosition);
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
                Advertisement.Show(Placements.Rewarded, this);
            }
            else
            {
                onRVAvailable?.Invoke(false);
                Advertisement.Load(Placements.Rewarded, this);
            }
        }

        public override void ShowInterstitialAd(AdsLocation location, Action onCompleted = null, params string[] parameters)
        {
            if (IsReadyInterstitial)
            {
                this.onCompleted = onCompleted;
                currentLocation = location;
                Advertisement.Show(Placements.Interstitial, this);
            }
            else
            {
                onCompleted?.Invoke();
                Advertisement.Load(Placements.Interstitial, this);
            }
        }
        public override void ShowBanner()
        {
            // Set up options to notify the SDK of show events:
            BannerOptions options = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            };

            // Show the loaded Banner Ad Unit:
            Advertisement.Banner.Show(Placements.Banner, options);
        }

        public override void HideBanner()
        {
            Advertisement.Banner.Hide();
        }
        // Implement code to execute when the loadCallback event triggers:
        void OnBannerLoaded()
        {
            Debug.Log($"Banner Loaded");
        }

        // Implement code to execute when the load errorCallback event triggers:
        void OnBannerError(string message)
        {
            Debug.Log($"Banner Error: {message}");
        }

        void OnBannerClicked() { }
        void OnBannerShown() { }
        void OnBannerHidden() { }

        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete.");
            Initialized = true;
            if (Placements.LoadRewarded && Placements.Rewarded != "") Advertisement.Load(Placements.Rewarded, this);
            if (!IsRemoveAds.value)
            {
                if (Placements.LoadInterstitial && Placements.Interstitial != "") Advertisement.Load(Placements.Interstitial, this);
                if (Placements.LoadBanner && Placements.Banner != "")
                {
                    var options = new BannerLoadOptions();

                    options.loadCallback = OnBannerLoaded;
                    options.errorCallback = OnBannerError;

                    Advertisement.Banner.SetPosition(bannerPosition);
                    Advertisement.Banner.Load(Placements.Banner, options);
                }
            }
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }
        public void OnUnityAdsShowStart(string placementId)
        {
            adsLoaded.Remove(placementId);
            if (placementId == Placements.Interstitial)
            {
                GameEventHandler.Invoke(AdvertisingEventCode.OnShowAd, AdsType.Interstitial, currentLocation);
            }
            else if (placementId == Placements.Rewarded)
            {
                GameEventHandler.Invoke(AdvertisingEventCode.OnShowAd, AdsType.Rewarded, currentLocation);
            }
        }
        public void OnUnityAdsShowClick(string placementId) { }
        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if (placementId == Placements.Interstitial)
            {
                GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Interstitial, currentLocation, true);
                onCompleted?.Invoke();
            }
            else if (placementId == Placements.Rewarded)
            {
                GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Rewarded, currentLocation, true);
                onCompleted?.Invoke();
            }
            Advertisement.Load(placementId, this);
        }
        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {placementId}: {error.ToString()} - {message}");
            // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
            if (placementId == Placements.Interstitial)
            {
                GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Interstitial, currentLocation, false);
                onCompleted?.Invoke();
            }
            else if (placementId == Placements.Rewarded)
            {
                GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Rewarded, currentLocation, false);
                onFailed?.Invoke();
            }
        }
        // Implement Load Listener and Show Listener interface methods: 
        public void OnUnityAdsAdLoaded(string placementId)
        {
            // Optionally execute code if the Ad Unit successfully loads content.
            adsLoaded.Add(placementId);
        }
        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit: {placementId} - {error.ToString()} - {message}");
            // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
        }
#else
        protected override void Initialize()
        {
        }
#endif
    }
}

