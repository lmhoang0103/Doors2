using System;
using UnityEngine;

public class Switch : MonoBehaviour, IInteractable
{
    public event EventHandler OnSwitchTurnOn;
    public event EventHandler OnswitchTurnOff;

    private enum SwitchState
    {
        SwitchOn,
        SwitchOff,
    }

    [SerializeField] private SwitchState switchState;

    private void Start()
    {
        switch (switchState)
        {
            case SwitchState.SwitchOn:
                OnSwitchTurnOn?.Invoke(this, EventArgs.Empty);
                break;
            case SwitchState.SwitchOff:
                OnswitchTurnOff?.Invoke(this, EventArgs.Empty);
                break;
        }
    }


    public void Interact(Player player)
    {
        switch (switchState)
        {
            case SwitchState.SwitchOn:
                SwitchTurnOff();
                break;
            case SwitchState.SwitchOff:
                SwitchTurnOn();
                break;
        }
    }
    private void SwitchTurnOff()
    {
        switchState = SwitchState.SwitchOff;
        OnswitchTurnOff?.Invoke(this, EventArgs.Empty);
    }
    private void SwitchTurnOn()
    {
        switchState = SwitchState.SwitchOn;
        OnSwitchTurnOn?.Invoke(this, EventArgs.Empty);
    }
    public bool IsInteractable(Player player)
    {
        return true;
    }
}
