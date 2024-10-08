#region

using System;
using UnityEngine;

#endregion

public class AdManager : Singleton<AdManager>
{
    public AdsNetwork network;

    private IMediationController _mediationController;

    public IronSourceManager ironSource;
    //public MaxMediationController maxMediation;

    public AppOpenAdManager appOpenAdManager;

    [HideInInspector]
    public Action onStartFullscreenAds;

    [HideInInspector]
    public Action onFinishFullscreenAds;

    [HideInInspector]
    public Action onGetRewardedAds;

    public bool IsWatchingFullscreenAds { get; set; }

    protected override void Awake()
    {
        base.Awake();

        switch (network)
        {
            /*case AdsNetwork.Max:
                _mediationController = maxMediation;
                break;*/
            case AdsNetwork.IronSource:
                _mediationController = ironSource;
                break;
        }

        EventGlobalManager.Instance.OnPurchaseNoAds.AddListener(HideBanner);
    }

    public void Init()
    {
        _mediationController.Init();
    }

    private void OnDestroy()
    {
        if (EventGlobalManager.Instance == null)
            return;

        EventGlobalManager.Instance.OnPurchaseNoAds.RemoveListener(HideBanner);
    }

    private void RegisterCallback(Action onStartAds, Action onFinishAds, Action onGetReward)
    {
        onStartFullscreenAds = onStartAds;
        onFinishFullscreenAds = onFinishAds;
        onGetRewardedAds = onGetReward;
    }

    public bool IsInterstitialAdReady()
    {
        return _mediationController.IsInterstitialLoaded();
    }

    public bool IsRewardAdReady()
    {
        return _mediationController.IsRewardedAdLoaded();
    }

    public bool IsBannerAdReady()
    {
        return _mediationController.IsBannerAdLoaded();
    }

    public bool ShowInterstitialAd(string placement, bool notif = false, Action onStart = null, Action onFinish = null)
    {
        if (!GameManager.EnableAds)
            return false;

#if UNITY_EDITOR
        if (notif)
            PopupInterAdsNotif.Show(null, 2);
        
        return true;
#endif
        
        if (!IsInterstitialAdReady())
            return false;

        RegisterCallback(onStart, onFinish, null);

        if (notif)
            PopupInterAdsNotif.Show(ShowAds, 2);
        else
            ShowAds();
        
        return true;
        
        void ShowAds()
        {
            _mediationController.OnStartWatchFullscreenAds();
            _mediationController.ShowInterstitial(placement);
        }
    }

    public bool ShowRewardedAds(string placement, Action onGetReward, Action onStart = null, Action onFinish = null)
    {
        if (!GameManager.EnableAds)
        {
            onGetReward?.Invoke();
            return false;
        }

#if UNITY_EDITOR
        onGetReward?.Invoke();
        return true;
#endif
        
        if (!IsRewardAdReady())
        {
            if (!GameManager.NetworkAvailable)
                PopupNoInternet.Show();
            else
                PopupNotification.Show("No reward video available! Try again later.");

            return false;
        }

        RegisterCallback(onStart, onFinish, onGetReward);

        _mediationController.OnStartWatchFullscreenAds();
        _mediationController.ShowRewardedAd(placement);

        return true;
    }

    public bool ShowBanner()
    {
        if (!GameManager.EnableAds)
            return false;

        if (!IsBannerAdReady())
        {
            if (!GameManager.NetworkAvailable)
                PopupNoInternet.Show();
            
            return false;
        }

        _mediationController.ShowBanner();

        return true;
    }

    private void HideBanner()
    {
        _mediationController.HideBanner();
    }

    public static string VerifyInterAdPlacement(string input)
    {
        var output = input;
        if (input.IndexOf("Inter_") != 0)
            output = "Inter_" + input;

        return output;
    }

    public static string VerifyRewardAdPlacement(string input)
    {
        var output = input;
        if (input.IndexOf("Reward_") != 0)
            output = "Reward_" + input;

        return output;
    }
}

public enum AdsType
{
    Rewarded,
    Interstitial,
    Banner,
    Aoa
}

public enum AdsNetwork
{
    Max,
    IronSource,
}