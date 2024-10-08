public class PopupNoInternet : UIPanel
{
    public override UiPanelType GetId()
    {
        return UiPanelType.PopupNoInternet;
    }

    public static void Show()
    {
        var newInstance = (PopupNoInternet) GUIManager.Instance.NewPanel(UiPanelType.PopupNoInternet);
        newInstance.OnAppear();
    }

    public override void OnAppear()
    {
        base.OnAppear();
        Invoke(nameof(Close), 3f);
    }
}