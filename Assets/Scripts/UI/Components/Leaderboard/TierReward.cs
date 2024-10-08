using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TierReward : HCMonoBehaviour
{
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject gift;
    [SerializeField] private TMP_Text valueLabel;

    private RewardType _rewardType;
    private int _coinValue;
    
    public void InitCoin(int coinValue)
    {
        _rewardType = RewardType.Coin;
        coin.SetActive(true);
        gift.SetActive(false);
        
        _coinValue = coinValue;
        valueLabel.text = coinValue.ToFormatString();
    }

    public void InitGift()
    {
        _rewardType = RewardType.Gift;
        coin.SetActive(false);
        gift.SetActive(true);
        
        // TODO: Init gift logic
    }

    public void ClaimReward()
    {
        switch (_rewardType)
        {
            case RewardType.Coin:
                Gm.AddMoney(_coinValue);
                break;
            case RewardType.Gift:
                // TODO: Claim gift logic
                
                break;
        }
    }
}
