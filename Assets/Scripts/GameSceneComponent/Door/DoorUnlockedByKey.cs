using Lofelt.NiceVibrations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlockedByKey : BaseDoor, IHintable, IInteractable {
    [SerializeField] private List<ItemObjectSO> itemObjectSONeededToUnlockDoor;
    [SerializeField] private ItemObjectSO fakeKeySO;



    public event EventHandler OnHintShow;
    public event EventHandler OnHintHidden;

    public void Interact(Player player) {
        if (!player.HasItemObject()) {
            //If Player dont hold anything
            OnHintShow?.Invoke(this, EventArgs.Empty);

        } else {
            //If Player hold something, check if this is the right item required to open door
            if (IsPlayerHoldCorrectItem(player.GetItemObject().GetItemObjectSO())) {
                OpenDoor();
                InventoryManager.Instance.RemoveItemFromInventory(player.GetItemObject().GetItemObjectSO(), 1);
                player.GetItemObject().DestroySelf();
                //Remove fake item from inventory
                InventoryManager.Instance.RemoveItemFromInventory(fakeKeySO, 1);

                OnHintHidden?.Invoke(this, EventArgs.Empty);
            } else {
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);

                CinemachineShake.Instance.ErrorCameraShake();
                SoundManager.Instance.PlayPuzzlefailSound(this.transform.position);
                //Remove item if this is fake key
                if (player.GetItemObject().GetItemObjectSO() == fakeKeySO) {
                    InventoryManager.Instance.RemoveItemFromInventory(fakeKeySO, 1);
                    player.GetItemObject().DestroySelf();



                }
                OnHintShow?.Invoke(this, EventArgs.Empty);

            };
        }
    }

    private bool IsPlayerHoldCorrectItem(ItemObjectSO itemObjectSOCheck) {
        foreach (ItemObjectSO itemObjectSO in itemObjectSONeededToUnlockDoor) {
            if (itemObjectSOCheck == itemObjectSO) {
                return true;
            }
        }
        return false;
    }

    public bool IsInteractable(Player player) {
        return true;
    }
}
