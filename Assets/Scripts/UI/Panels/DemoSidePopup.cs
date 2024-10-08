public class DemoSidePopup : UIPanel, IDemoPanel
{
    public static DemoSidePopup Instance { get; private set; }

    public override UiPanelType GetId()
    {
        return UiPanelType.DemoSidePopup;
    }

    public static void Show()
    {
        var newInstance = (DemoSidePopup) GUIManager.Instance.NewPanel(UiPanelType.DemoSidePopup);
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