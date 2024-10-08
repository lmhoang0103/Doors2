using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupInterAdsNotif : UIPanel
{
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text timer;

    private Coroutine _timerCoroutine;
    private Action _onClose;

    public static PopupInterAdsNotif Instance { get; private set; }

    public override UiPanelType GetId()
    {
        return UiPanelType.PopupInterAdsNotif;
    }

    public static void Show(Action onClose, int delay)
    {
        var newInstance = (PopupInterAdsNotif) GUIManager.Instance.NewPanel(UiPanelType.PopupInterAdsNotif);
        Instance = newInstance;
        newInstance.OnAppear(onClose, delay);
    }

    public void OnAppear(Action onClose, int delay)
    {
        if (isInited)
            return;

        base.OnAppear();

        Init(onClose, delay);
    }

    private void Init(Action onClose, int delay)
    {
        _onClose = onClose;
        
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }

        _timerCoroutine = StartCoroutine(TimerCoroutine(delay));
    }

    IEnumerator TimerCoroutine(int time)
    {
        int remainingTime = time;
        fill.DOKill();
        fill.fillAmount = 1;
        fill.DOFillAmount(0, time)
            .SetEase(Ease.Linear);

        while (remainingTime >= 0)
        {
            timer.text = $"Ads in {remainingTime}s";
            yield return Yielders.Get(1);
            remainingTime--;
        }
        
        _onClose?.Invoke();
        Close();
    }
    
    protected override void RegisterEvent()
    {
        base.RegisterEvent();
    }

    protected override void UnregisterEvent()
    {
        base.UnregisterEvent();
    }

    public override void OnDisappear()
    {
        base.OnDisappear();
        Instance = null;
    }
}