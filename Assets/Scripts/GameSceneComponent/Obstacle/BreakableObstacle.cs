using UnityEngine;

public class BreakableObstacle : MonoBehaviour, IInteractable {
    public bool IsInteractable(Player player) {
        //Only allow to be interact with if player is holding a hammer
        if (player.HasItemObject()) {
            if (player.GetItemObject().GetItemObjectSO().itemType == ItemType.Hammer) {
                return true;
            }
        }
        return false;
    }
    public void Interact(Player player) {
        //Remove Hammer
        Destroy(gameObject);
        InventoryManager.Instance.RemoveItemFromInventory(player.GetItemObject().GetItemObjectSO(), 1);
        player.GetItemObject().DestroySelf();
        SoundManager.Instance.PlayPuzzleSuccessSound(this.transform.position);
    }


    public void Break() {
        Destroy(gameObject);
    }
}
