#region

using HyperCatSdk;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class PopupSetting : UIPanel
{
    public static PopupSetting Instance { get; private set; }

    [SerializeField]
    private Button btnRestorePurchase;

    [SerializeField]
    private Button btnSound;

    [SerializeField]
    private Button btnVibrate;

    [SerializeField]
    private Button closeBg;

    [SerializeField]
    private Button closeButton;

    [SerializeField]
    private GameObject soundOff;

    [SerializeField]
    private GameObject soundOn;

    [SerializeField]
    private GameObject vibrateOff;

    [SerializeField]
    private GameObject vibrateOn;

    public override UiPanelType GetId()
    {
        return UiPanelType.PopupSetting;
    }

    public static void Show()
    {
        var newInstance = (PopupSetting) GUIManager.Instance.NewPanel(UiPanelType.PopupSetting);
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
        vibrateOn.SetActive(Gm.data.setting.haptic);
        vibrateOff.SetActive(!Gm.data.setting.haptic);

        soundOn.SetActive(Gm.data.setting.soundVolume > 0);
        soundOff.SetActive(Gm.data.setting.soundVolume == 0);
    }

    public void SwitchSound()
    {
        Gm.data.setting.soundVolume = Gm.data.setting.soundVolume > 0 ? 0 : 1;

        soundOn.SetActive(Gm.data.setting.soundVolume > 0);
        soundOff.SetActive(Gm.data.setting.soundVolume == 0);

        Database.SaveData();
        EventGlobalManager.Instance.OnUpdateSetting.Dispatch();

        AudioAssistant.Shot(TypeSound.Button);
    }

    public void SwitchVibrate()
    {
        Gm.data.setting.haptic = !Gm.data.setting.haptic;
        HCVibrate.Haptic(HcHapticTypes.RigidImpact);

        vibrateOn.SetActive(Gm.data.setting.haptic);
        vibrateOff.SetActive(!Gm.data.setting.haptic);

        Database.SaveData();
        EventGlobalManager.Instance.OnUpdateSetting.Dispatch();

        AudioAssistant.Shot(TypeSound.Button);
    }

    public void OnClickRestorePurchase()
    {
        AudioAssistant.Shot(TypeSound.Button);
#if UNITY_IOS
        IAPManager.Instance.RestorePurchases();
#endif
    }

    protected override void RegisterEvent()
    {
        base.RegisterEvent();
        closeButton.onClick.AddListener(Close);
        closeBg.onClick.AddListener(Close);
    }

    protected override void UnregisterEvent()
    {
        base.UnregisterEvent();
        closeButton.onClick.RemoveListener(Close);
        closeBg.onClick.RemoveListener(Close);
    }

    public override void OnDisappear()
    {
        base.OnDisappear();
        Instance = null;
    }

    public override void Close()
    {
        base.Close();
    }
}