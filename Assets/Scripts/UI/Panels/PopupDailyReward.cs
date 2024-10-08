using System;
using TMPro;
using UnityEngine;

public class PopupDailyReward : UIPanel
{
    [SerializeField] private DailyRewardItem[] dailyRewardItems;
    [SerializeField] private GameObject claimable;
    [SerializeField] private GameObject notClaimable;
    [SerializeField] private TMP_Text timer;
    
    public static PopupDailyReward Instance { get; private set; }

    public override UiPanelType GetId()
    {
        return UiPanelType.PopupDailyReward;
    }

    public static void Show()
    {
        var newInstance = (PopupDailyReward) GUIManager.Instance.NewPanel(UiPanelType.PopupDailyReward);
        Instance = newInstance;
        newInstance.OnAppear();
    }

    public override void OnAppear()
    {
        if (isInited)
            return;

        base.OnAppear();

        Init();
    }

    private void Init()
    {
        for (var i = 0; i < dailyRewardItems.Length; i++)
        {
            if (i < dailyRewardItems.Length - 1)
                dailyRewardItems[i].InitCoin(Cfg.gameCfg.extraFeatureConfig.dailyRewardCoinRewardValues[i], UpdateTimer);
            else
                dailyRewardItems[i].InitGift(UpdateTimer);
        }
        
        UpdateTimer();
    }

    void UpdateTimer()
    {
        int currentDayIndex = Gm.data.user.dailyRewardClaimedCount % 7;
        var isClaimable = IsClaimable();

        claimable.SetActive(isClaimable);
        notClaimable.SetActive(!isClaimable);
        
        
        if (!isClaimable)
            timer.text = GetRemainTime().ToTimeFormat();
        
        for (var i = 0; i < dailyRewardItems.Length; i++)
        {
            if (i < currentDayIndex)
                dailyRewardItems[i].SetStatus(DailyRewardItem.Status.Claimed);
            else if (i == currentDayIndex)
                dailyRewardItems[i].SetStatus(isClaimable 
                    ? DailyRewardItem.Status.Claimable 
                    : DailyRewardItem.Status.NotClaimable);
            else
                dailyRewardItems[i].SetStatus(DailyRewardItem.Status.NotClaimable);
        }
    }

    public static bool IsClaimable()
    {
        var now = DateTime.Now;
        bool isClaimable = now.Day > GameManager.Instance.data.user.lastDailyRewardClaimTime.Day ||
                           now.Month > GameManager.Instance.data.user.lastDailyRewardClaimTime.Month ||
                           now.Year > GameManager.Instance.data.user.lastDailyRewardClaimTime.Year;
        return isClaimable;
    }

    public void SkipDay()
    {
        AdManager.Instance.ShowRewardedAds("SkipDailyRewardDay", () =>
        {
            Gm.data.user.lastDailyRewardClaimTime = DateTime.MinValue;
            UpdateTimer();
        });
    }
    
    public static int GetRemainTime() => (int) (DateTime.Today.AddDays(1) - DateTime.Now).TotalSeconds;
    
    protected override void RegisterEvent()
    {
        base.RegisterEvent();

        Evm.OnEverySecondTick.AddListener(UpdateTimer);
    }

    protected override void UnregisterEvent()
    {
        base.UnregisterEvent();
        
        if (Evm)
            Evm.OnEverySecondTick.RemoveListener(UpdateTimer);
    }

    public override void OnDisappear()
    {
        base.OnDisappear();
        Instance = null;
    }
}