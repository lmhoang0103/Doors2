using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class DailyRewardItem : HCMonoBehaviour
{
    public enum Status
    {
        NotClaimable,
        Claimable,
        Claimed
    }
    
    [SerializeField] private HCButton button;
    [SerializeField] private GameObject coin, gift, claimed, claimable;
    [SerializeField] private TMP_Text coinVal;
    [SerializeField] private RewardType type;

    private Action _onClaim;
    private int _coinValue;

    public void InitCoin(int value, Action onClaim)
    {
        type = RewardType.Coin;
        _onClaim = onClaim;
        
        _coinValue = value;

        UpdateVisual();
    }
    
    public void InitGift(Action onClaim)
    {
        type = RewardType.Gift;
        _onClaim = onClaim;
        
        // TODO: Init gift logic

        UpdateVisual();
    }

    void UpdateVisual()
    {
        coin.SetActive(type == RewardType.Coin);
        gift.SetActive(type == RewardType.Gift);

        coinVal.text = _coinValue.ToFormatString();
    }
    
    public void SetStatus(Status status)
    {
        button.interactable = status == Status.Claimable;
        claimable.SetActive(status == Status.Claimable);
        claimed.SetActive(status == Status.Claimed);
    }

    public void Claim()
    {
        switch (type)
        {
            case RewardType.Coin:
                Gm.AddMoney(_coinValue);
                SetStatus(Status.Claimed);
                break;
            case RewardType.Gift:
                // TODO: Claim gift logic
                
                SetStatus(Status.Claimed);
                break;
        }

        Gm.data.user.dailyRewardClaimedCount++;
        Gm.data.user.lastDailyRewardClaimTime = DateTime.Now;
        _onClaim?.Invoke();
    }
}
