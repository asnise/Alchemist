using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameLoader : MonoBehaviour
{
    public GameObject elementalPrefab;
    public GameObject itemPrefab;
    public Transform elementalParent;
    public Transform itemParent;

    private string elementalsPath = Application.persistentDataPath + "/elementals.json";
    private string itemsPath = Application.persistentDataPath + "/items.json";

    void Start()
    {
        LoadElementals();
        LoadItems();
    }

    void LoadElementals()
    {
        if (File.Exists(elementalsPath))
        {
            string json = File.ReadAllText(elementalsPath);
            List<ElementalStructure> elementals = JsonUtility.FromJson<List<ElementalStructure>>(json);

            foreach (var elemental in elementals)
            {
                GameObject elementalInstance = Instantiate(elementalPrefab, elementalParent);
                // Update UI with elemental data
                elementalInstance.GetComponentInChildren<TMP_Text>().text = elemental.name_;
            }
        }
        else
        {
            Debug.LogWarning("Elementals file not found.");
        }
    }

    void LoadItems()
    {
        if (File.Exists(itemsPath))
        {
            string json = File.ReadAllText(itemsPath);
            List<Item> items = JsonUtility.FromJson<List<Item>>(json);

            foreach (var item in items)
            {
                GameObject itemInstance = Instantiate(itemPrefab, itemParent);
                // Update UI with item data
                itemInstance.GetComponentInChildren<TMP_Text>().text = item.description;
            }
        }
        else
        {
            Debug.LogWarning("Items file not found.");
        }
    }
}
