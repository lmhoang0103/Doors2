using System;
using UnityEngine;

public class DoorUnlockedByKey : BaseDoor, IHintable, IInteractable
{
    [SerializeField] private ItemObjectSO itemObjectSONeededToUnlockDoor;

    [Range(0f, 1f)]
    [SerializeField] private float shakingDuration;
    [Range(0f, 1f)]
    [SerializeField] private float shakingMagnitude;

    public event EventHandler OnHintShow;
    public event EventHandler OnHintHidden;

    public void Interact(Player player)
    {
        if (!player.HasItemObject())
        {
            //If Player dont hold anything
            OnHintShow?.Invoke(this, EventArgs.Empty);

        } else
        {
            //If Player hold something, check if this is the right item required to open door
            if (player.GetItemObject().GetItemObjectSO() == itemObjectSONeededToUnlockDoor)
            {
                OpenDoor();
                player.GetItemObject().DestroySelf();
                OnHintHidden?.Invoke(this, EventArgs.Empty);
            } else
            {
                Debug.Log("Wrong Item");
                player.GetItemObject().DestroySelf();

                if (Camera.main.TryGetComponent(out StressReceiverModify stressReceiver))
                {
                    stressReceiver.StartCoroutine(stressReceiver._ShakeCamera(shakingDuration, shakingMagnitude));
                } else
                {
                    Debug.LogError("StressReceiverModify component not found on the main camera.");
                }
                OnHintShow?.Invoke(this, EventArgs.Empty);
            };
        }
    }

    public bool IsInteractable(Player player)
    {
        return true;
    }
}
