using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DailyRewardButton : HCMonoBehaviour
{
    [SerializeField] private GameObject notif;

    private void OnEnable()
    {
        EventGlobalManager.Instance.OnEverySecondTick.AddListener(UpdateNotif);
        UpdateNotif();
    }

    private void OnDisable()
    {
        if (EventGlobalManager.Instance)
            EventGlobalManager.Instance.OnEverySecondTick.AddListener(UpdateNotif);
    }
    
    void UpdateNotif()
    {
        notif.SetActive(PopupDailyReward.IsClaimable());
    }

    public void OpenPopup()
    {
        PopupDailyReward.Show();
    }
}
