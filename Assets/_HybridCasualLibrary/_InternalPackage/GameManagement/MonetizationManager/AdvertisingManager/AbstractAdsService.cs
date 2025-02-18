using UnityEngine;
using System;
using HyrphusQ.Events;

namespace LatteGames.Monetization
{
    public abstract class AbstractAdsService : MonoBehaviour, IAdvertising
    {
        public virtual bool Initialized { get; protected set; }
        protected bool hasInitialized;
        public virtual bool HasInitialized => hasInitialized;
        public virtual bool IsReadyInterstitial => false;
        public virtual bool IsReadyRewarded => false;
        public virtual bool IsReadyBanner => false;
        [HideInInspector] public PPrefBoolVariable IsRemoveAds;

        public virtual void ShowRewardedAd(
            AdsLocation location,
            Action onCompleted,
            Action onFailed = null,
            Action<bool> onRVAvailable = null,
            params string[] parameters)
        {
            print("Show Rewarded Ad (default)");
        }

        public virtual void ShowInterstitialAd(AdsLocation location, Action onCompleted = null, params string[] parameters)
        {
            print("Show Interstitial Ad (default)");
        }

        public virtual void ShowBanner()
        {
            print("Show Banner (default)");
        }

        public virtual void HideBanner()
        {
            print("Hide Banner (default)");
        }
    }
}

