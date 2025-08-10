using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System.Linq;


[System.Serializable]
public class ElementalData
{
    public int id;
    public int stack_;
    public int price;
    public string sprite;
    public string name_;
    public string description_;
    public string symbol_;
    public string elementState_;
    public int atom_number;
    public float atom_mass;
    public float weight;
    public int photron_;
    public int electron_;
    public Vector2 pos_in_table;
}

[System.Serializable]
public class ElementalDataList
{
    public ElementalData[] elements;
}


public class ElementalReader : MonoBehaviour
{
    public GameObject player_;
    public TextAsset jsonFile;
    public List<ElementalStructure> elementList = new List<ElementalStructure>();
    public GameObject Element_table;
    public GameObject infomation_window;
    public TextMeshProUGUI element_des;
    public TextMeshProUGUI element_name;
    public TextMeshProUGUI element_symbol;
    public TextMeshProUGUI element_atom_number;

    public GameObject elementbase_prefab;

    public int[] isIgonreIndex;

    [SerializeField]
    public Dictionary<Vector2, GameObject> elementDict = new Dictionary<Vector2, GameObject>();

    public Dictionary<int, GameObject> elementDict_periodic = new Dictionary<int, GameObject>();

    public bool isUnlock = false;

    public GameObject CheatedPage;
    public GameObject Cheated_btn;

    void Start()
    {
        LoadElements();

        SetDataElement();

        foreach (ElementalStructure elemental in elementList)
        {
            GameObject obj = Instantiate(Cheated_btn, CheatedPage.transform);
            obj.transform.GetChild(0).GetComponent<Text>().text = elemental.name_ + $"({elemental.atom_number})";
            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                Player playerStatus = player_.GetComponent<Player>();
                elemental.AddToInventory(playerStatus);
                playerStatus.TransferDungeonItemsToPlayer();
            });
        }
    }

    void LoadElements()
    {
        if (jsonFile == null)
        {
            Debug.LogError("No JSON file assigned!");
            return;
        }

        string jsonText = jsonFile.text;
        ElementalDataList dataList = JsonUtility.FromJson<ElementalDataList>("{\"elements\":" + jsonText + "}");

        foreach (ElementalData data in dataList.elements)
        {
            ElementalStructure newElement = ScriptableObject.CreateInstance<ElementalStructure>();
            newElement.id = data.id;
            newElement.price = data.price;
            newElement.stack_ = Mathf.Max(data.stack_, 1);
            newElement.name_ = data.name_;
            newElement.description_ = data.description_;
            newElement.symbol_ = data.symbol_;
            newElement.atom_number = data.atom_number;
            newElement.atom_mass = data.atom_mass;
            newElement.weight = data.weight;
            newElement.photron_ = data.photron_;
            newElement.electron_ = data.electron_;
            newElement.pos_in_table = new Vector2(data.pos_in_table.x, data.pos_in_table.y);

            if (System.Enum.TryParse(data.elementState_, out ElementState state))
            {
                newElement.elementState_ = state;
            }
            else
            {
                Debug.LogWarning($"Invalid element state: {data.elementState_}");
            }

            elementList.Add(newElement);



        }

        //Debug.Log($"Loaded {elementList.Count} elements.");
    }


    public void InstanElementPlate(Vector2 pos_in_table)
    {
        GameObject obj = Instantiate(elementbase_prefab, Element_table.transform);
        obj.name = "None";

        elementDict.Add(pos_in_table, obj);
    }

    public void CreatePlateSlotElemt()
    {
        int group = 1;
        int period = 1;

        if(elementDict != null)
        {
            foreach (var item in elementDict)
            {
                Destroy(item.Value);
            }

            elementDict.Clear();
        }

        for (int i = 0; i < 126; i++)
        {
            if (group > 18)
            {
                group = 1;
                period++;
            }

            Vector2 pos_in_table = new Vector2(group, period);
            if (!elementDict.ContainsKey(pos_in_table))
            {
                InstanElementPlate(pos_in_table);
            }
            else
            {

            }

            group++;
        }
    }

    public void SetDataElement()
    {
        CreatePlateSlotElemt();

        foreach (ElementalStructure element in elementList)
        {
            Player playerStatus = player_.GetComponent<Player>();
            if ((playerStatus != null && playerStatus.playerStatus_.elementals != null && playerStatus.playerStatus_.elementals.Exists(e => e.name_ == element.name_)) || isUnlock == true)
            {
                if (element.pos_in_table.x == 0 || element.pos_in_table.y == 0)
                {
                    continue;
                }
                Vector2 pos_in_table = new Vector2(element.pos_in_table.x, element.pos_in_table.y);
                if (elementDict.ContainsKey(pos_in_table))
                {
                    elementDict[pos_in_table].name = element.name_;
                    var elementDetail = elementDict[pos_in_table].GetComponent<ElementalDatadetail>();
                    if (elementDetail != null)
                    {
                        elementDetail.elementalStructure = element;
                    }
                    var button = elementDict[pos_in_table].transform.GetChild(0).GetComponent<Button>();
                    if (button != null)
                    {
                        button.enabled = true;
                        ColorBlock colorBlock = button.colors;
                        var image = button.GetComponent<Image>();
                        if (image != null)
                        {
                            image.color = new Color(255, 255, 255, 1);
                        }
                        colorBlock.normalColor = new Color(255, 255, 255, 1);
                        button.colors = colorBlock;
                        var textMeshProUGUI = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                        if (textMeshProUGUI != null)
                        {
                            textMeshProUGUI.text = element.symbol_;
                        }
                        var atomNumberText = button.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                        if (atomNumberText != null)
                        {
                            atomNumberText.text = element.atom_number.ToString();
                        }
                        var nameText = button.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                        if (nameText != null)
                        {
                            nameText.text = element.name_;
                        }

                        button.onClick.AddListener(() =>
                        {
                            infomation_window.SetActive(true);
                            element_des.text = $"<color=#00FF7F>{element.name_}</color>\n" + element.description_ + $"\nSymbol:{element.symbol_}\nAtom Num:{element.atom_number}\nAtom Mass:{element.atom_mass}";
                            element_name.text = element.name_;
                            element_symbol.text = element.symbol_;
                            element_atom_number.text = element.atom_number.ToString();
                        });
                    }
                }
                else
                {
                    Debug.LogWarning($"Element not found in dictionary: {element.name_}");
                }
            }
        }
    }

    public ElementalStructure GetElementById(int id)
    {
        return elementList.FirstOrDefault(e => e.id == id);
    }

    public ElementalStructure GetRandomElementByState(ElementState state)
    {
        List<ElementalStructure> elementsWithState = elementList.FindAll(e => e.elementState_ == state);
        if (elementsWithState.Count > 0)
        {
            int randomIndex = Random.Range(0, elementsWithState.Count);
            return elementsWithState[randomIndex];
        }
        else
        {
            Debug.LogWarning($"No elements found with state {state}");
            return null;
        }
    }

    public void ToggleUnlock()
    {
        isUnlock = !isUnlock;
    }
}