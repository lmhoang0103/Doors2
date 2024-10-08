#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine;

#endregion

public class RemoteConfigManager : Singleton<RemoteConfigManager>
{
    private bool _fetcheCompleted;

    public bool isReady;
    public bool shouldShowAds = true;

    public int configMaxDaySendRevenue = 1;
    public float minValueRevenue = 0.1f;

    public void StartAsync()
    {
        var defaults = new Dictionary<string, object>();
        defaults.Add("min_value_revenue", minValueRevenue);
        defaults.Add("config_max_day_send_revenue", configMaxDaySendRevenue);
        defaults.Add("AdsEnabled", shouldShowAds);
        defaults.Add("AppOpenAds", true);
        defaults.Add("AppResumeAds", true);
        defaults.Add("WaterfallTierCount", 3);
        defaults.Add("TestFillAOA", false);
        defaults.Add("TryGetAOATime", -1);

        FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
            .ContinueWithOnMainThread(task =>
            {
                StartCoroutine(WaitForAsync());
                _fetcheCompleted = true;
            });
    }

    private IEnumerator WaitForAsync()
    {
        while (!_fetcheCompleted)
            yield return null;

        FetchDataAsync();
    }

    public Task FetchDataAsync()
    {
        var fetchTask =
            FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {
            Debug.Log("Fetch canceled.");
        }
        else if (fetchTask.IsFaulted)
        {
            Debug.Log("Fetch encountered an error.");
        }
        else if (fetchTask.IsCompleted)
        {
            Debug.Log("Fetch completed successfully!");

            AppOpenAdManager.shouldShowOpenAds = FirebaseRemoteConfig.DefaultInstance
                .GetValue("AppOpenAds").BooleanValue;
            Debug.Log("AppOpenAds " + AppOpenAdManager.shouldShowOpenAds);

            AppOpenAdManager.shouldShowResumeAds = FirebaseRemoteConfig.DefaultInstance
                .GetValue("AppResumeAds").BooleanValue;
            Debug.Log("AppResumeAds " + AppOpenAdManager.shouldShowResumeAds);

            configMaxDaySendRevenue = (int) FirebaseRemoteConfig.DefaultInstance
                .GetValue("config_max_day_send_revenue").LongValue;
            Debug.Log("config_max_day_send_revenue " + configMaxDaySendRevenue);

            minValueRevenue = (float) FirebaseRemoteConfig.DefaultInstance
                .GetValue("min_value_revenue").DoubleValue;
            Debug.Log("min_value_revenue " + minValueRevenue);

            AppOpenAdManager.waterfallTierCount = (int) FirebaseRemoteConfig.DefaultInstance
                .GetValue("WaterfallTierCount").LongValue;
            Debug.Log("WaterfallTierCount " + AppOpenAdManager.waterfallTierCount);

            shouldShowAds = FirebaseRemoteConfig.DefaultInstance
                .GetValue("AdsEnabled").BooleanValue;
            Debug.Log("AdsEnabled " + shouldShowAds);

            AppOpenAdManager.tryGetAoaTime = (int) FirebaseRemoteConfig.DefaultInstance
                .GetValue("TryGetAOATime").LongValue;
            Debug.Log("TryGetAOATime " + AppOpenAdManager.tryGetAoaTime);

            isReady = true;
        }

        var info = FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case LastFetchStatus.Success:
                FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                    .ContinueWithOnMainThread(task =>
                    {
                        Debug.Log(string.Format("Remote data loaded and ready (last fetch time {0}).",
                            info.FetchTime));
                    });

                break;
            case LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");
                        break;
                    case FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }

                break;
            case LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }
    }
}