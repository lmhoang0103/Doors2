using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestItem : HCMonoBehaviour
{
    enum RewardType
    {
        Coin,
        Gift
    }
    
    [SerializeField] private HCButton chestButton;
    [SerializeField] private Transform coin;
    [SerializeField] private TMP_Text coinValue;
    [SerializeField] private Image giftIcon;
    [SerializeField] private CanvasGroup canvasGroup;
    
    public bool Claimed { get; private set; }
    
    private Action _onClaim;
    private RewardType _rewardType;
    
    public void InitCoin(int value, Action onClaim)
    {
        _rewardType = RewardType.Coin;
        coinValue.text = value.ToFormatString();
        chestButton.Show();
        Claimed = false;
        _onClaim = onClaim;
    }

    public void InitGift(Action onClaim)
    {
        // TODO: Init gift logic
        
        _rewardType = RewardType.Gift;
        chestButton.Show();
        Claimed = false;
        _onClaim = onClaim;
    }

    public void Claim()
    {
        chestButton.Hide();
        
        Transform targetTrans = _rewardType == RewardType.Coin ? coin : giftIcon.transform;
        targetTrans.gameObject.SetActive(true);
        targetTrans.localScale = Vector3.zero;
        targetTrans.DOScale(1, .3f);
        
        Claimed = true;
        _onClaim?.Invoke();
    }

    public void HideReward()
    {
        chestButton.interactable = true;
        canvasGroup.alpha = 1;
        coin.gameObject.SetActive(false);
        giftIcon.gameObject.SetActive(false);
    }

    public void PeekReward()
    {
        chestButton.interactable = false;
        canvasGroup.alpha = .5f;
        coin.gameObject.SetActive(_rewardType == RewardType.Coin);
        giftIcon.gameObject.SetActive(_rewardType == RewardType.Gift);
    }
}
