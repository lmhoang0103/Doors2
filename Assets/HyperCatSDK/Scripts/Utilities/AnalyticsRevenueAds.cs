#region

using System;
using System.Threading.Tasks;
using AppsFlyerSDK;
using Firebase.Analytics;
using Firebase.Extensions;
using UniRx;
using UnityEngine;

#endregion

public class AnalyticsRevenueAds
{
    public static string appsflyerId;
    public static string firebaseId;

    public static async void Init()
    {
        appsflyerId = AppsFlyer.getAppsFlyerId();
        firebaseId = await GetAnalyticsInstanceId();
    }

    #region Iron Source

    public static void SendEvent(IronSourceImpressionData data)
    {
        SendEventRealtime(data);
    }

    private static void SendEventRealtime(IronSourceImpressionData data)
    {
        Firebase.Analytics.Parameter[] adParameters =
        {
            new Firebase.Analytics.Parameter("ad_platform", "iron_source"),
            new Firebase.Analytics.Parameter("ad_source", data.adNetwork),
            new Firebase.Analytics.Parameter("ad_unit_name", data.adUnit),
            new Firebase.Analytics.Parameter("currency", "USD"),
            new Firebase.Analytics.Parameter("value", data.revenue.Value),
            new Firebase.Analytics.Parameter("placement", data.placement),
            new Firebase.Analytics.Parameter("country_code", data.country),
            new Firebase.Analytics.Parameter("ad_format", data.instanceName),
        };

        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression_ironsource", adParameters);
    }

    #endregion

        public static void SendEvent(ImpressionData data, AdFormat type)
    {
        SendEventRealtime(data, type);
        SendEventThreshold(data, type);
    }

    private static void SendEventRealtime(ImpressionData data, AdFormat type)
    {
        Parameter[] adParameters =
        {
            new Parameter("ad_platform", "applovin"),
            new Parameter("ad_source", data.NetworkName),
            new Parameter("ad_unit_name", data.AdUnitIdentifier),
            new Parameter("currency", "USD"),
            new Parameter("value", data.Revenue),
            new Parameter("placement", data.Placement),
            new Parameter("country_code", data.CountryCode),
            new Parameter("ad_format", data.AdFormat)
        };

        FirebaseAnalytics.LogEvent("ad_impression_rocket", adParameters);
    }

    private static void SendEventThreshold(ImpressionData data, AdFormat type)
    {
        var rev = GetRevenueCache(type);
        rev += data.Revenue;
        var time = GetTimeLogin(type);
        var isMaxDay = CheckConditionDay(time, RemoteConfigManager.Instance.configMaxDaySendRevenue);

        if (rev >= RemoteConfigManager.Instance.minValueRevenue || isMaxDay)
        {
            // send event
            Parameter[] adParameters =
            {
                new Parameter("ad_platform", "applovin"),
                new Parameter("ad_source", data.NetworkName),
                new Parameter("ad_unit_name", data.AdUnitIdentifier),
                new Parameter("currency", "USD"),
                new Parameter("value", rev),
                new Parameter("placement", data.Placement),
                new Parameter("country_code", data.CountryCode),
                new Parameter("ad_format", data.AdFormat)
            };

            FirebaseAnalytics.LogEvent("ad_impression_threshold", adParameters);

            SetRevenueCache(type, 0);
            SetTimeLogin(type, DateTime.Now.ToString());
        }
        else
        {
            SetRevenueCache(type, rev);
        }
    }

    private static void SendEventServer(ImpressionData data, AdFormat type)
    {
        var form = new WWWForm(); //here you create a new form connection

#if UNITY_EDITOR
        form.AddField("platform", 2);
#elif UNITY_IOS
            form.AddField("platform", 1);
#else
            form.AddField("platform", 0);
#endif
        form.AddField("packagename", GameManager.Instance.gameSetting.packageName);
        form.AddField("ad_platform", "applovin");
        form.AddField("ad_source", data.NetworkName);
        form.AddField("ad_unit_name", data.AdUnitIdentifier);
        form.AddField("ad_format", data.AdFormat);
        form.AddField("currency", "USD");
        form.AddField("value", data.Revenue.ToString());
        form.AddField("appsflyer_id", appsflyerId);
        form.AddField("firebase_id", firebaseId);

        //send
        ObservableWWW.Post("http://analytics.rocketstudio.com.vn:2688/api/firebase_analystic", form).Subscribe(
            x => { Debug.Log("SendAnalystic Done"); }, // onSuccess
            ex =>
            {
                Debug.Log("SendAnalystic ex" + ex.Message);
                //MobileNativeMessage msg = new MobileNativeMessage("TAPP.vn", "Có lỗi xảy ra");
            } // onError
        );
    }

    private static double GetRevenueCache(AdFormat type)
    {
        return PlayerPrefs.GetFloat("revenueAd" + type, 0);
    }

    private static void SetRevenueCache(AdFormat type, double rev)
    {
        PlayerPrefs.SetFloat("revenueAd" + type, (float) rev);
    }

    private static bool CheckConditionDay(string stringTimeCheck, int maxDays)
    {
        if (string.IsNullOrEmpty(stringTimeCheck))
            return false;

        try
        {
            var timeNow = DateTime.Now;
            var timeOld = DateTime.Parse(stringTimeCheck);
            var timeOldCheck = new DateTime(timeOld.Year, timeOld.Month, timeOld.Day, 0, 0, 0);
            var tickTimeNow = timeNow.Ticks;
            var tickTimeOld = timeOldCheck.Ticks;

            var elapsedTicks = tickTimeNow - tickTimeOld;
            var elapsedSpan = new TimeSpan(elapsedTicks);
            var totalDay = elapsedSpan.TotalDays;

            if (totalDay >= maxDays)
                return true;
        }
        catch
        {
            return true;
        }

        return false;
    }

    private static string GetTimeLogin(AdFormat type)
    {
        return PlayerPrefs.GetString("time_login_check_rev" + type, DateTime.Now.ToString());
    }

    private static void SetTimeLogin(AdFormat type, string time)
    {
        PlayerPrefs.SetString("time_login_check_rev" + type, time);
    }

    public static Task<string> GetAnalyticsInstanceId()
    {
        return FirebaseAnalytics.GetAnalyticsInstanceIdAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                //DebugLog("App instance ID fetch was canceled.");
            }
            else if (task.IsFaulted)
            {
                //DebugLog(String.Format("Encounted an error fetching app instance ID {0}",
                //task.Exception.ToString()));
            }
            else if (task.IsCompleted)
            {
                //DebugLog(String.Format("App instance ID: {0}", task.Result));
            }

            return task;
        }).Unwrap();
    }
}

public class ImpressionData
{
    // !! Do not rename !!
    public string AdFormat;
    public string AdUnitIdentifier;
    public string CountryCode;
    public string NetworkName;
    public string Placement;
    public double Revenue;
}

public enum AdFormat
{
    // !! Do not rename !!
    interstitial,
    video_rewarded,
    banner
}