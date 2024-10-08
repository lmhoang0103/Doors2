using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class IronSourceManager : Singleton<IronSourceManager>, IMediationController
{
    private const int RetryLoadAdsInterval = 1;
    private string _appKey;
    
    public void Init()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;

        IronSource.Agent.shouldTrackNetworkState(true);

        RegisterRewardAdsCallback();
        RegisterInterstitialAdsCallback();
        RegisterBannerAdsCallback();
        
        RegisterRevenuePaidCallback();
#if UNITY_ANDROID
        _appKey = GameManager.Instance.gameSetting.androidAppKey;
#elif UNITY_IOS
        _appKey = GameManager.Instance.gameSetting.iosAppKey;
#endif
        IronSource.Agent.init(_appKey);
    }

    private void SdkInitializationCompletedEvent()
    {
        Debug.Log("========== IRON SOURCE INITED SUCCESSFULLY! APP KEY: " + _appKey);
        IronSource.Agent.validateIntegration();
        LoadRewardAds();
        LoadInterAds();
        LoadBanner();
    }
    
    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    public void OnStartWatchFullscreenAds()
    {
        AdManager.Instance.IsWatchingFullscreenAds = true;
        if (AdManager.Instance.onStartFullscreenAds != null)
            AdManager.Instance.onStartFullscreenAds.Invoke();
        Debug.Log("IRON SOURCE > START WATCHING ADS");
    }

    public void OnFinishWatchFullscreenAds()
    {
        AdManager.Instance.IsWatchingFullscreenAds = false;
        if (AdManager.Instance.onFinishFullscreenAds != null)
            AdManager.Instance.onFinishFullscreenAds.Invoke();
        
        ShowBanner();
        Debug.Log("IRON SOURCE > FINISH WATCHING ADS");
    }
    
    void OnGetReward()
    {
        if (AdManager.Instance.onGetRewardedAds != null)
            AdManager.Instance.onGetRewardedAds.Invoke();
    }
    
    #region Inter Ads

    public bool IsInterstitialLoaded() => IronSource.Agent.isInterstitialReady();
    
    public void LoadInterAds() => IronSource.Agent.loadInterstitial();
    
    void RetryLoadInterAds()
    {
        CancelInvoke(nameof(LoadInterAds));
        if (!IsInterstitialLoaded())
        {
            Debug.Log("Inter Load Ad Failed. Trying again...");
            Invoke(nameof(LoadInterAds), RetryLoadAdsInterval);
        }
    }
    
    private void RegisterInterstitialAdsCallback()
    {
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;
    }

    /************* Interstitial AdInfo Delegates *************/
    // Invoked when the interstitial ad was loaded successfully.
    void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
    {
        CancelInvoke(nameof(LoadInterAds));
        Debug.Log("Inter Ad Ready!");
    }
    // Invoked when the initialization process has failed.
    void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
    {
        RetryLoadInterAds();
    }
    // Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
    void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo) { }
    // Invoked when end user clicked on the interstitial ad
    void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo) { }
    // Invoked when the ad failed to show.
    void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        RetryLoadInterAds();
        OnFinishWatchFullscreenAds();
    }
    // Invoked when the interstitial ad closed and the user went back to the application screen.
    void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        RetryLoadInterAds();
        OnFinishWatchFullscreenAds();
    }
    // Invoked before the interstitial ad was opened, and before the InterstitialOnAdOpenedEvent is reported.
    // This callback is not supported by all networks, and we recommend using it only if  
    // it's supported by all networks you included in your build. 
    void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo) { }

    public void ShowInterstitial(string placement)
    {
        if (IsInterstitialLoaded())
            IronSource.Agent.showInterstitial(AdManager.VerifyInterAdPlacement(placement));
        else
            RetryLoadInterAds();
    }
    
    #endregion
    
    #region Reward Ads

    public bool IsRewardedAdLoaded() => IronSource.Agent.isRewardedVideoAvailable();
    
    public void LoadRewardAds() => IronSource.Agent.loadRewardedVideo();

    void RetryLoadRewardAds()
    {
        if(!IsRewardedAdLoaded())
            LoadRewardAds();
    }
    
    private void RegisterRewardAdsCallback()
    {
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
    }

    /************* RewardedVideo AdInfo Delegates *************/
    // Indicates that there’s an available ad.
    // The adInfo object includes information about the ad that was loaded successfully
    // This replaces the RewardedVideoAvailabilityChangedEvent(true) event
    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
        Debug.Log("Reward Ad Ready!");
    }
    // Indicates that no ads are available to be displayed
    // This replaces the RewardedVideoAvailabilityChangedEvent(false) event
    void RewardedVideoOnAdUnavailable()
    {
        RetryLoadRewardAds();
    }
    // The Rewarded Video ad view has opened. Your activity will loose focus.
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo){ }
    // The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        RetryLoadRewardAds();
        OnFinishWatchFullscreenAds();
    }
    // The user completed to watch the video, and should be rewarded.
    // The placement parameter will include the reward data.
    // When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        OnGetReward();
        RetryLoadRewardAds();
    }
    // The rewarded video ad was failed to show.
    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        RetryLoadRewardAds();
    }
    // Invoked when the video ad was clicked.
    // This callback is not supported by all networks, and we recommend using it only if
    // it’s supported by all networks you included in your build.
    void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo){ }
    
    public void ShowRewardedAd(string placement)
    {
        if (IsRewardedAdLoaded())
            IronSource.Agent.showRewardedVideo(placement);
        else
            RetryLoadRewardAds();
    }

    #endregion

    #region Banner Ads

    bool _isBannerLoaded;
        
    public bool IsBannerAdLoaded() => _isBannerLoaded;

    public void LoadBanner()
    {
        if (!GameManager.EnableAds)
            return;

        var bannerSize = IronSourceBannerSize.BANNER;
        bannerSize.SetAdaptive(true);
        IronSource.Agent.loadBanner(bannerSize, IronSourceBannerPosition.BOTTOM);
        Debug.Log("Start load Banner Ad...");
    }

    void RetryLoadBannerAds()
    {
        CancelInvoke(nameof(LoadBanner));
        if (!IsBannerAdLoaded())
        {
            Debug.Log($"Banner Ad Load Failed. Try again after {RetryLoadAdsInterval}s");
            Invoke(nameof(LoadBanner), RetryLoadAdsInterval);
        }
    }
    
    private void RegisterBannerAdsCallback()
    {
        IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;
    }
    
    /************* Banner AdInfo Delegates *************/
    //Invoked once the banner has loaded
    void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
    {
        _isBannerLoaded = true;
        Debug.Log("Banner Ad Ready!");
        CancelInvoke(nameof(RetryLoadBannerAds));
        ShowBanner();
    }
    //Invoked when the banner loading process has failed.
    void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
    {
        _isBannerLoaded = false;
        RetryLoadBannerAds();
    }
    // Invoked when end user clicks on the banner ad
    void BannerOnAdClickedEvent(IronSourceAdInfo adInfo) { }
    //Notifies the presentation of a full screen content following user click
    void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo) { }
    //Notifies the presented screen has been dismissed
    void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo) { }
    //Invoked when the user leaves the app
    void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo) { }
    
    public void ShowBanner()
    {
        if (GameManager.EnableAds && IsBannerAdLoaded())
            IronSource.Agent.displayBanner();
        else
            RetryLoadBannerAds();
    }
    
    public void HideBanner()
    {
        IronSource.Agent.hideBanner();
    }

    #endregion
}