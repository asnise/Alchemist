using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public enum ItemCategory
{
    Miscellaneous,
    Consumable,
}


[System.Serializable]
public class UseResource
{
    public int AtomNumber;
    public int StackCount;
}

[System.Serializable]
[CreateAssetMenu(fileName = "NewItem", menuName = "Item/NewItem")]
public class Item : ScriptableObject
{
    public int id;
    public Sprite sprite;
    public string itemName;
    public int price = 1;
    [TextArea]
    public string description;
    public ItemCategory category = ItemCategory.Miscellaneous;

    public int stack = 1;
    public int stackCount = 1;

    [SerializeField]
    public List<UseResource> crafting_resources;

    public void AddToInventory(Player player)
    {
        // Ensure valid stack count
        stack = Mathf.Max(stack, 1);

        // Check if the item is stackable
        if (category is ItemCategory.Miscellaneous or ItemCategory.Consumable)
        {
            Item existingItem = player.items_indungeon.FirstOrDefault(item => item.id == id);

            if (existingItem != null)
            {
                existingItem.stackCount += stack;
                return;
            }
        }

        // Clone the item manually since ScriptableObject.Instantiate can cause issues
        Item clonedItem = CreateInstance<Item>();
        clonedItem.id = id;
        clonedItem.sprite = sprite;
        clonedItem.itemName = itemName;
        clonedItem.price = price;
        clonedItem.description = description;
        clonedItem.category = category;
        clonedItem.stack = stack;
        clonedItem.stackCount = 1;
        clonedItem.crafting_resources = new List<UseResource>(crafting_resources); // Deep copy list

        // Add to inventory and update UI
        player.items_indungeon.Add(clonedItem);
        player.CreateSlotUIItem();
    }

}
