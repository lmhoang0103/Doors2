using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class WheelItem : HCMonoBehaviour
{
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject gift;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private RewardType type;
    
    private int _value;

    public void InitCoin(int val)
    {
        type = RewardType.Coin;
        _value = val;
        coin.SetActive(true);
        gift.SetActive(false);
        valueText.text = val.ToFormatString();
    }

    public void InitGift()
    {
        type = RewardType.Gift;
        
        // TODO: Init gift reward
        
        coin.SetActive(false);
        gift.SetActive(true);
    }

    public void Claim()
    {
        switch (type)
        {
            case RewardType.Coin:
                Gm.AddMoney(_value);
                break;
            case RewardType.Gift:
                // TODO: Claim gift logic
                
                break;
        }
    }
}