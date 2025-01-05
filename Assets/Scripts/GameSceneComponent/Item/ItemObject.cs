using System;
using UnityEngine;

public class ItemObject : MonoBehaviour {
    [SerializeField] private ItemObjectSO itemObjectSO;

    private IItemObjectParent itemObjectParent;

    public ItemObjectSO GetItemObjectSO() { return itemObjectSO; }

    public void SetItemObjectParent(IItemObjectParent itemObjectParent) {
        this.itemObjectParent?.ClearItemObject();
        this.itemObjectParent = itemObjectParent;

        itemObjectParent.SetItemObject(this);

        transform.parent = itemObjectParent.GetItemObjectFollowTransform();
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public IItemObjectParent GetItemObjectParent() { return itemObjectParent; }

    public void DestroySelf() {
        itemObjectParent.ClearItemObject();
        Destroy(gameObject);
    }


    public static ItemObject SpawnItemObjectSO(ItemObjectSO itemObjectSO, IItemObjectParent itemObjectParent) {
        Transform itemObjectTransform = Instantiate(itemObjectSO.prefab);
        ItemObject itemObject = itemObjectTransform.GetComponent<ItemObject>();
        itemObject.SetItemObjectParent(itemObjectParent);
        return itemObject;

    }



    public static void SwapItemObject(IItemObjectParent itemObjectParent1, IItemObjectParent itemObjectParent2) {
        // Retrieve the ItemObject components from the IItemObjectParent objects
        ItemObject itemObject1 = itemObjectParent1.GetItemObject();
        ItemObject itemObject2 = itemObjectParent2.GetItemObject();

        // Retrieve the original parent objects of the ItemObject components
        IItemObjectParent originalParent1 = itemObject1.GetItemObjectParent();
        IItemObjectParent originalParent2 = itemObject2.GetItemObjectParent();

        // Swap the ItemObject components between the two IItemObjectParent objects
        itemObject1.SetItemObjectParent(itemObjectParent2);
        itemObject2.SetItemObjectParent(itemObjectParent1);

        // Update the original parent objects of the ItemObject components
        originalParent1.SetItemObject(itemObject2);
        originalParent2.SetItemObject(itemObject1);
    }

}
