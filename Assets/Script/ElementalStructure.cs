using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ElementState
{
    Solid,
    Liquid,
    Gas
}

[System.Serializable]
[CreateAssetMenu(fileName = "NewElemental", menuName = "Elemental/NewElemental")]
public class ElementalStructure : ScriptableObject
{
    public int id;
    public int stack_ = 1;
    public int stack_Count = 1;
    public int price;
    public Sprite sprite;
    public string name_;
    public string description_;
    public string symbol_;
    public ElementState elementState_ = ElementState.Solid;

    public int atom_number = 0;
    public float atom_mass = 0;
    public float weight = 0;  // Fixed typo from "wight" to "weight"
    public int photron_;
    public int electron_;
    public Vector2 pos_in_table;

    public void AddToInventory(Player player)
    {
        stack_ = Mathf.Max(stack_, 1); // Ensure valid stack

        // Check if item already exists in inventory
        ElementalStructure existingItem = player.elementals_indungeon.FirstOrDefault(item => item.id == id);

        if (existingItem != null)
        {
            existingItem.stack_Count += stack_;
            return;
        }

        // Create a new instance manually to avoid modifying the original ScriptableObject
        ElementalStructure clonedItem = ScriptableObject.CreateInstance<ElementalStructure>();
        clonedItem.id = id;
        clonedItem.stack_ = stack_;
        clonedItem.stack_Count = 1;
        clonedItem.price = price;
        clonedItem.sprite = sprite;
        clonedItem.name_ = name_;
        clonedItem.description_ = description_;
        clonedItem.symbol_ = symbol_;
        clonedItem.elementState_ = elementState_;
        clonedItem.atom_number = atom_number;
        clonedItem.atom_mass = atom_mass;
        clonedItem.weight = weight;
        clonedItem.photron_ = photron_;
        clonedItem.electron_ = electron_;
        clonedItem.pos_in_table = pos_in_table;

        // Add to inventory and update UI
        player.elementals_indungeon.Add(clonedItem);
        player.CreateSlotUIElement();
    }
}
