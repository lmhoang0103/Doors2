#region

using Firebase;
using Firebase.Analytics;
using System;
using UnityEngine;
using AppsFlyerSDK;
using Firebase.Messaging;

#endregion

public class GameServices : Singleton<GameServices>
{
#if !PROTOTYPE
    public FirebaseApp firebaseApp;

    private bool _firebaseInited;
    public bool FirebaseInited => _firebaseInited && firebaseApp != null;

    [HideInInspector]
    public string firebaseMesssageToken = "";

    public void Init()
    {
#if FIREBASE
        FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
        InitFirebase();
#endif

        InitAppFlyer();
    }

    private void InitAppFlyer()
    {
#if PROTOTYPE
        AppsFlyer.setIsDebug(true);
#else
        AppsFlyer.setIsDebug(false);
#endif
        AppsFlyer.setHost("","appsflyersdk.com");

        AppsFlyer.initSDK(GameConst.AppflyerAppKey, GameManager.Instance.gameSetting.appstoreId);
        AppsFlyer.startSDK();
    }

    private void InitFirebase()
    {
        FirebaseApp.CheckDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                firebaseApp = FirebaseApp.DefaultInstance;
                SetupFirebase();
            }
            else
            {
                Debug.LogError(string.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs e)
    {
        firebaseMesssageToken = e.Token;
    }

    private void SetupFirebase()
    {
        _firebaseInited = true;

        RemoteConfigManager.Instance.StartAsync();

        HCDebug.Log("Firebase Inited Successfully!", HcColor.Aqua);

        AnalyticManager.SetFirebaseUserProperties(UserProperties.lastTimeLogin, DateTime.Now.DayOfYear.ToString());
        AnalyticManager.SetFirebaseUserProperties(UserProperties.gameVersion, GameManager.Instance.gameSetting.gameVersion);
        FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));

        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;
        
        AnalyticsRevenueAds.Init();
    }
#endif
}