using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem {
    private ItemObjectSO itemObjectSO;
    private int amount;

    public InventoryItem(ItemObjectSO itemObjectSO) {
        this.itemObjectSO = itemObjectSO;
        amount = 0;
    }

    public void AddToStack(int numberOfItem) {
        amount = amount + numberOfItem;
    }
    public void RemoveFromStack(int numberOfItem) {
        amount = amount - numberOfItem;
    }

    public ItemObjectSO GetItemObjectSO() { return itemObjectSO; }
    public int GetItemAmount() { return amount; }


}

public class InventoryManager : SingletonDestroy<InventoryManager> {

    [SerializeField] private ItemObjectSO coinItemObjectSO;
    [SerializeField] private InventoryUI inventoryUi;


    private List<InventoryItem> inventory = new List<InventoryItem>();
    private Dictionary<ItemObjectSO, InventoryItem> inventoryItemDictionary = new Dictionary<ItemObjectSO, InventoryItem>();


    protected override void Awake() {
        base.Awake();
    }

    public void PopulateInventory() {
        //Set whatever item player hold before game start here
        AddItemToInventory(coinItemObjectSO, SaveManager.Instance.save.Coin);
    }

    //Add item to inventory
    //Can't add if item is unstackable and already hold item
    public void AddItemToInventory(ItemObjectSO itemObjectSO, int numberOfItem = 1) {

        if (inventoryItemDictionary.TryGetValue(itemObjectSO, out InventoryItem foundInventoryItem)) {
            //If Inventory already contain this item
            //Can only add if it is stackable
            if (itemObjectSO.isStackable) {
                foundInventoryItem.AddToStack(numberOfItem);
            }

        } else {
            //Add New Item
            InventoryItem newInventoryItem = new InventoryItem(itemObjectSO);
            newInventoryItem.AddToStack(numberOfItem);
            inventory.Add(newInventoryItem);
            inventoryItemDictionary.Add(itemObjectSO, newInventoryItem);
        }



        inventoryUi.SetInventory(inventory);
    }

    public void RemoveItemFromInventory(ItemObjectSO itemObjectSO, int numberOfItem = 1) {

        if (inventoryItemDictionary.TryGetValue(itemObjectSO, out InventoryItem foundiInventoryItem)) {
            //If Inventory already contain this item
            //Remove if possible
            if (itemObjectSO.isStackable) {
                //If stackable, remove the amount, if amount == 0 then remove from inventory
                foundiInventoryItem.RemoveFromStack(numberOfItem);
                if (foundiInventoryItem.GetItemAmount() <= 0) {
                    inventory.Remove(foundiInventoryItem);
                    inventoryItemDictionary.Remove(itemObjectSO);
                }
            } else {
                //Not stackable, remove from inventory
                inventory.Remove(foundiInventoryItem);
                inventoryItemDictionary.Remove(itemObjectSO);
            }

        } else {
            //This item is not in inventory
        }



        inventoryUi.SetInventory(inventory);
    }
    public List<InventoryItem> GetInventoryItemList() {
        return inventory;
    }

    public int GetCoinCount() {
        foreach (InventoryItem item in inventory) {
            if (item.GetItemObjectSO().itemType == ItemType.Coin) {
                return item.GetItemAmount();
            }
        }
        return 0;

    }
}
