using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemReader : MonoBehaviour
{
    public TextAsset jsonFile; // Reference to the JSON file in Unity's Inspector
    public List<Item> itemList = new List<Item>(); // List to store all the loaded items

    public GameObject CheatedPage;
    public GameObject Cheated_btn;

    public void Start()
    {
        LoadItems();

        foreach (Item item in itemList)
        {
            GameObject obj = Instantiate(Cheated_btn, CheatedPage.transform);
            obj.transform.GetChild(0).GetComponent<Text>().text = item.itemName;
            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                Player playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                item.AddToInventory(playerStatus);
                playerStatus.TransferDungeonItemsToPlayer();
            });
        }
    }

    public void LoadItems()
    {
        if (jsonFile == null)
        {
            Debug.LogError("No JSON file assigned!");
            return;
        }

        string jsonText = jsonFile.text;

        ItemDataList dataList = JsonUtility.FromJson<ItemDataList>("{\"items\":" + jsonText + "}");

        foreach (ItemData data in dataList.items)
        {
            Item newItem = ScriptableObject.CreateInstance<Item>();

            newItem.name = data.id.ToString();
            newItem.id = data.id;
            newItem.itemName = data.itemName;
            newItem.price = data.price;
            newItem.description = data.description;
            newItem.category = (ItemCategory)System.Enum.Parse(typeof(ItemCategory), data.category);
            newItem.stack = Mathf.Max(data.stack, 1); // Ensure stack is never 0 or negative
            newItem.stackCount = Mathf.Max(data.stackCount, 1); // Ensure stackCount is positive
            newItem.crafting_resources = data.crafting_resources;

            itemList.Add(newItem);
        }

        Debug.Log($"Loaded {itemList.Count} items.");
    }

    public Item GetRandom()
    {
        return itemList[Random.Range(0, itemList.Count)];
    }
}

[System.Serializable]
public class ItemDataList
{
    public List<ItemData> items;
}

[System.Serializable]
public class ItemData
{
    public int id;
    public string itemName;
    public int price;
    public string description;
    public string category;
    public int stack;
    public int stackCount;
    public List<UseResource> crafting_resources;
}
