using UnityEngine;

public class BaseCounter : HCMonoBehaviour, IItemObjectParent, IInteractable
{

    [SerializeField] private Transform counterTopPoint;

    private ItemObject itemObject;


    public Transform GetItemObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetItemObject(ItemObject itemObject)
    {
        this.itemObject = itemObject;
    }

    public ItemObject GetItemObject()
    {
        return itemObject;
    }

    public void ClearItemObject()
    {
        itemObject = null;
    }

    public bool HasItemObject()
    {
        return itemObject != null;
    }

    public virtual void Interact(Player player)
    {
        Debug.LogError("Not Implement Interact of Counter");


    }

    public virtual bool IsInteractable(Player player)
    {
        return true;
    }
}
