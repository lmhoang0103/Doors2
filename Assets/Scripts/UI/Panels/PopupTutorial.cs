public class PopupTutorial : UIPanel
{
    public static PopupTutorial Instance { get; private set; }

    public override UiPanelType GetId()
    {
        return UiPanelType.PopupTutorial;
    }

    public static void Show()
    {
        var newInstance = (PopupTutorial) GUIManager.Instance.NewPanel(UiPanelType.PopupTutorial);
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
}