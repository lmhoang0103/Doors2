using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class PopupLuckyWheel : UIPanel
{
    [SerializeField] private List<WheelItem> wheelItems;
    [SerializeField] private Transform spin;
    [SerializeField] private HCButton spinBtn;
    [SerializeField] private TMP_Text spinBtnLabel;
    [SerializeField, Range(0, 100)] private float giftRate;
    [SerializeField] private LuckyWheelProgress luckyWheelProgress;

    private bool _isSpinning;    
    private int _giftRewardIndex;
    
    public static PopupLuckyWheel Instance { get; private set; }

    public override UiPanelType GetId()
    {
        return UiPanelType.PopupLuckyWheel;
    }

    public static void Show()
    {
        var newInstance = (PopupLuckyWheel) GUIManager.Instance.NewPanel(UiPanelType.PopupLuckyWheel);
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
        _isSpinning = false;
        
        InitItems();
        UpdateSpinBtn();
        luckyWheelProgress.Init();
    }
    
    void InitItems()
    {
        _giftRewardIndex = Random.Range(0, wheelItems.Count);

        List<int> coinRewards = Cfg.gameCfg.extraFeatureConfig.luckyWheelCoinRewardValues.Clone();

        for (int i = 0; i < wheelItems.Count; i++)
        {
            if (_giftRewardIndex == i)
            {
                wheelItems[i].InitGift();
            }
            else
            {
                int reward = coinRewards.GetRandom();
                coinRewards.Remove(reward);
                wheelItems[i].InitCoin(reward);
            }
        }
    }

    public void FreeSpin()
    {
        if (!_isSpinning)
        {
            Spin();
            spinBtn.interactable = false;
            Gm.data.user.lastFreeSpinTime = DateTime.Now;
        }
    }

    public void SpinAds()
    {
        if (!_isSpinning)
        {
            AdManager.Instance.ShowRewardedAds("SpinLuckyWheelAds", Spin);
        }
    }

    void Spin()
    {
        _isSpinning = true;
        
        if (Gm.data.user.luckyWheelProgress == 10)
            Gm.data.user.luckyWheelProgress = 0;

        Gm.data.user.luckyWheelProgress++;
        luckyWheelProgress.UpdateProgress();
        
        int rd = (Random.Range(0f, 100f) < giftRate ? _giftRewardIndex : GetRandomCoinItemIndex())
                 + wheelItems.Count * Random.Range(5, 8);
        var rewardItem = wheelItems[rd % wheelItems.Count];
        spin.DORotate(new Vector3(0, 0, -rd * 45), 5).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            _isSpinning = false;

            rewardItem.Claim();
        });
    }

    void UpdateSpinBtn()
    {
        if (spinBtn.interactable)
            return;

        var remainTime = GetRemainTime();

        if (remainTime >= 0)
            spinBtnLabel.text = remainTime.ToTimeFormat();
        else
        {
            spinBtnLabel.text = "Spin";
            spinBtn.interactable = true;
        }
    }
    
    int GetRandomCoinItemIndex()
    {
        int result = Random.Range(0, wheelItems.Count);

        while (result == _giftRewardIndex)
            result = Random.Range(0, wheelItems.Count);

        return result;
    }

    public static int GetRemainTime() => ConfigManager.Instance.gameCfg.extraFeatureConfig.freeSpinInterval
                         - (int) (DateTime.Now - GameManager.Instance.data.user.lastFreeSpinTime).TotalSeconds;

    protected override void RegisterEvent()
    {
        base.RegisterEvent();

        Evm.OnEverySecondTick.AddListener(UpdateSpinBtn);
    }

    protected override void UnregisterEvent()
    {
        base.UnregisterEvent();
        
        if (Evm)
            Evm.OnEverySecondTick.RemoveListener(UpdateSpinBtn);
    }

    public override void OnDisappear()
    {
        base.OnDisappear();
        Instance = null;
    }
}