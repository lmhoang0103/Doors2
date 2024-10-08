using System;
using TMPro;
using UnityEngine;

public class PopupConsent : UIPanel
{
    [SerializeField] private TMP_Text title;
    
    public static PopupConsent Instance { get; private set; }

    public override UiPanelType GetId()
    {
        return UiPanelType.PopupConsent;
    }

    private Action _onConsentUpdated;
    
    public static void Show(Action onConsentUpdated)
    {
        var newInstance = (PopupConsent) GUIManager.Instance.NewPanel(UiPanelType.PopupConsent);
        Instance = newInstance;
        newInstance.OnAppear(onConsentUpdated);
    }

    public void OnAppear(Action onConsentUpdated)
    {
        if (isInited)
            return;

        base.OnAppear();

        Init(onConsentUpdated);
    }

    private void Init(Action onConsentUpdated)
    {
        _onConsentUpdated = onConsentUpdated;

        title.text = $"Thanks for playing\n\"{Gm.gameSetting.gameName}\"";
    }

    public void OnConsentUpdate(bool agree)
    {
        IronSource.Agent.setConsent(agree);
        Gm.data.user.isConsentUpdated = true;
        _onConsentUpdated?.Invoke();
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