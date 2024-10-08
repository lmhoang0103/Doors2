using System;
using Sirenix.OdinInspector;
using UnityEngine;

[InlineEditor()]
[CreateAssetMenu(fileName = "Tier Config", menuName = "Configs/Tier")]
public class TierConfig : ScriptableObject
{
    public string tierName;
    public int scoreRequire;
    public Sprite icon;
    
    public RewardType rewardType;

    [ShowIf(nameof(rewardType), RewardType.Coin)] public int coinRewardValue;
}