using UnityEngine;

public class ClearCounter : BaseCounter
{
    private const string UNINTERACTABLE_LAYER_NAME = "Default";

    [SerializeField] private ItemObjectSO itemObjectSOToSpawn;



    private void Start()
    {
        if (itemObjectSOToSpawn != null)
        {
            ItemObject.SpawnItemObjectSO(itemObjectSOToSpawn, this);
        } else
        {
            gameObject.layer = LayerMask.NameToLayer(UNINTERACTABLE_LAYER_NAME);
        }
    }

    public override void Interact(Player player)
    {
        if (player.HasItemObject())
        {
            //Player is carrying something
            ItemObject.SwapItemObject(player, this);

        } else
        {
            //Player is not carrying anything
            GetItemObject().SetItemObjectParent(player);
            gameObject.layer = LayerMask.NameToLayer(UNINTERACTABLE_LAYER_NAME);
        }

    }

}
