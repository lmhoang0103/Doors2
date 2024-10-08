#region

using TMPro;

#endregion

public class PopupNotification : UIPanel
{
    public TMP_Text txtMessage;

    public override UiPanelType GetId()
    {
        return UiPanelType.PopupNotification;
    }

    public static void Show(string message, float duration = 3f)
    {
        var newInstance = (PopupNotification) GUIManager.Instance.NewPanel(UiPanelType.PopupNotification);
        newInstance.OnAppear(message, duration);
    }

    private void OnAppear(string message, float duration)
    {
        base.OnAppear();
        txtMessage.text = message;
        Invoke(nameof(Close), duration);
    }
}