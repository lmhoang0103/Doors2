using System;
using TMPro;
using UnityEngine;

public class PopupOfflineReward : UIPanel
{
    [SerializeField] private TMP_Text rewardTxt;

    private int _reward;
    private bool _claimed;
    
    public static PopupOfflineReward Instance { get; private set; }

    public override UiPanelType GetId()
    {
        return UiPanelType.PopupOfflineReward;
    }

    public static void Show()
    {
        var newInstance = (PopupOfflineReward) GUIManager.Instance.NewPanel(UiPanelType.PopupOfflineReward);
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
        var offlineTime = (DateTime.Now - Gm.data.user.lastTimeLogOut).TotalSeconds;
        
        // TODO: Income formula
        _reward = 1000;

        _claimed = false;
        rewardTxt.text = _reward.ToFormatString();
    }

    public void Claim()
    {
        if (!_claimed)
        {
            _claimed = true;
            Gm.AddMoney(_reward);
            
            Close();
        }
    }

    public void ClaimAds()
    {
        AdManager.Instance.ShowRewardedAds("ClaimOfflineRewardAds", () =>
        {
            if (!_claimed)
            {
                _claimed = true;
                Gm.AddMoney(_reward * 2);

                Close();
            }
        });
    }

    public override void OnDisappear()
    {
        base.OnDisappear();
        Instance = null;
    }
}