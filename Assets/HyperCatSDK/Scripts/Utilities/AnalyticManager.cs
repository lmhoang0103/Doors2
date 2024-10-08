#region

using System.Collections.Generic;
#if !PROTOTYPE
using AppsFlyerSDK;
using Firebase.Analytics;

#endif

#endregion

public class AnalyticManager : Singleton<AnalyticManager>
{
#if !PROTOTYPE

    #region Setup

    private static string GetCorrectEventName(string name)
    {
        return string.Format("{0}_{1}", GameManager.Instance.gameSetting.firebaseEventPrefix, name);
    }

    public static void SetFirebaseUserProperties(string eventName, string property)
    {
#if FIREBASE
        if (GameServices.Instance.FirebaseInited)
        {
            HCDebug.Log("Firebase Set User Property: " + eventName + ", value: " + property, HcColor.Yellow);
            FirebaseAnalytics.SetUserProperty(GetCorrectEventName(eventName), property);
        }
#endif
    }

    public static void LogEvent(string eventName, params Parameter[] parameters)
    {
#if FIREBASE
        if (GameServices.Instance.FirebaseInited)
        {
            HCDebug.Log("Firebase: " + eventName, HcColor.Yellow);
            FirebaseAnalytics.LogEvent(GetCorrectEventName(eventName), parameters);
        }
#endif
    }

    public static void LogEvent(string eventName, string paramName, string paramValue)
    {
#if FIREBASE
        if (GameServices.Instance.FirebaseInited)
        {
            HCDebug.Log("Firebase: " + eventName, HcColor.Yellow);
            FirebaseAnalytics.LogEvent(GetCorrectEventName(eventName), paramName, paramValue);
        }
#endif
    }

    public static void LogEvent(string eventName)
    {
#if FIREBASE
        if (GameServices.Instance.FirebaseInited)
        {
            HCDebug.Log("Firebase: " + eventName, HcColor.Yellow);
            FirebaseAnalytics.LogEvent(GetCorrectEventName(eventName));
        }
#endif
    }

    #endregion

    #region APPFLYER

    public void AppflyerLogIapAndroid(string signature, string purchaseData, string price, string currency)
    {
#if UNITY_ANDROID
        AppsFlyerAndroid.validateAndSendInAppPurchaseStatic(GameConst.PublicKeyAppsflyer, signature, purchaseData, price, currency, null, this);
#endif
        HCDebug.Log("Log IAP to Appflyer: " + price, HcColor.Purple);
    }

    public static void AppflyerLogAdsClicked(AdsType type, string placement)
    {
        switch (type)
        {
            case AdsType.Interstitial:
                AppsFlyer.sendEvent("event_interstitial_ad_clicked", new Dictionary<string, string> {{"interstitial_ad_clicked", placement}});
                break;
            case AdsType.Rewarded:
                AppsFlyer.sendEvent("event_video_reward_clicked", new Dictionary<string, string> {{"clicked_video", placement}});
                break;
        }
    }

    public static void AppflyerLogAdsImpression(AdsType type, string placement)
    {
        switch (type)
        {
            case AdsType.Interstitial:
                AppsFlyer.sendEvent("event_interstitial_ad_impression", new Dictionary<string, string> {{"event_interstitial_ad_impression", placement}});
                break;
            case AdsType.Rewarded:
                AppsFlyer.sendEvent("event_video_reward_impression", new Dictionary<string, string> {{"impression_video", placement}});
                break;
        }
    }

    #endregion

#endif
}