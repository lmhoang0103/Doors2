using UnityEngine;

public class KeyHolder : BaseCounter
{
    private const string UNINTERACTABLE_LAYER_NAME = "Default";

    [SerializeField] private ItemObjectSO keyObjectSOToSpawn;

    private void Start()
    {
        if (keyObjectSOToSpawn != null)
        {
            if (keyObjectSOToSpawn.itemType == ItemType.NormalKey || keyObjectSOToSpawn.itemType == ItemType.FakeKey)
            {
                ItemObject.SpawnItemObjectSO(keyObjectSOToSpawn, this);
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
