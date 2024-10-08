using UnityEngine;

public interface IItemObjectParent
{

    public Transform GetItemObjectFollowTransform();


    public void SetItemObject(ItemObject itemObject);

    public ItemObject GetItemObject();

    public void ClearItemObject();

    public bool HasItemObject();
}
