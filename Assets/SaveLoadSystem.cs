using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class SaveLoadSystem : MonoBehaviour
{
    public GameObject Male_model;
    public GameObject Female_model;
    public GenderType genderType = GenderType.Male;
    public TMP_InputField PlayerNameText;
    public string Name_;

    public bool CreateSlot;
    public GameObject SaveCanvas;
    public GameObject SaveSlot;

    public List<GameObject> SaveSlots = new List<GameObject>();

    public Player player_;
    public ElementalReader elementalReader;
    public ItemReader ItemReader;
    PlayerStatus PlayerStatus_;

    public SceneController sceneController;

    private string[] maleNames = { "John", "Michael", "David", "James", "Robert" };
    private string[] femaleNames = { "Emily", "Sarah", "Emma", "Olivia", "Sophia" };

    public void Start()
    {
        if (PlayerNameText != null)
        {
            RandomName();
        }
        if (CreateSlot)
        {
            LoadsaveSlot();
        }
    }

    public void LoadsaveSlot()
    {
        foreach (GameObject saveSlot in SaveSlots)
        {
            Destroy(saveSlot);
        }
        SaveSlots.Clear();

        string path = Application.persistentDataPath;
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        FileInfo[] files = directoryInfo.GetFiles("*.sav");

        foreach (FileInfo file in files)
        {
            GameObject saveSlotInstance = Instantiate(SaveSlot, SaveCanvas.transform);
            string json = File.ReadAllText(file.FullName);
            PlayerStatus playerStatus = JsonUtility.FromJson<PlayerStatus>(json);
            saveSlotInstance.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = playerStatus.playerName;
            saveSlotInstance.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = GetDateOnly(playerStatus.lastSaveTime) + $"\n{playerStatus.gold_.ToString("N0")} G";
            saveSlotInstance.transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = FormatPlayTime(playerStatus.play_time);

            Button loadButton = saveSlotInstance.transform.GetChild(0).GetComponent<Button>();
            loadButton.onClick.AddListener(() =>
            {
                PlayerPrefs.SetString("PlayerName", playerStatus.playerName);
                PlayerPrefs.SetInt("LoadSave", 1);
                PlayerPrefs.Save();
                sceneController.LoadSceneByIndex(2);

            });

            Button deleteButton = saveSlotInstance.transform.GetChild(1).GetComponent<Button>();
            deleteButton.onClick.AddListener(() => DeleteSaveFile(file.FullName, saveSlotInstance));


            SaveSlots.Add(saveSlotInstance);
        }
    }

    private string FormatPlayTime(float playTime)
    {
        int totalMinutes = Mathf.FloorToInt(playTime / 60);
        int seconds = Mathf.FloorToInt(playTime % 60);
        return $"{totalMinutes}:{seconds}";
    }
    public string GetDateOnly(string dateTimeString)
    {
        if (DateTime.TryParse(dateTimeString, out DateTime dateTime))
        {
            return dateTime.ToString("dd/MM/yyyy");
        }
        return string.Empty;
    }
    private void DeleteSaveFile(string filePath, GameObject saveSlotInstance)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Destroy(saveSlotInstance);
        }

        LoadsaveSlot();
    }
    

    public void RandomName()
    {
        string randomName = "";
        System.Random random = new System.Random();
        bool isMale = random.Next(0, 2) == 0;

        if (isMale)
        {
            randomName = maleNames[random.Next(0, maleNames.Length)];
        }
        else
        {
            randomName = femaleNames[random.Next(0, femaleNames.Length)];
        }

        PlayerNameText.text = randomName;
    }

    public void NewGame()
    {
        Name_ = PlayerNameText.text;
        if (IsPlayerNameTaken(Name_) || PlayerNameText.text == "Player name is alread.")
        {
            PlayerNameText.text = "Player name is alread.";
            return;
        }

        PlayerStatus_ = new PlayerStatus();
        NewGameSaveCreate();
        sceneController.LoadSceneByIndex(1);
    }

    private bool IsPlayerNameTaken(string playerName)
    {
        string path = Application.persistentDataPath;
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        FileInfo[] files = directoryInfo.GetFiles("*.sav");

        foreach (FileInfo file in files)
        {
            string json = File.ReadAllText(file.FullName);
            PlayerStatus playerStatus = JsonUtility.FromJson<PlayerStatus>(json);
            if (playerStatus.playerName == playerName)
            {
                return true;
            }
        }

        return false;
    }


    public void NewGameSaveCreate()
    {
        PlayerPrefs.SetString("PlayerName", Name_);
        PlayerPrefs.SetInt("PlayerType", (int)genderType);
        PlayerPrefs.SetInt("LoadSave", 0);

        PlayerPrefs.Save();
    }

    public void LoadPlayerDataPrefs()
    {
        Debug.Log(PlayerPrefs.GetString("PlayerName"));
        Debug.Log((GenderType)PlayerPrefs.GetInt("PlayerType"));
        Debug.Log("Loading (0 False / 1 True) : " + PlayerPrefs.GetInt("LoadSave"));
        player_.playerStatus_.playerName = PlayerPrefs.GetString("PlayerName");
        player_.playerStatus_.playerType = (GenderType)PlayerPrefs.GetInt("PlayerType");
        //PlayerNameText.text = playerStatus_.playerName;
    }

    public void SetMale()
    {
        genderType = GenderType.Male;
        Male_model.SetActive(true);
        Female_model.SetActive(false);
    }

    public void SetFemale()
    {
        genderType = GenderType.Female;
        Female_model.SetActive(true);
        Male_model.SetActive(false);
    }

    public void Save()
    {
        // Proceed with saving the game data
        player_.playerStatus_.lastSaveTime = DateTime.Now.ToString();
        string path = Application.persistentDataPath + $"/{player_.playerStatus_.playerName}.sav";

        PlayerStatusDataFormater dataFormater = new PlayerStatusDataFormater
        {
            playerName = player_.playerStatus_.playerName,
            playerType = player_.playerStatus_.playerType,
            gold_ = player_.playerStatus_.gold_,
            dept_ = player_.playerStatus_.dept_,
            maxHealth = player_.playerStatus_.maxHealth,
            weapon_lv = player_.playerStatus_.weapon_lv,
            armor_lv = player_.playerStatus_.armor_lv,
            lastSaveTime = player_.playerStatus_.lastSaveTime,
            play_time = player_.playerStatus_.play_time
        };

        foreach (var elemental in player_.playerStatus_.elementals)
        {
            PlayerStatusDataFormater.itemDataformater elementalData = new PlayerStatusDataFormater.itemDataformater
            {
                id = elemental.id,
                stack_ = elemental.stack_Count
            };
            dataFormater.elementals.Add(elementalData);
        }

        foreach (var item in player_.playerStatus_.items)
        {
            PlayerStatusDataFormater.itemDataformater itemData = new PlayerStatusDataFormater.itemDataformater
            {
                id = item.id,
                stack_ = item.stackCount
            };
            dataFormater.items.Add(itemData);
        }

        string json = JsonConvert.SerializeObject(dataFormater);
        File.WriteAllText(path, json);
        Debug.Log("Game Saved to " + path);
        Debug.Log(player_.playerStatus_.lastSaveTime);
    }


    public void Load(string playername)
    {
        string path = Application.persistentDataPath + $"/{playername}.sav";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerStatusDataFormater playerStatusDataFormater = JsonConvert.DeserializeObject<PlayerStatusDataFormater>(json);

            player_.playerStatus_.playerName = playerStatusDataFormater.playerName;
            player_.playerStatus_.playerType = playerStatusDataFormater.playerType;
            player_.playerStatus_.gold_ = playerStatusDataFormater.gold_;
            player_.playerStatus_.dept_ = playerStatusDataFormater.dept_;
            player_.playerStatus_.maxHealth = playerStatusDataFormater.maxHealth;
            player_.playerStatus_.weapon_lv = playerStatusDataFormater.weapon_lv;
            player_.playerStatus_.armor_lv = playerStatusDataFormater.armor_lv;
            player_.playerStatus_.lastSaveTime = playerStatusDataFormater.lastSaveTime;
            player_.playerStatus_.play_time = playerStatusDataFormater.play_time;

            player_.playerStatus_.elementals.Clear();
            foreach (PlayerStatusDataFormater.itemDataformater id in playerStatusDataFormater.elementals)
            {
                ElementalStructure elemental = elementalReader.elementList.Find(e => e.id == id.id);
                if (elemental != null)
                {
                    elemental.stack_Count = id.stack_;
                    player_.playerStatus_.elementals.Add(elemental);
                }
            }

            player_.playerStatus_.items.Clear();
            foreach (PlayerStatusDataFormater.itemDataformater ide in playerStatusDataFormater.items)
            {
                Item item = ItemReader.itemList.Find(i => i.id == ide.id);
                if (item != null)
                {
                    item.stackCount = ide.stack_;
                    player_.playerStatus_.items.Add(item);
                }
            }
        }

        player_.GenderModelLoad();
    }

}


public class PlayerStatusDataFormater
{
    public string playerName = "Player";
    public GenderType playerType;
    public int gold_;
    public int dept_ = 1000000;
    public List<itemDataformater> elementals = new List<itemDataformater>();
    public List<itemDataformater> items = new List<itemDataformater>();
    public int maxHealth = 10;
    public int weapon_lv = 1;
    public int armor_lv = 1;

    public string lastSaveTime;
    public float play_time = 0f;

    public class itemDataformater
    {
        public int id;
        public int stack_;
    }
}
