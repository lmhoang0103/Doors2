using System.Collections.Generic;
using UnityEngine;

public class DemoShopScreen : UIPanel, IDemoPanel
{
    [SerializeField] private List<DemoTabButton> tabButtons;

    public static DemoShopScreen Instance { get; private set; }

    public override UiPanelType GetId()
    {
        return UiPanelType.ScreenDemoShop;
    }

    public static void Show()
    {
        var newInstance = (DemoShopScreen) GUIManager.Instance.NewPanel(UiPanelType.ScreenDemoShop);
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
        SelectTabs(0);
    }

    public void SelectTabs(int tabIndex)
    {
        for (int i = 0; i < tabButtons.Count; i++)
        {
            tabButtons[i].SetSelected(i == tabIndex);
        }
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