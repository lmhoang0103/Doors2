using UnityEngine;

public class ClearCounter : BaseCounter {
    private const string UNINTERACTABLE_LAYER_NAME = "Default";

    [SerializeField] private ItemObjectSO itemObjectSOToSpawn;
    [SerializeField] private LootTableSO lootTableSO;

    protected override void Awake() {
        base.Awake();
        lootTableSO = DoorGameManager.Instance.GetLootTableSO();
        if (itemObjectSOToSpawn != null) {
            ItemObject.SpawnItemObjectSO(itemObjectSOToSpawn, this);
        } else {
            itemObjectSOToSpawn = lootTableSO.GetRandomItem();
            if (itemObjectSOToSpawn != null) {
                ItemObject.SpawnItemObjectSO(itemObjectSOToSpawn, this);
            }
        }
        ResetGameOBject();
    }

    public override bool IsInteractable(Player player) {
        return HasItemObject();
    }
    public override void Interact(Player player) {
        //Put this in inventory
        InventoryManager.Instance.AddItemToInventory(GetItemObject().GetItemObjectSO(), 1);
        //Destroy the item here
        GetItemObject().DestroySelf();
        SoundManager.Instance.PlayItemPickupSound(this.transform.position);

    }
    private void ResetGameOBject() {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

}
