#region
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion

public class GUIManager : Singleton<GUIManager>
{
    private readonly Dictionary<UiPanelType, UIPanel> _initiedPanels = new Dictionary<UiPanelType, UIPanel>();

    private readonly Queue<Action> _queuePopup = new Queue<Action>();

    private readonly List<UIPanel> _showingNotifications = new List<UIPanel>();

    private readonly List<UIPanel> _showingPopups = new List<UIPanel>();

    private Stack<UIPanel> _screenStack = new Stack<UIPanel>();

    [SerializeField]
    public Canvas layerLoading;

    [SerializeField]
    public Canvas layerNotify;

    [SerializeField]
    public Canvas layerPopup;

    [SerializeField]
    public Canvas layerScreen;

    [SerializeField]
    private Camera mainCamera;

    public Canvas root;

    protected override void Awake()
    {
        base.Awake();
        ClearGui();
    }

    public void ReloadCamera()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        root.worldCamera = mainCamera;
    }

    public void Init()
    {
        ReloadCamera();

        EventGlobalManager.Instance.OnStartLoadScene.AddListener(StartLoading);
        EventGlobalManager.Instance.OnFinishLoadScene.AddListener(FinishLoading);
        EventGlobalManager.Instance.OnFinishLoadScene.AddListener(ReloadCamera);
    }

    public UIPanel NewPanel(UiPanelType id)
    {
        var type = id.ToString().GetPanelType();

        UIPanel newPanel = null;
        if (_initiedPanels.ContainsKey(id))
        {
            newPanel = _initiedPanels[id];
        }
        else
        {
            newPanel = Instantiate(GetPrefab(id), GetRootByType(type).transform);
            _initiedPanels.Add(id, newPanel);
        }

        if (type == PanelType.Popup)
        {
            if (_showingPopups.Contains(newPanel))
                _showingPopups.Remove(newPanel);

            _showingPopups.Add(newPanel);
        }
        else if (type == PanelType.Notification)
        {
            if (!_showingNotifications.Contains(newPanel))
                _showingNotifications.Add(newPanel);
        }
        else
        {
            var currentScreen = GetCurrentScreen();
            if (currentScreen != null && currentScreen.GetId() != id && currentScreen.gameObject.activeSelf)
                currentScreen.Close();

            if (_screenStack.Contains(newPanel))
                _screenStack = MakeElementToTopStack(newPanel, _screenStack);
            else
                _screenStack.Push(newPanel);
        }

        newPanel.transform.SetAsLastSibling();
        newPanel.gameObject.SetActive(true);

        return newPanel;
    }

    public UIPanel GetCurrentScreen()
    {
        if (_screenStack.Count == 0)
            return null;

        return _screenStack.Peek();
    }

    public void GoBackLastScreen()
    {
        _screenStack.Pop().Close();

        if (GetCurrentScreen() == null || GetCurrentScreen().GetId() == UiPanelType.MainScreen)
        {
            MainScreen.Show();
        }
        else
        {
            var newPanel = NewPanel(GetCurrentScreen().GetId());
            newPanel.OnAppear();
        }
    }

    public void ClearGui()
    {
        foreach (var pair in _initiedPanels)
            pair.Value.Hide();

        DestroyChildren(layerScreen.transform);
        DestroyChildren(layerPopup.transform);
        DestroyChildren(layerNotify.transform);
    }

    public void Dismiss(UIPanel panel)
    {
        _showingPopups.Remove(panel);
        _showingNotifications.Remove(panel);

        /*if (GetCurrentScreen() == panel && GetCurrentScreen().GetId() != UiPanelType.MainScreen)
            MainScreen.Show();*/
    }

    public void DismissTopPopup()
    {
        var topPanel = GetTopPopup();
        if (topPanel == null)
            return;

        topPanel.Close();
        Dismiss(topPanel);
    }

    public void DismissPanelById(UiPanelType id)
    {
        var panel = GetPanel(id);
        if (panel == null)
            return;

        panel.Close();
        Dismiss(panel);
    }

    #region Loading
    public UILoading loadingUi;

    public void StartLoading()
    {
        loadingUi.root.gameObject.SetActive(true);
    }

    public void FinishLoading()
    {
        loadingUi.root.gameObject.SetActive(false);
    }
    #endregion

    public void CheckPopupQueue()
    {
        if (_showingPopups.Count == 0 && _queuePopup.Count > 0)
            if (_queuePopup.Peek() != null)
                _queuePopup.Dequeue().Invoke();
    }

    public void AddPopupQueue(Action action)
    {
        _queuePopup.Enqueue(action);

        CheckPopupQueue();
    }

    #region Utilities
    public UIPanel GetTopPopup()
    {
        if (_showingPopups.Count == 0)
            return null;

        return _showingPopups.GetLast();
    }

    private UIPanel GetPrefab(UiPanelType id)
    {
        if (ConfigManager.Instance.uiConfig.panelInstances == null)
            return null;

        return ConfigManager.Instance.uiConfig.panelInstances.FindLast(e => e.GetId().Equals(id));
    }

    private Canvas GetRootByType(PanelType type)
    {
        switch (type)
        {
            case PanelType.Screen:
                return layerScreen;
            case PanelType.Popup:
                return layerPopup;
            case PanelType.Notification:
                return layerNotify;
            case PanelType.Loading:
                return layerLoading;
        }

        return null;
    }

    public static IEnumerator TextAnim(TMP_Text label, long oldValue, long newValue)
    {
        var currentValue = oldValue;
        var speed = (newValue - oldValue) / 0.3f * Time.fixedDeltaTime / 2;
        if (Mathf.Abs(speed) > 1)
        {
            var currentProgress = 0f;
            while (currentProgress < 0.3f)
            {
                yield return new WaitForFixedUpdate();
                currentProgress += Time.fixedDeltaTime;
                currentValue += (long) speed;
                label.text = currentValue.ToFormatString();
            }
        }

        label.text = newValue.ToFormatString();
    }

    public static IEnumerator ProgressAnim(Image bar, float oldValue, float newValue)
    {
        var currentValue = oldValue;
        var speed = (newValue - oldValue) * Time.fixedDeltaTime;

        var currentProgress = 0f;
        while (currentProgress < 1f)
        {
            yield return new WaitForFixedUpdate();
            currentProgress += Time.fixedDeltaTime;
            currentValue += speed;
            bar.fillAmount = currentValue;
        }

        bar.fillAmount = newValue;
    }

    public void DestroyChildren(Transform transform)
    {
        var totalChild = transform.childCount;

        if (totalChild == 0)
            return;

        for (var i = totalChild - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }

    public UIPanel GetPanel(UiPanelType type)
    {
        if (_initiedPanels.ContainsKey(type))
            return _initiedPanels[type];

        return null;
    }

    public Stack<UIPanel> MakeElementToTopStack(UIPanel objectTop, Stack<UIPanel> stack)
    {
        var extraPanel = stack.ToArray();
        for (var i = 0; i < extraPanel.Length; i++)
            if (extraPanel[i] == objectTop)
            {
                for (var ii = i; ii > 0; ii--)
                    extraPanel[ii] = extraPanel[ii - 1];

                extraPanel[0] = objectTop;
            }

        Array.Reverse(extraPanel);
        return new Stack<UIPanel>(extraPanel);
    }

    public void ShowGui()
    {
        layerPopup.enabled = true;
        layerScreen.enabled = true;
    }

    public void HideGui()
    {
        layerPopup.enabled = false;
        layerScreen.enabled = false;
    }

    public RectTransform rootRect;
    #endregion
}