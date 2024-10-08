
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Extra Feature Config", menuName = "Configs/Extra Feature")]
[InlineEditor(Expanded = true, ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
public class ExtraFeatureConfig : ScriptableObject
{
    [Title("Online Gift")] 
    public int onlineRewardInterval;
    public int onlineRewardValue;

    [PropertySpace] 
    [Title("Lucky Wheel")] 
    public int freeSpinInterval;
    public List<int> luckyWheelCoinRewardValues;

    [PropertySpace] 
    [Title("Chest Room")] 
    public List<int> chestRoomCoinRewardValues;
    
    [PropertySpace] 
    [Title("Daily Reward")] 
    public List<int> dailyRewardCoinRewardValues;
    
    [PropertySpace] 
    [Title("Leaderboard")] 
    public TextAsset nameList;
    [TableList] public List<TierConfig> tierConfigs;
    [SerializeField, FolderPath] private string tierConfigPath;
    public AnimationCurve rankToScoreCurve;
    // score = a * rank + b
    [SerializeField, HideInInspector] private float a, b;
    public int minDecreaseRankOfflineTime;
    public int decreaseRankTimeInterval;
    public int decreaseRankScoreOffset;
    
#if UNITY_EDITOR    
    [Button()]
    void GetTierConfig()
    {
        tierConfigs = HCTools.GetConfigs<TierConfig>(tierConfigPath).OrderBy(config => config.scoreRequire).ToList();
    }
#endif
    
    private void OnValidate()
    {
        rankToScoreCurve.SetCurveLinear();
        var keys = rankToScoreCurve.keys.OrderByDescending(keyframe => keyframe.time).ToArray();
        
        a = (keys[1].value - keys[0].value) / (keys[1].time - keys[0].time);
        b = keys[0].value - keys[0].time * a;
    }

    public float GetMaxRankAtZeroScore(int scoreOffset) => (-b - scoreOffset) / a;
}