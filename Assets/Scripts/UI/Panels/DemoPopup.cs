public class DemoPopup : UIPanel, IDemoPanel
{
    public static DemoPopup Instance { get; private set; }

    public override UiPanelType GetId()
    {
        return UiPanelType.DemoPopup;
    }

    public static void Show()
    {
        var newInstance = (DemoPopup) GUIManager.Instance.NewPanel(UiPanelType.DemoPopup);
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