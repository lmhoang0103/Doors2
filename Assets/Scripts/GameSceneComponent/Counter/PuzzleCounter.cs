using Lofelt.NiceVibrations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCounter : BaseCounter {
    private const string UNINTERACTABLE_LAYER_NAME = "Default";
    public event EventHandler OnCorrectPuzzleItem;

    [SerializeField] private List<ItemType> puzzleItemTypesList;
    [SerializeField] private ItemObjectSO requiredPuzzleItemObjectSO;

    public override void Interact(Player player) {
        if (!HasItemObject()) {
            //There are no objects here
            if (player.HasItemObject()) {
                //Player is carrying a puzzle item
                //Put item in here, remove from inventory
                InventoryManager.Instance.RemoveItemFromInventory(player.GetItemObject().GetItemObjectSO(), 1);
                player.GetItemObject().SetItemObjectParent(this);
                if (IsCorrectPuzzleItemPlaced(this.GetItemObject().GetItemObjectSO())) {
                    HandlePuzzleCounterOnCorrect();
                }
            } else {

            }
        } else {
            //There is an object here
            if (player.HasItemObject()) {
                //This object is incorrect, because if it's correct, then player cannot interact with it
                //Move the Items player holding out of inventory, add the item on here in inventory
                InventoryManager.Instance.RemoveItemFromInventory(player.GetItemObject().GetItemObjectSO(), 1);
                InventoryManager.Instance.AddItemToInventory(GetItemObject().GetItemObjectSO(), 1);

                ItemObject.SwapItemObject(player, this);
                if (IsCorrectPuzzleItemPlaced(this.GetItemObject().GetItemObjectSO())) {
                    //Player swap correct item in
                    HandlePuzzleCounterOnCorrect();
                }
            } else {
                //Player Pick up the object
                InventoryManager.Instance.AddItemToInventory(GetItemObject().GetItemObjectSO(), 1);

                GetItemObject().DestroySelf();
            }
        }
    }


    public override bool IsInteractable(Player player) {
        //Only allow interact when player or this have an Item
        //Only allow player to interact when holding correct itemType for this puzzle
        if (HasItemObject()) {
            return true;
        }
        if (!HasItemObject() && !player.HasItemObject()) {
            return false;
        } else if (player.HasItemObject()) {
            if (puzzleItemTypesList.Contains(player.GetItemObject().GetItemObjectSO().itemType)) {

                return true;
            }
        }
        return false;
    }



    protected virtual bool IsCorrectPuzzleItemPlaced(ItemObjectSO itemObjectSO) {

        if (itemObjectSO != requiredPuzzleItemObjectSO) {
            CinemachineShake.Instance.ErrorCameraShake();
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
            SoundManager.Instance.PlayPuzzlefailSound(this.transform.position);
            return false;
        } else {
            SoundManager.Instance.PlayPuzzleSuccessSound(this.transform.position);
            return true;
        }
    }

    protected void HandlePuzzleCounterOnCorrect() {
        OnCorrectPuzzleItem?.Invoke(this, EventArgs.Empty);
        gameObject.layer = LayerMask.NameToLayer(UNINTERACTABLE_LAYER_NAME);
    }
}
