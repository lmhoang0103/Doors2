using UnityEngine;

public class PuzzleCounterPlacementPreview : MonoBehaviour, IItemObjectParent
{
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private BasePuzzleCounter basePuzzleCounter;

    private ItemObject itemObject;
    private IInteractable interactable;
    private IItemObjectParent itemObjectParent;
    private void Start()
    {
        if (!basePuzzleCounter.TryGetComponent(out interactable))
        {
            Debug.LogError($"GameObject {basePuzzleCounter.name} does not have a component that implements the IInteractable interface.");
        }
        if (!basePuzzleCounter.TryGetComponent(out itemObjectParent))
        {
            Debug.LogError($"GameObject {basePuzzleCounter.name} does not have a component that implements the IInteractable interface.");
        }
        Player.Instance.OnSelectedInteractableObjectChanged += Player_OnSelectedInteractableObjectChanged;
        DestroyItemPlacementPreview();

    }

    private void Player_OnSelectedInteractableObjectChanged(object sender, Player.OnSelectedInteractableObjectChangedEventArgs e)
    {

        if (e.interactable == interactable && CanPlaceItemPreview())
        {
            SpawnItemPlacementPreview();
        } else
        {
            DestroyItemPlacementPreview();
        }



    }

    private bool CanPlaceItemPreview()
    {
        if (Player.Instance.HasItemObject())
        {
            if (!itemObjectParent.HasItemObject())
            {
                return true;
            }
        }

        return false;

    }
    private void SpawnItemPlacementPreview()
    {
        if (!this.HasItemObject())
        {
            ItemObject itemObject = ItemObject.SpawnItemObjectSO(Player.Instance.GetItemObject().GetItemObjectSO(), this);
            itemObject.RaiseOnEnableItemPlacementPreviewEvent();

        }
    }
    private void DestroyItemPlacementPreview()
    {

        if (this.HasItemObject())
        {
            itemObject.DestroySelf();
        }
    }

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
}
