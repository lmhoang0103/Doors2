#region

using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

#endregion

public class UIPanel : HCMonoBehaviour
{
    protected GUIManager gui;

    protected bool isInited;

    public Canvas root;
    public TypeSound openSound = TypeSound.OpenPopup;
    public TypeSound closeSound = TypeSound.ClosePopup;
    public PanelAnim panelAnimator;

    public virtual UiPanelType GetId()
    {
        return UiPanelType.None;
    }

    private void Awake()
    {
        gui = GUIManager.Instance;
        if (panelAnimator)
            panelAnimator.Setup(this);
    }

    public virtual void OnAppear()
    {
        if (panelAnimator)
            panelAnimator.StartAnimIn();

        RegisterEvent();
        AudioAssistant.Shot(openSound);
    }

    public virtual void OnDisappear()
    {
        if (panelAnimator)
            panelAnimator.StartAnimOut();

        UnregisterEvent();
        AudioAssistant.Shot(closeSound);
        isInited = false;
    }

    public virtual void Close()
    {
        OnDisappear();
        if (panelAnimator == null || panelAnimator.animOut == PanelAnim.AnimOutType.None)
            Hide();
        gui.Dismiss(this);
        gui.CheckPopupQueue();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    protected virtual void RegisterEvent()
    {
    }

    protected virtual void UnregisterEvent()
    {
    }
}