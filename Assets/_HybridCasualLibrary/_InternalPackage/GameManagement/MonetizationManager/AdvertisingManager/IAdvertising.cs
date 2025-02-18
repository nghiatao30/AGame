using System;
using static LatteGames.Monetization.AbstractAdsService;

public interface IAdvertising
{
    /// <summary>
    /// Check if the AdsManager has been initialized
    /// </summary>
    bool HasInitialized { get; }
    /// <summary>
    /// Check if Interstitial is ready
    /// </summary>
    bool IsReadyInterstitial { get; }
    /// <summary>
    /// Check if Rewarded is ready
    /// </summary>
    bool IsReadyRewarded { get; }
    /// <summary>
    /// Check if Banner is ready
    /// </summary>
    bool IsReadyBanner { get; }
    /// <summary>
    /// Shows an interstitial ad.
    /// </summary>
    void ShowInterstitialAd(AdsLocation location, Action onCompleted = null, params string[] parameters);
    /// <summary>
    /// Shows an rewarded ad.
    /// </summary>
    void ShowRewardedAd(AdsLocation location, Action onCompleted, Action onFailed = null, Action<bool> onRVAvailable = null, params string[] parameters);
    /// <summary>
    /// Shows banner.
    /// </summary>
    void ShowBanner();
    /// <summary>
    /// Hides banner.
    /// </summary>
    void HideBanner();
}
