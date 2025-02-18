using UnityEngine;
using System;
using HyrphusQ.Events;
using Sirenix.OdinInspector;

namespace LatteGames.Monetization
{
    public abstract class AbstractMobileAdsService : AbstractAdsService
    {
        [SerializeField] protected bool isTestMode;
        [Title("Android")]
        public string AndroidKey;
        public Placements AndroidPlacements;

        [Title("iOS")]
        public string IOSKey;
        public Placements IOSPlacements;

#if UNITY_ANDROID

        protected string Key => AndroidKey;
        protected Placements Placements => AndroidPlacements;

#elif UNITY_IOS

        protected string Key => IOSKey;
        protected Placements Placements => IOSPlacements;

#elif UNITY_EDITOR

        protected string Key => AndroidKey;
        protected Placements Placements => AndroidPlacements;

#endif
        protected abstract void Initialize();
        protected virtual void Awake()
        {
#if !UNITY_EDITOR
            Initialize();
#endif
        }
    }
    [Serializable]
    public class Placements
    {
        [Header("Interstitial")]
        public bool LoadInterstitial = true;
        public string Interstitial;

        [Header("Rewarded")]
        public bool LoadRewarded = true;
        public string Rewarded;

        [Header("Banner")]
        public bool LoadBanner = true;
        public string Banner;
    }
}

