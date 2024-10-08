using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PopupOnlineReward : UIPanel
{
    [SerializeField] private Transform claimableRoot;
    [SerializeField] private Transform notClaimableRoot;
    [SerializeField] private TMP_Text timer;
    [SerializeField] private TMP_Text rewardValueTxt;
    
    private int _rewardValue;
    private bool _claimable;
    
    public static PopupOnlineReward Instance { get; private set; }

    public override UiPanelType GetId()
    {
        return UiPanelType.PopupOnlineReward;
    }

    public static void Show()
    {
        var newInstance = (PopupOnlineReward) GUIManager.Instance.NewPanel(UiPanelType.PopupOnlineReward);
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
        #region Init reward

        _rewardValue = Cfg.gameCfg.extraFeatureConfig.onlineRewardValue;
        rewardValueTxt.text = _rewardValue.ToFormatString();

        #endregion
        
        _claimable = GetRemainTime() < 0;
        
        if (_claimable)
            ShowGift(true);
        else
            HideGift(true);
        
        UpdateTimer();
    }
    
    void ShowGift(bool isInit = false)
    {
        _claimable = true;
        
        if (isInit)
            ShowClaimable();
        else
        {
            notClaimableRoot.gameObject.SetActive(true);
            claimableRoot.gameObject.SetActive(false);
            notClaimableRoot.localScale = Vector3.one;
            notClaimableRoot.DOScale(0, .3f).OnComplete(ShowClaimable);
        }

        void ShowClaimable()
        {
            notClaimableRoot.gameObject.SetActive(false);
            claimableRoot.gameObject.SetActive(true);
            claimableRoot.localScale = Vector3.zero;
            claimableRoot.DOScale(1, .3f);
        }
    }

    void HideGift(bool isInit = false)
    {
        _claimable = false;

        if (isInit)
            ShowNotClaimable();
        else
        {
            claimableRoot.gameObject.SetActive(true);
            notClaimableRoot.gameObject.SetActive(false);
            claimableRoot.localScale = Vector3.one;
            claimableRoot.DOScale(0, .3f).OnComplete(ShowNotClaimable);
        }

        void ShowNotClaimable()
        {
            claimableRoot.gameObject.SetActive(false);
            notClaimableRoot.gameObject.SetActive(true);
            notClaimableRoot.localScale = Vector3.zero;
            notClaimableRoot.DOScale(1, .3f);
        }
    }
    
    void UpdateTimer()
    {
        if (_claimable)
            return;

        int timeRemain = GetRemainTime();

        if (timeRemain < 0)
            ShowGift();
        else
            timer.text = timeRemain.ToTimeFormatCompact();
    }
    
    public static int GetRemainTime() => Mathf.RoundToInt(ConfigManager.Instance.gameCfg.extraFeatureConfig.onlineRewardInterval -
                                                          (Time.time - GameManager.Instance.lastClaimOnlineGiftTime));

    public void Claim()
    {
        #region Claim logic

        Gm.AddMoney(_rewardValue);

        #endregion
        
        ResetGiftTimer();
        Close();
    }

    public void ClaimAds()
    {
        AdManager.Instance.ShowRewardedAds("ClaimOnlineGiftAds", () =>
        {
            #region Claim ads logic

            Gm.AddMoney(_rewardValue * 3);

            #endregion

            ResetGiftTimer();
            Close();
        });
    }
    
    void ResetGiftTimer()
    {
        Gm.lastClaimOnlineGiftTime = Time.time;
    }
    
    protected override void RegisterEvent()
    {
        base.RegisterEvent();
        
        EventGlobalManager.Instance.OnEverySecondTick.AddListener(UpdateTimer);
    }

    protected override void UnregisterEvent()
    {
        base.UnregisterEvent();
        
        if (EventGlobalManager.Instance)
            EventGlobalManager.Instance.OnEverySecondTick.RemoveListener(UpdateTimer);
    }

    public override void OnDisappear()
    {
        base.OnDisappear();
        Instance = null;
    }
}