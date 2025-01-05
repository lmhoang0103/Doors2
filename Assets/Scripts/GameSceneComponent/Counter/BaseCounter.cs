using UnityEngine;

public class BaseCounter : MonoBehaviour, IItemObjectParent, IInteractable {
    private const string ITEM_HOLD_POINT_NAME = "ItemHoldPoint";

    private Transform itemHoldPoint;

    private ItemObject itemObject;

    protected virtual void Awake() {
        itemHoldPoint = transform.Find(ITEM_HOLD_POINT_NAME);
    }


    public virtual void Interact(Player player) {
        Debug.LogError("Not Implement Interact of Counter");

    }

    public virtual bool IsInteractable(Player player) {
        return true;
    }
    public Transform GetItemObjectFollowTransform() {
        return itemHoldPoint;
    }

    public void SetItemObject(ItemObject itemObject) {
        this.itemObject = itemObject;
    }

    public ItemObject GetItemObject() {
        return itemObject;
    }

    public void ClearItemObject() {
        itemObject = null;
    }

    public bool HasItemObject() {
        return itemObject != null;
    }

}
