using System;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    private const string UNINTERACTABLE_LAYER_NAME = "Default";
    public event EventHandler OnLeverTurnOn;

    public void Interact(Player player)
    {
        OnLeverTurnOn?.Invoke(this, EventArgs.Empty);
        gameObject.layer = LayerMask.NameToLayer(UNINTERACTABLE_LAYER_NAME);
    }

    public bool IsInteractable(Player player)
    {
        return true;
    }

}
