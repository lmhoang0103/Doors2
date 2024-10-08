using UnityEngine;
public class BookHolder : BaseCounter
{
    private const string UNINTERACTABLE_LAYER_NAME = "Default";

    [SerializeField] private ItemObjectSO bookObjectSOToSpawn;

    private void Start()
    {
        if (bookObjectSOToSpawn != null)
        {
            if (bookObjectSOToSpawn.itemType == ItemType.Book)
            {
                ItemObject.SpawnItemObjectSO(bookObjectSOToSpawn, this);
            } else
            {
                gameObject.layer = LayerMask.NameToLayer(UNINTERACTABLE_LAYER_NAME);
            }

        } else
        {
            gameObject.layer = LayerMask.NameToLayer(UNINTERACTABLE_LAYER_NAME);
        }
        ResetGameOBject();
    }

    public override bool IsInteractable(Player player)
    {
        if (player.HasItemObject())
        {
            return false;
        }

        return true;
    }
    public override void Interact(Player player)
    {
        //Can Only take item when not holding anything
        GetItemObject().SetItemObjectParent(player);
        gameObject.SetActive(false);
    }

    private void ResetGameOBject()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
