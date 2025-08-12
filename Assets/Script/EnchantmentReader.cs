using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnchantmentReader : MonoBehaviour
{
    public TextAsset jsonFile; // Assign your JSON in the Inspector
    public GameObject EnchantButtonPrefab; // Button UI prefab
    public GameObject EnchantButtonParent; // UI Parent for buttons

    public List<Enchantment> enchantments = new List<Enchantment>();

    void Start()
    {
        LoadEnchantments();

        if (EnchantButtonPrefab != null && EnchantButtonParent != null)
        {
            foreach (var enchant in enchantments)
            {
                GameObject obj = Instantiate(EnchantButtonPrefab, EnchantButtonParent.transform);
                obj.transform.GetChild(0).GetComponent<Text>().text = enchant.name;
                obj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.Log($"Applied enchantment: {enchant.name}");
                    // Replace with actual application logic
                    ApplyEnchantmentToPlayer(enchant);
                });
            }
        }
    }

    void LoadEnchantments()
    {
        if (jsonFile == null)
        {
            Debug.LogError("No JSON file assigned!");
            return;
        }

        string wrappedJson = "{\"data\":" + jsonFile.text + "}";
        EnchantmentDataWrapper wrapper = JsonUtility.FromJson<EnchantmentDataWrapper>(wrappedJson);
        Debug.Log("Wrapped JSON: " + wrappedJson);


        foreach (EnchantmentData pair in wrapper.data)
        {
            Enchantment enchantment = new Enchantment
            {
                id = pair.id,
                name = pair.name,
                description = pair.description,
                level = pair.level,
                maxLevel = pair.maxLevel,
                material_atom = pair.material_atom ?? new Dictionary<string, int>(),
                material_item = pair.material_item ?? new Dictionary<string, int>(),
                effects = pair.effects ?? new EnchantmentEffect()
            };

            enchantments.Add(enchantment);
        }

        Debug.Log($"Loaded {enchantments.Count} enchantments.");
    }

    void ApplyEnchantmentToPlayer(Enchantment enchantment)
    {
        // You can extend this with actual player logic
        Debug.Log($"Effect: {enchantment.effects.effectType}, Damage: {enchantment.effects.damage}");
    }
}

[System.Serializable]
public class EnchantmentDataWrapper
{
    public List<EnchantmentData> data;
}

[System.Serializable]
public class EnchantmentData
{
    public int id;
    public string name;
    public string description;
    public int level;
    public int maxLevel;
    public Dictionary<string, int> material_atom;
    public Dictionary<string, int> material_item;
    public EnchantmentEffect effects;
}

[System.Serializable]
public class Enchantment
{
    public int id;
    public string name;
    public string description;
    public int level;
    public int maxLevel;
    public Dictionary<string, int> material_atom;
    public Dictionary<string, int> material_item;
    public EnchantmentEffect effects;
}

[System.Serializable]
public class EnchantmentEffect
{
    public string effectType;
    public int damage;
}