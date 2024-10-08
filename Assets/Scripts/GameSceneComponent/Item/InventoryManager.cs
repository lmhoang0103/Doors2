using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] private InventoryItemObjectListSO inventoryItemListSO;

    private Dictionary<ItemObjectSO, int> inventoryDictionary = new Dictionary<ItemObjectSO, int>();

    private void Start()
    {
        foreach (ItemObjectSO item in inventoryItemListSO.inventoryItemSOList)
        {
            inventoryDictionary.Add(item, 0);
        }
    }

    public void AddItemToInventory(ItemObjectSO item, int quantity)
    {
        // Check if the item is in the inventory dictionary
        if (inventoryDictionary.ContainsKey(item))
        {
            // Increment the quantity of the item in play
            inventoryDictionary[item] += quantity;
        } else
        {
            Debug.LogError("Item is not in the inventory item list SO!");
        }
    }

    public void RemoveItemFromInventory(ItemObjectSO item, int quantity)
    {
        // Check if the item is in the inventory dictionary
        if (inventoryDictionary.ContainsKey(item))
        {
            // Decrement the quantity of the item in play
            inventoryDictionary[item] -= quantity;
            if (inventoryDictionary[item] < 0)
            {
                inventoryDictionary[item] = 0;
            }
        } else
        {
            Debug.LogError("Item is not in the inventory item list SO!");
        }
    }
}
