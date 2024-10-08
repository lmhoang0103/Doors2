public class DemoBottomPopup : UIPanel, IDemoPanel
{
    public static DemoBottomPopup Instance { get; private set; }

    public override UiPanelType GetId()
    {
        return UiPanelType.DemoBottomPopup;
    }

    public static void Show()
    {
        var newInstance = (DemoBottomPopup) GUIManager.Instance.NewPanel(UiPanelType.DemoBottomPopup);
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

    public void DemoShow()
    {
        Show();
    }
}