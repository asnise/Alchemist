using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlchemiteReader : MonoBehaviour
{
    public Player player;
    public GameObject ContentViewerSlot;
    public GameObject FurnaceViewerSlot;
    public TextMeshProUGUI Description_;

    public GameObject ItemSlotPrefab;
    public GameObject Crafting_recieViewerSlot;

    public List<GameObject> Itemslot_view;

    public List<GameObject> ItemSlotCrafting_recie_view;

    public List<GameObject> FurnaceSlot_view;
    public List<ElementalStructure> inFurnace;

    public List<Item> items_recipe;

    public AudioClip btn_sfx;

    public ItemReader itemReader;

    public enum InventoryCatagory
    {
        Elemental,
        Item
    }

    public void CombineAtom()
    {
        if (ItemSlotCrafting_recie_view != null)
        {
            foreach (GameObject gameObject_slot in ItemSlotCrafting_recie_view)
            {
                Destroy(gameObject_slot);
            }
            ItemSlotCrafting_recie_view.Clear();
        }

        List<UseResource> useResources = new List<UseResource>();

        foreach (ElementalStructure elem in inFurnace)
        {
            UseResource resource = new UseResource();
            resource.AtomNumber = elem.atom_number;
            resource.StackCount = elem.stack_Count;
            useResources.Add(resource);
        }

        foreach (Item item in itemReader.itemList)
        {
            if (item.crafting_resources.Count > 0)
            {
                bool sharesResource = false;
                foreach (UseResource resource in item.crafting_resources)
                {
                    if (useResources.Any(ur => ur.AtomNumber == resource.AtomNumber))
                    {
                        sharesResource = true;
                        break;
                    }
                }

                if (sharesResource)
                {
                    Debug.Log($"Item {item.name} shares at least one UseResource with the useResources list.");

                    GameObject slot = Instantiate(ItemSlotPrefab, Crafting_recieViewerSlot.transform);

                    string ItemRecipe_txt = item.itemName + "\n";
                    foreach (UseResource resource in item.crafting_resources)
                    {
                        ItemRecipe_txt += $"<color=#00FF7F>{GetElementalStructureByAtomNumber(resource.AtomNumber).symbol_}{resource.StackCount}</color> ";
                    }

                    slot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = ItemRecipe_txt;
                    slot.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                    slot.transform.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                    slot.transform.transform.GetChild(1).GetChild(3).gameObject.SetActive(true);

                    bool allResourcesMatch = item.crafting_resources.All(cr =>
                        useResources.Any(ur => ur.AtomNumber == cr.AtomNumber && ur.StackCount >= cr.StackCount));

                    slot.transform.GetChild(1).GetComponent<Button>().interactable = allResourcesMatch;

                    slot.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
                    {
                        foreach (UseResource resource in item.crafting_resources)
                        {
                            var matchingElement = inFurnace.Find(e => e.atom_number == resource.AtomNumber && e.stack_Count >= resource.StackCount);
                            if (matchingElement != null)
                            {
                                matchingElement.stack_Count -= resource.StackCount;
                                if (matchingElement.stack_Count <= 0)
                                {
                                    inFurnace.Remove(matchingElement);
                                }
                            }
                        }

                        Item itemNew = Instantiate(item);
                        itemNew.stackCount = 1;

                        player.playerStatus_.items.Add(itemNew);
                        SoundManager.Instance.PlaySoundEffect(btn_sfx);
                        CreateItemSlot();
                        CreateItemSlotinFurnace();
                        CombineAtom();

                    });

                    ItemSlotCrafting_recie_view.Add(slot);
                }
            }
        }
    }

    private ElementalStructure GetElementalStructureByAtomNumber(int atomNumber)
    {
        return FindAnyObjectByType<ElementalReader>().elementList.FirstOrDefault(e => e.atom_number == atomNumber);
    }

    public bool CheckFurnaceForCraftingResources()
    {
        foreach (Item item in itemReader.itemList)
        {
            bool allResourcesMatch = true;

            foreach (UseResource resource in item.crafting_resources)
            {
                var matchingElement = inFurnace.Find(e => e.atom_number == resource.AtomNumber && e.stack_Count >= resource.StackCount);
                if (matchingElement == null)
                {
                    allResourcesMatch = false;
                    break;
                }
            }

            if (allResourcesMatch)
            {
                return true;
            }
        }

        return false;
    }


    public void CreateItemSlot()
    {
        if (Itemslot_view != null)
        {
            foreach (GameObject gameObject_slot in Itemslot_view)
            {
                Destroy(gameObject_slot);
            }
            Itemslot_view.Clear();
        }

        foreach (ElementalStructure elem in player.playerStatus_.elementals)
        {
            GameObject slot = Instantiate(ItemSlotPrefab, ContentViewerSlot.transform);
            slot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = $"<color=#00FF7F>{elem.symbol_}</color>  " + elem.name_;
            slot.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = elem.stack_Count.ToString();
            slot.transform.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            slot.transform.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);

            slot.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
            {
                Description_.text = $"<color=#00FF7F>{elem.symbol_}</color>  " + elem.name_ + "\n" + elem.description_ + $"\nAtom Number : {elem.atom_number}";
            });

            slot.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
            {
                if(elem.stack_Count > 1)
                {
                    elem.stack_Count -= 1;
                }
                else if (elem.stack_Count <= 1)
                {
                    player.playerStatus_.elementals.Remove(elem);
                }

                var existingElemental = inFurnace.Find(e => e.name_ == elem.name_);
                if (existingElemental != null)
                {
                    existingElemental.stack_Count += 1;
                }
                else
                {
                    var newElement = Instantiate(elem);
                    newElement.name = elem.name;
                    newElement.stack_Count = 1;
                    inFurnace.Add(newElement);
                }

                SoundManager.Instance.PlaySoundEffect(btn_sfx);

                CreateItemSlot();
                CreateItemSlotinFurnace();
                CombineAtom();
            });

            Itemslot_view.Add(slot);
        }
    }

    public void CreateItemSlotinFurnace()
    {

        if (FurnaceSlot_view != null)
        {
            foreach (GameObject gameObject_slot in FurnaceSlot_view)
            {
                Destroy(gameObject_slot);
            }
            FurnaceSlot_view.Clear();
        }

        foreach (ElementalStructure elem in inFurnace)
        {
            GameObject slot = Instantiate(ItemSlotPrefab, FurnaceViewerSlot.transform);
            slot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = $"<color=#00FF7F>{elem.symbol_}</color>  " + elem.name_;
            slot.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = elem.stack_Count.ToString();
            slot.transform.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            slot.transform.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            slot.transform.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);

            slot.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
            {
                Description_.text = $"<color=#00FF7F>{elem.symbol_}</color>  " + elem.name_ + "\n" + elem.description_ + $"\nAtom Number : {elem.atom_number}";
            });


            slot.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
            {
                if (elem.stack_Count > 1)
                {
                    elem.stack_Count -= 1;
                }
                else if (elem.stack_Count <= 1)
                {
                    inFurnace.Remove(elem);
                }

                var existingElemental = player.playerStatus_.elementals.Find(e => e.name_ == elem.name_);
                if (existingElemental != null)
                {
                    existingElemental.stack_Count += 1;
                }
                else
                {
                    var newElement = Instantiate(elem);
                    newElement.name = elem.name;
                    newElement.stack_Count = 1;
                    player.playerStatus_.elementals.Add(newElement);
                }

                SoundManager.Instance.PlaySoundEffect(btn_sfx);

                CreateItemSlot();
                CreateItemSlotinFurnace();
                CombineAtom();

            });

            FurnaceSlot_view.Add(slot);
        }
    }

    public void ReturnAllElementalsFromFurnace()
    {
        // Loop through each element in the inFurnace list
        foreach (ElementalStructure elemental in inFurnace)
        {
            // Find if the same elemental already exists in the player's inventory
            var existingElemental = player.playerStatus_.elementals.Find(e => e.atom_number == elemental.atom_number);

            // If it exists, increase its stack count
            if (existingElemental != null)
            {
                existingElemental.stack_Count += elemental.stack_Count;
            }
            // If it doesn't exist, add a new instance of it to the player's inventory
            else
            {
                // You should instantiate a new object to avoid issues with referencing the original object
                var newElemental = Instantiate(elemental);
                newElemental.stack_Count = elemental.stack_Count;
                player.playerStatus_.elementals.Add(newElemental);
            }
        }

        // Clear the inFurnace list after returning all elements
        inFurnace.Clear();

        // Re-render the UI for both inventories to show the changes
        CreateItemSlot();
        CreateItemSlotinFurnace();

        // Since the furnace is empty, you might want to update the crafting recipes as well
        CombineAtom();
    }
}
