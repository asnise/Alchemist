using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopReader : MonoBehaviour
{
    public Player player;
    public GameObject ContentViewerSlot;
    public GameObject ItemSlotPrefab;

    public float price_muliplier = 1.7f;

    public List<GameObject> Itemslot_view;

    public AudioClip btn_sfx;

    public GameObject GamaManager_;

    public List<ElementalStructure> elementals_instock_list;
    public List<Item> items_instock_list;

    public enum InventoryCatagory
    {
        Elemental,
        Item,
        Buy
    }


    public void CreateItemSlot(InventoryCatagory inventoryCatagory)
    {

        if (Itemslot_view != null)
        {
            foreach (GameObject gameObject_slot in Itemslot_view)
            {
                Destroy(gameObject_slot);
            }
            Itemslot_view.Clear();
        }


        if (inventoryCatagory == InventoryCatagory.Elemental)
        {
            var sortedElementals = player.playerStatus_.elementals.OrderByDescending(e => e.price).ToList();
            foreach (ElementalStructure elem in sortedElementals)
            {
                GameObject slot = Instantiate(ItemSlotPrefab, ContentViewerSlot.transform);
                slot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = $"<color=#00FF7F>{elem.symbol_}</color>  " + elem.name_ + $"\n<color=#00FF7F>{(int)elem.price}G</color>";
                slot.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = elem.stack_Count.ToString();
                slot.transform.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                slot.transform.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);

                slot.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
                {
                    player.playerStatus_.gold_ += (int)elem.price;

                    if (elem.stack_Count > 1)
                    {
                        elem.stack_Count--;
                    }
                    else if (elem.stack_Count <= 1)
                    {
                        player.playerStatus_.elementals.Remove(elem);
                    }

                    CreateSlotUIElement();
                    SoundManager.Instance.PlaySoundEffect(btn_sfx);

                });

                Itemslot_view.Add(slot);
            }
        }
        else if (inventoryCatagory == InventoryCatagory.Item)
        {
            var sortedItem = player.playerStatus_.items.OrderByDescending(e => e.price).ToList();
            foreach (Item item in sortedItem)
            {
                GameObject slot = Instantiate(ItemSlotPrefab, ContentViewerSlot.transform);
                slot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = item.itemName + $"\n<color=#00FF7F>{item.price}G</color>";
                slot.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = item.stackCount.ToString();
                slot.transform.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                slot.transform.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);

                slot.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
                {
                    player.playerStatus_.gold_ += item.price;

                    if (item.stackCount > 1)
                    {
                        item.stackCount--;
                    }
                    else if (item.stackCount <= 1)
                    {
                        player.playerStatus_.items.Remove(item);
                    }
                    CreateSlotUIItem();
                    SoundManager.Instance.PlaySoundEffect(btn_sfx);

                });

                Itemslot_view.Add(slot);
            }
        }
        else if (inventoryCatagory == InventoryCatagory.Buy)
        {
            foreach (Item item in items_instock_list)
            {
                GameObject slot = Instantiate(ItemSlotPrefab, ContentViewerSlot.transform);
                slot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = item.itemName + $"\n<color=#00FF7F>{item.price * price_muliplier}G</color>";
                slot.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "1";
                slot.transform.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                slot.transform.transform.GetChild(1).GetChild(4).gameObject.SetActive(true);

                slot.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
                {
                    player.playerStatus_.gold_ -= Convert.ToInt32(item.price * price_muliplier);


                    Item item1 = Instantiate(item);
                    item1.stackCount = 1;
                    player.playerStatus_.items.Add(item1);
                    Destroy(slot);
                    Itemslot_view.Remove(slot);
                    items_instock_list.Remove(item);
                    SoundManager.Instance.PlaySoundEffect(btn_sfx);

                });

                Itemslot_view.Add(slot);
            }

            foreach (ElementalStructure elem in elementals_instock_list)
            {
                GameObject slot = Instantiate(ItemSlotPrefab, ContentViewerSlot.transform);
                slot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = $"<color=#00FF7F>{elem.symbol_}</color>  " + elem.name_ + $"\n<color=#00FF7F>{(int)elem.price * price_muliplier}G</color>";
                slot.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "1";
                slot.transform.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                slot.transform.transform.GetChild(1).GetChild(4).gameObject.SetActive(true);
                slot.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
                {
                    player.playerStatus_.gold_ -= Convert.ToInt32(elem.price * price_muliplier);
                    ElementalStructure elem1 = Instantiate(elem);
                    elem1.stack_Count = 1;
                    player.playerStatus_.elementals.Add(elem1);
                    Destroy(slot);
                    Itemslot_view.Remove(slot);
                    elementals_instock_list.Remove(elem);
                    SoundManager.Instance.PlaySoundEffect(btn_sfx);
                });
                Itemslot_view.Add(slot);
            }
        }
    }

    public void Random_instock_list()
    {
        elementals_instock_list.Clear();
        items_instock_list.Clear();

        // Ensure the GameManager reference is valid  
        if (GamaManager_ == null)
        {
            Debug.LogError("GamaManager_ is null. Please assign it in the inspector.");
            return;
        }

        // Fetch all elementals and items safely  
        var elementalReader = GamaManager_.GetComponent<ElementalReader>();
        var itemReader = GamaManager_.GetComponent<ItemReader>();

        if (elementalReader == null || itemReader == null)
        {
            Debug.LogError("ElementalReader or ItemReader component is missing on GamaManager_.");
            return;
        }

        var allElementals = elementalReader.elementList;
        var allItems = itemReader.itemList;

        if (allElementals == null || allItems == null)
        {
            Debug.LogError("Elemental list or item list is null.");
            return;
        }

        // Randomly select up to 5 unique ElementalStructures  
        var randomElementals = allElementals.Where(e => e != null && !elementals_instock_list.Contains(e))
                                            .OrderBy(e => UnityEngine.Random.value)
                                            .Take(5)
                                            .ToList();
        elementals_instock_list.AddRange(randomElementals);

        // Randomly select up to 5 unique Items  
        var randomItems = allItems.Where(i => i != null && !items_instock_list.Contains(i))
                                   .OrderBy(i => UnityEngine.Random.value)
                                   .Take(5)
                                   .ToList();
        items_instock_list.AddRange(randomItems);
    }

    public void CreateSlotUIElement()
    {
        CreateItemSlot(InventoryCatagory.Elemental);
    }
    public void CreateSlotUIItem()
    {
        CreateItemSlot(InventoryCatagory.Item);
    }

    public void CreateSlotUIBuy()
    {
        CreateItemSlot(InventoryCatagory.Buy);
    }
}
