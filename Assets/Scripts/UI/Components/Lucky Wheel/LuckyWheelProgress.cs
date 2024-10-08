using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LuckyWheelProgress : HCMonoBehaviour
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text progressTxt;
    [SerializeField] private SerializedDictionary<int, LuckyWheelProgressReward> rewardMap;

    public void Init()
    {
        var progress = Gm.data.user.luckyWheelProgress;
        progressSlider.value = progress;

        foreach (var pair in rewardMap)
        {
            pair.Value.SetClaimed(pair.Key <= progress);
        }
    }

    public void UpdateProgress()
    {
        var progress = Gm.data.user.luckyWheelProgress;
        progressTxt.text = $"Progress: {progress}/{progressSlider.maxValue}";
        progressSlider.DOValue(progress, .5f).OnComplete(() =>
        {
            foreach (var pair in rewardMap)
            {
                if (pair.Key == progress)
                    pair.Value.Claim();
                else
                    pair.Value.SetClaimed(pair.Key <= progress);
            }
        });
    }

    public void ClaimReward1()
    {
        // TODO: Claim reward logic
    }
    
    public void ClaimReward2()
    {
        // TODO: Claim reward logic
    }
    
    public void ClaimReward3()
    {
        // TODO: Claim reward logic
    }
}
