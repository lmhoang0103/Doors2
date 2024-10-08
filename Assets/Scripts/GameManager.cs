#region

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using System;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

#endregion

public class GameManager : Singleton<GameManager>
{
#if UNITY_EDITOR
    private bool IsPlaying => EditorApplication.isPlaying;
    [ShowIf(nameof(IsPlaying))] 
#endif
    public GameData data;
    public bool gameInited;
    public HCGameSetting gameSetting;

    private int _secondToRemindComeback;
    
    [HideInInspector] public float lastClaimOnlineGiftTime;

    public static bool EnableAds
    {
        get
        {
            if (RemoteConfigManager.Instance != null && !RemoteConfigManager.Instance.shouldShowAds)
                return false;

            if (Instance != null && Instance.data != null && Instance.data.user.purchasedNoAds)
                return false;

            return true;
        }
    }

    public static bool NetworkAvailable => Application.internetReachability != NetworkReachability.NotReachable;

    public string GameVersion => string.Format("{0}.{1}.{2}", gameSetting.gameVersion, gameSetting.bundleVersion, gameSetting.buildVersion);


#if UNITY_EDITOR
    [Button]
    private void GetGameSetting()
    {
        gameSetting = HCTools.GetGameSetting();
    }
#endif

    protected override void Awake()
    {
        base.Awake();

        LoadGameData();

        SetupPushNotification();

        RequestTrackingForiOs();

        GUIManager.Instance.Init();

        Instance.gameInited = true;

        EventGlobalManager.Instance.OnGameInited.Dispatch();
        
        lastClaimOnlineGiftTime = Time.time;
    }

    private void Start()
    {
        GameServices.Instance.Init();
        EventGlobalManager.Instance.OnUpdateSetting.Dispatch();
        AdManager.Instance.appOpenAdManager.Init();
        
        if (!data.user.isConsentUpdated)
            PopupConsent.Show(LoadGameplay);
        else
            LoadGameplay();
        
        void LoadGameplay()
        {
            LoadingManager.Instance.LoadScene(SceneIndex.Gameplay, MainScreen.Show);
            AdManager.Instance.Init();
        }
    }

    [Button]
    public void AddMoney(int value)
    {
        data.user.money += value;
        EventGlobalManager.Instance.OnMoneyChange.Dispatch(true);
    }

    [Button]
    public bool SpendMoney(int value)
    {
        if (data.user.money >= value)
        {
            data.user.money -= value;
            EventGlobalManager.Instance.OnMoneyChange.Dispatch(true);
            return true;
        }

        EventGlobalManager.Instance.OnMoneyChange.Dispatch(false);
        return false;
    }

    private void LoadGameData()
    {
        data = Database.LoadData();
        if (data == null)
        {
            data = new GameData();

#if PROTOTYPE
            Data.User.PurchasedNoAds = true;
#endif
            Database.SaveData();
        }
    }

    private void SetupPushNotification()
    {
        _secondToRemindComeback = 60 * ConfigManager.Instance.gameCfg.maxOfflineRemindMinute + 30 * 60;

        if (data.setting.requestedPn)
            SetupRemindOfflinePushNotification();
        else
            PushNotificationManager.Instance.StartRequest();
    }

    private void RequestTrackingForiOs()
    {
#if UNITY_IOS
        if (!Data.Setting.iOSTrackingRequested)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
            Data.Setting.iOSTrackingRequested = true;
            Database.SaveData();
        }
#endif
    }

    private void UpdateGraphicSetting()
    {
        if (data.setting.highPerformance == 1)
        {
            Application.targetFrameRate = 60;
            Screen.SetResolution(Screen.width, Screen.height, true);
        }
        else
        {
            Application.targetFrameRate = 30;
            Screen.SetResolution(Screen.width / 2, Screen.height / 2, true);
        }
    }

    public override void OnApplicationQuit()
    {
        Logout();
        base.OnApplicationQuit();
    }

    public void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            Logout();
    }

    public void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            Logout();
    }

    private void Logout()
    {
        data.user.lastTimeLogOut = DateTime.Now;
        Database.SaveData();
    }

    private void OnEnable()
    {
        EventGlobalManager.Instance.OnUpdateSetting.AddListener(UpdateGraphicSetting);
    }

    private void OnDestroy()
    {
        if (!EventGlobalManager.Instance)
            return;

        EventGlobalManager.Instance.OnUpdateSetting.RemoveListener(UpdateGraphicSetting);
    }

    public void SetupRemindOfflinePushNotification()
    {
        PushNotificationManager.Instance.CancelNotification(PushNotificationType.RemindComeback);

        PushNotificationManager.Instance.ScheduleNotification(PushNotificationType.RemindComeback,
            "You have received a surprise gift. Claim it now!", _secondToRemindComeback);

        Database.SaveData();
    }
}