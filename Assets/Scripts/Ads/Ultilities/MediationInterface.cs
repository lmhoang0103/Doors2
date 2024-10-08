public interface IMediationController
{
    void Init();

    void OnStartWatchFullscreenAds();

    void OnFinishWatchFullscreenAds();

    #region Interstitial Ads

    bool IsInterstitialLoaded();

    void ShowInterstitial(string placement);

    #endregion

    #region Rewarded Ads

    bool IsRewardedAdLoaded();

    void ShowRewardedAd(string placement);

    #endregion

    #region Banner Ads

    bool IsBannerAdLoaded();

    void ShowBanner();

    void HideBanner();

    #endregion
}