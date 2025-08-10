using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GenderType
{
    Male,
    Female
}

[System.Serializable]
public class PlayerStatus
{
    public string playerName = "Player";
    public GenderType playerType;
    public int gold_;
    public int dept_ = 1000000;
    public List<ElementalStructure> elementals = new List<ElementalStructure>();
    public List<Item> items = new List<Item>();
    public int maxHealth = 10;
    public int weapon_lv = 1;
    public int armor_lv = 1;

    public PlayerStatus()
    {
        gold_ = 0;
        dept_ = 1000000;
    }

    public string lastSaveTime;
    public float play_time = 0f;
}

public class Player : MonoBehaviour
{
    public int health = 10;
    public int maxHealth = 10;
    public Image healthBar;
    public TextMeshProUGUI healthText;


    [SerializeField]
    public PlayerStatus playerStatus_ = new PlayerStatus();

    public List<ElementalStructure> elementals_indungeon;
    public List<Item> items_indungeon;

    private float updateSpeed = 0.2f;


    public GameObject InventoryUi;
    public GameObject ContentViewerSlot;
    public GameObject ItemSlotPrefab;

    public List<GameObject> Itemslot_view;

    public GameObject GameOverPanel;

    public AudioClip DamageSfx;

    public SaveLoadSystem saveLoadSystem;

    public GameObject Headbase;
    public GameObject Bodybase;

    public Sprite Head_M;
    public Sprite Body_M;

    public Sprite Head_F;
    public Sprite Body_F;

    public void Start()
    {
        saveLoadSystem.LoadPlayerDataPrefs();

        if (PlayerPrefs.GetInt("LoadSave") == 1)
        {
            saveLoadSystem.Load(PlayerPrefs.GetString("PlayerName"));
        }


        health = maxHealth;
        UpdateHealthBarInstant();
        GenderModelLoad();
    }

    public void GenderModelLoad()
    {
        if (playerStatus_.playerType == 0)
        {
            Headbase.GetComponent<SpriteRenderer>().sprite = Head_M;
            Bodybase.GetComponent<SpriteRenderer>().sprite = Body_M;
        }
        else
        {
            Headbase.GetComponent<SpriteRenderer>().sprite = Head_F;
            Bodybase.GetComponent<SpriteRenderer>().sprite = Body_F;
        }
    }

    public void FixedUpdate()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (float)health / maxHealth, Time.deltaTime * 10f);
        healthText.text = health + " / " + maxHealth;

        playerStatus_.play_time += Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        if (health > 0)
        {
            health -= damage;
            if (health < 0) health = 0;
            FindObjectOfType<CameraFollow>().ShakeCamera(0.1f, 0.5f);
            SoundManager.Instance.PlaySoundEffectOnPlayer(DamageSfx);
            StartCoroutine(UpdateHealthBarSmooth());

            if (health <= 0)
            {
                Die();
            }
        }
    }

    private IEnumerator UpdateHealthBarSmooth()
    {
        float targetFill = (float)health / maxHealth;
        float elapsedTime = 0f;
        float startFill = healthBar.fillAmount;

        while (elapsedTime < updateSpeed)
        {
            elapsedTime += Time.deltaTime;
            healthBar.fillAmount = Mathf.Lerp(startFill, targetFill, elapsedTime / updateSpeed);
            yield return null;
        }

        healthBar.fillAmount = targetFill;
    }

    private void UpdateHealthBarInstant()
    {
        healthBar.fillAmount = (float)health / maxHealth;
    }

    public void Die()
    {
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;
        GameOverPanel.SetActive(true);
        GetComponent<Movement>().animator.SetBool("Die",true);
        GetComponent<PlayerCombat>().canAttack = false;
        GetComponent<Movement>().canMove = false;
    }

    public void Revive()
    {
        GameOverPanel.SetActive(false);
        health = maxHealth;
        UpdateHealthBarInstant();
        GetComponent<Movement>().animator.SetBool("Die", false);
        GetComponent<PlayerCombat>().canAttack = true;
        GetComponent<Movement>().canMove = true;
    }

    public enum InventoryCatagory
    {
        Elemental,
        Item
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
            if (GetComponent<PlayerCombat>().enabled)
            {
                foreach (ElementalStructure elem in elementals_indungeon)
                {
                    GameObject slot = Instantiate(ItemSlotPrefab, ContentViewerSlot.transform);
                    slot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = $"<color=#00FF7F>{elem.symbol_}</color>  " + elem.name_;
                    slot.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = elem.stack_Count.ToString();
                    Itemslot_view.Add(slot);
                }
                InventoryUi.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Back Pack";

            }
            else
            {
                foreach (ElementalStructure elem in playerStatus_.elementals)
                {
                    GameObject slot = Instantiate(ItemSlotPrefab, ContentViewerSlot.transform);
                    slot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = $"<color=#00FF7F>{elem.symbol_}</color>  " + elem.name_;
                    slot.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = elem.stack_Count.ToString();
                    Itemslot_view.Add(slot);
                }
                InventoryUi.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Storage";

            }
        }
        else
        {
            if (GetComponent<PlayerCombat>().enabled)
            {
                foreach (Item item in items_indungeon)
                {
                    GameObject slot = Instantiate(ItemSlotPrefab, ContentViewerSlot.transform);
                    slot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = item.itemName;
                    slot.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = item.stackCount.ToString();
                    Itemslot_view.Add(slot);
                }
                InventoryUi.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Back Pack";

            }
            else
            {
                foreach (Item item in playerStatus_.items)
                {
                    GameObject slot = Instantiate(ItemSlotPrefab, ContentViewerSlot.transform);
                    slot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = item.itemName;
                    slot.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = item.stackCount.ToString();
                    Itemslot_view.Add(slot);
                }
                InventoryUi.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Storage";

            }
        }
    }

    public void CreateSlotUIElement()
    {
        CreateItemSlot(InventoryCatagory.Elemental);
    }
    public void CreateSlotUIItem()
    {
        CreateItemSlot(InventoryCatagory.Item);
    }

    public void TransferDungeonItemsToPlayer()
    {
        foreach (var elem in elementals_indungeon)
        {
            var existingElemental = playerStatus_.elementals.Find(e => e.name_ == elem.name_);
            if (existingElemental != null)
            {
                existingElemental.stack_Count += elem.stack_Count;
            }
            else
            {
                playerStatus_.elementals.Add(elem);
            }
        }
        elementals_indungeon.Clear();

        foreach (var item in items_indungeon)
        {
            var existingItem = playerStatus_.items.Find(i => i.itemName == item.itemName);
            if (existingItem != null)
            {
                existingItem.stackCount += item.stackCount;
            }
            else
            {
                playerStatus_.items.Add(item);
            }
        }
        items_indungeon.Clear();

        Debug.Log("Transferred all dungeon items and elementals to player inventory.");
    }

}
