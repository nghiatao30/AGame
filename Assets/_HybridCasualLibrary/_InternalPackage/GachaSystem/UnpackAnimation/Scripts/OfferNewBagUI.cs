using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LatteGames.UnpackAnimation
{
    using Monetization;

    public class OfferNewBagUI : ComposeCanvasElementVisibilityController
    {
        public event Action OnRVWatched = delegate { };
        public event Action OnNoThanksButtonClicked = delegate { };

        [SerializeField] RVButtonBehavior rvButton;
        [SerializeField] Button noThanksButton;

        private void Awake()
        {
            rvButton.OnRewardGranted += HandleRVWatched;
            noThanksButton.onClick.AddListener(HandleNoThanksButtonClicked);
        }

        private void OnDestroy()
        {
            rvButton.OnRewardGranted -= HandleRVWatched;
            noThanksButton.onClick.RemoveListener(HandleNoThanksButtonClicked);
        }

        void HandleRVWatched(RVButtonBehavior.RewardGrantedEventData data)
        {
            OnRVWatched();
        }

        void HandleNoThanksButtonClicked()
        {
            OnNoThanksButtonClicked();
        }
        public void Setup(AdsLocation adsLocation)
        {
            rvButton.Location = adsLocation;
        }
    }
}