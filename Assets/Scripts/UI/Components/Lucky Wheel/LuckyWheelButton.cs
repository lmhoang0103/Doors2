
using TMPro;
using UnityEngine;

public class LuckyWheelButton : HCMonoBehaviour
{
    [SerializeField] private TMP_Text timer;
    [SerializeField] private GameObject notif;

    private void OnEnable()
    {
        EventGlobalManager.Instance.OnEverySecondTick.AddListener(UpdateTimer);
        UpdateTimer();
    }

    private void OnDisable()
    {
        if (EventGlobalManager.Instance)
            EventGlobalManager.Instance.OnEverySecondTick.AddListener(UpdateTimer);
    }

    void UpdateTimer()
    {
        int timeRemain = PopupLuckyWheel.GetRemainTime();

        if (timeRemain < 0)
        {
            notif.SetActive(true);
            timer.text = "Spin";
        }
        else
        {
            notif.SetActive(false);
            timer.text = timeRemain.ToTimeFormatCompact();
        }
    }

    public void OpenPopup()
    {
        PopupLuckyWheel.Show();
    }
}