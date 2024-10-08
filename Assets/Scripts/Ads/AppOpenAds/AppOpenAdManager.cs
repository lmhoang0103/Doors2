#region

using System;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

#endregion

public class AppOpenAdManager : Singleton<AppOpenAdManager>
{
    private static AppOpenAdManager _instance;

    public static bool shouldShowOpenAds = true;
    public static bool shouldShowResumeAds = true;
    public static bool isWatchingOtherAds = false;
    public static bool showedFirstOpenAd = false;

    public static int waterfallTierCount = 3;
    public static int tryGetAoaTime = -1;

    private AppOpenAd _ad;

    private bool _isShowingAoa;
    private DateTime _loadTime;

    private int _firstOpenAdAttemp = 0;
    private int _tierIndex = 1;

    private bool IsAdAvailable => _ad != null && (DateTime.UtcNow - _loadTime).TotalHours < 4;

    public void Init()
    {
        MobileAds.Initialize(status => { OnInitSuccess(); });
        
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }

    private void OnAppStateChanged(AppState state)
    {
        // Display the app open ad when the app is foregrounded.
        UnityEngine.Debug.Log("App State is " + state);
        if (state == AppState.Foreground)
        {
            ShowResumeAppAd();
        }
    }

    private void OnInitSuccess()
    {
        StartRequestAoa();
    }

    public void StartRequestAoa()
    {
        if (!GameManager.EnableAds)
            return;

        _tierIndex = 1;
        RequestLoadAoaByTier(_tierIndex);
    }

    public void RequestLoadAoaByTier(int index)
    {
        if (_tierIndex <= 0 || _tierIndex > GameManager.Instance.gameSetting.aoaListIds.Count)
            _tierIndex = 1;

        var id = GameManager.Instance.gameSetting.aoaListIds[_tierIndex - 1];

        var request = new AdRequest.Builder().Build();
        AppOpenAd.LoadAd(id, GameManager.Instance.gameSetting.aoaOrientation, request, OnLoadAoaResponse);

        Debug.Log(string.Format("Request AOA tier {0} - Id: {1}", _tierIndex, id));
    }

    private void OnLoadAoaResponse(AppOpenAd ad, AdFailedToLoadEventArgs error)
    {
        if (error != null)
        {
            Debug.LogFormat("Failed to load Aoa. (reason: {0}), tier {1}", error.LoadAdError.GetMessage(), _tierIndex);

            _tierIndex++;
            if (_tierIndex <= waterfallTierCount)
            {
                RequestLoadAoaByTier(_tierIndex);
            }
            else
            {
                _tierIndex = 1;
                if (!showedFirstOpenAd)
                {
                    if (_firstOpenAdAttemp < 5)
                    {
                        _firstOpenAdAttemp++;
                        Invoke(nameof(StartRequestAoa), 0.5f);
                    }
                    else
                    {
                        showedFirstOpenAd = true;
                        if (tryGetAoaTime > 0)
                            Invoke(nameof(StartRequestAoa), tryGetAoaTime);
                    }
                }
                else
                {
                    if (tryGetAoaTime > 0)
                        Invoke(nameof(StartRequestAoa), tryGetAoaTime);
                }
            }

            return;
        }

        // App open ad is loaded.
        _ad = ad;
        _tierIndex = 1;
        _loadTime = DateTime.UtcNow;

        if (!showedFirstOpenAd)
            ShowOpenAppAd();
    }

    public void ShowOpenAppAd()
    {
        if (!shouldShowOpenAds)
            return;

        ShowAoa();

        showedFirstOpenAd = true;
    }

    public void ShowResumeAppAd()
    {
        if (!shouldShowResumeAds)
            return;

        ShowAoa();
    }

    private bool ShowAoa()
    {
        if (!GameManager.EnableAds)
            return false;

        if (AdManager.Instance.IsWatchingFullscreenAds || _isShowingAoa)
            return false;

        if (!IsAdAvailable)
        {
            StartRequestAoa();
            return false;
        }

        _ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
        _ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
        _ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
        _ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;
        _ad.OnPaidEvent += HandlePaidEvent;

        _ad.Show();

        return true;
    }

    private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        _ad = null;
        _isShowingAoa = false;
        StartRequestAoa();
    }

    private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
    {
        Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        _ad = null;
        StartRequestAoa();
    }

    private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Displayed app open ad");
        _isShowingAoa = true;
    }

    private void HandleAdDidRecordImpression(object sender, EventArgs args)
    {
        Debug.Log("Recorded ad impression");
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
            args.AdValue.CurrencyCode, args.AdValue.Value);
    }
}