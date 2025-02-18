using UnityEngine;
using System;
using HyrphusQ.Events;
using UnityEngine.Advertisements;
using Sirenix.OdinInspector;
using System.Collections.Generic;
#if LatteGames_CLIK
using Tabtale.TTPlugins;
#endif

namespace LatteGames.Monetization
{
    [OptionalDependency("Tabtale.TTPlugins.TTPCore", "LatteGames_CLIK")]
    public class CLIKAdsService : AbstractMobileAdsService
    {
#if LatteGames_CLIK
        #region Properties
        [Title("Custom Fields")]
        public override bool IsReadyInterstitial => TTPInterstitials.IsReady();
        public override bool IsReadyRewarded => TTPRewardedAds.IsReady();
        public override bool IsReadyBanner => true;
        Action onCompleted;
        Action onFailed;
        AdsLocation currentLocation;
        #endregion

        protected override void Initialize()
        {
            // Initialize TT Plugins
            TTPCore.Setup();
            Initialized = true;
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
                TTPRewardedAds.Show(location.ToString(), (isSuccess) =>
                {
                    if (isSuccess)
                    {
                        onCompleted?.Invoke();
                        GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Rewarded, currentLocation, true);
                    }
                    else
                    {
                        onFailed?.Invoke();
                        GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Rewarded, currentLocation, false);
                    }
                });
                GameEventHandler.Invoke(AdvertisingEventCode.OnShowAd, AdsType.Rewarded, currentLocation);
            }
            else
            {
                onRVAvailable?.Invoke(false);
            }
        }

        public override void ShowInterstitialAd(AdsLocation location, Action onCompleted = null, params string[] parameters)
        {
            if (IsReadyInterstitial)
            {
                this.onCompleted = onCompleted;
                currentLocation = location;
                TTPInterstitials.Show(location.ToString(), () =>
                {
                    onCompleted?.Invoke();
                    GameEventHandler.Invoke(AdvertisingEventCode.OnCloseAd, AdsType.Interstitial, currentLocation, true);
                });
                GameEventHandler.Invoke(AdvertisingEventCode.OnShowAd, AdsType.Interstitial, currentLocation);
            }
            else
            {
                onCompleted?.Invoke();
            }
        }
        public override void ShowBanner()
        {
            TTPBanners.Show();
        }

        public override void HideBanner()
        {
            TTPBanners.Hide();
        }
#else
        protected override void Initialize()
        {
        }
#endif
    }
}

