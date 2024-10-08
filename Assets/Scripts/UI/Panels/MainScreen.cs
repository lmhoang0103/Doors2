#region

using TMPro;
using UnityEngine;

#endregion

public class MainScreen : UIPanel
{
    public static MainScreen Instance { get; private set; }

    [SerializeField]
    public GameObject btnNoAds;

    public override UiPanelType GetId()
    {
        return UiPanelType.MainScreen;
    }

    public static void Show()
    {
        var newInstance = (MainScreen) GUIManager.Instance.NewPanel(UiPanelType.MainScreen);
        Instance = newInstance;
        newInstance.OnAppear();
    }

    public override void OnAppear()
    {
        if (isInited)
            return;

        base.OnAppear();

        Init();
    }

    private void Init()
    {
        UpdateNoAdsButton();
    }

    public void ShowSetting()
    {
        AudioAssistant.Shot(TypeSound.Button);
        PopupSetting.Show();
    }

    public void StartGame()
    {
        AudioAssistant.Shot(TypeSound.Button);

        if (!GameManager.NetworkAvailable)
        {
            PopupNoInternet.Show();
            return;
        }

        PlayScreen.Show();
    }

    void UpdateNoAdsButton() => btnNoAds.SetActive(!Gm.data.user.purchasedNoAds);

    public void OnBuyNoAds()
    {
        AudioAssistant.Shot(TypeSound.Button);
        IAPManager.Instance.BuyProduct(IapProductName.NoAds);
    }

    protected override void RegisterEvent()
    {
        Evm.OnPurchaseNoAds.AddListener(UpdateNoAdsButton);
        
        base.RegisterEvent();
    }

    protected override void UnregisterEvent()
    {
        Evm.OnPurchaseNoAds.RemoveListener(UpdateNoAdsButton);
        
        base.UnregisterEvent();
    }

    public override void OnDisappear()
    {
        base.OnDisappear();
        Instance = null;
    }
}