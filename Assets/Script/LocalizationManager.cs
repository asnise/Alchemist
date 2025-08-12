using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;


public class LanguageData
{
    public Dictionary<string, string> th;
    public Dictionary<string, string> en;
}

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    public TextAsset languageFile;

    private Dictionary<string, string> currentLanguage;
    private LanguageData languageData;

    public SystemLanguage defaultLanguage = SystemLanguage.English;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLanguageData();
            SetLanguage(GetSystemLanguage());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadLanguageData()
    {
        string json = languageFile.text;
        languageData = JsonConvert.DeserializeObject<LanguageData>(json); // <-- Use Newtonsoft.Json
    }

    private SystemLanguage GetSystemLanguage()
    {
        if (Application.systemLanguage == SystemLanguage.Thai)
            return SystemLanguage.Thai;

        return defaultLanguage;
    }

    public void SetLanguage(SystemLanguage lang)
    {
        switch (lang)
        {
            case SystemLanguage.Thai:
                currentLanguage = languageData.th;
                break;
            case SystemLanguage.English:
                currentLanguage = languageData.en;
                break;
            default:
                currentLanguage = languageData.en;
                break;
        }
    }

    public string GetLocalizedString(string key)
    {
        if (currentLanguage != null && currentLanguage.ContainsKey(key))
        {
            return currentLanguage[key];
        }

        Debug.LogWarning($"Missing localization for key: {key}");
        return key;
    }
}