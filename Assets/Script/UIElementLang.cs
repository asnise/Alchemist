using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementLang : MonoBehaviour
{
    public string key; // Localization key

    private TMP_Text textComponentTMP;
    private Text textComponentLegacy;

    void Start()
    {
        // Try getting TMP_Text first
        textComponentTMP = GetComponent<TMP_Text>();

        // If not found, fallback to legacy Text
        if (textComponentTMP == null)
        {
            textComponentLegacy = GetComponent<Text>();
        }

        UpdateText();
    }

    void UpdateText()
    {
        if (string.IsNullOrEmpty(key)) return;

        string localizedText = LocalizationManager.Instance.GetLocalizedString(key);

        if (textComponentTMP != null)
        {
            textComponentTMP.text = localizedText;
        }
        else if (textComponentLegacy != null)
        {
            textComponentLegacy.text = localizedText;
        }
    }
}
