using System.Collections;
using TMPro;
using UnityEngine;

public class SetTextPlayerMoney : MonoBehaviour
{
    private TextMeshProUGUI goldText;
    private PlayerStatus playerStatus;
    private long previousGold; // Stores the last displayed gold value
    private float lerpSpeed = 5f; // Speed of interpolation
    private float lerpTime = 0f;
    private long targetGold; // The target gold value
    private float currentGold; // Interpolated value for smooth transition

    public enum DisplayMode
    {
        Gold,
        Dept
    }

    public DisplayMode displayMode = DisplayMode.Gold;

    void Start()
    {
        goldText = GetComponent<TextMeshProUGUI>();
        playerStatus = GameObject.Find("Player").GetComponent<Player>().playerStatus_;

        if (playerStatus != null)
        {
            UpdateText();
            previousGold = GetCurrentValue();
            currentGold = previousGold;
            targetGold = previousGold;
        }
    }

    void Update()
    {
        if (playerStatus != null)
        {
            long currentValue = GetCurrentValue();
            if (targetGold != currentValue) // Detect changes
            {
                previousGold = targetGold; // Store the previous gold value
                targetGold = currentValue; // Update the target value
                lerpTime = 0f; // Reset lerp progress
            }

            // Smoothly interpolate gold value over time
            if (currentGold != targetGold)
            {
                lerpTime += Time.deltaTime * lerpSpeed;
                currentGold = Mathf.Lerp(previousGold, targetGold, lerpTime);
                goldText.text = Mathf.Round(currentGold).ToString("N0"); // Format with commas
            }
        }
    }

    public void SetTextMoney()
    {
        if (playerStatus != null)
        {
            UpdateText();
        }
    }

    private long GetCurrentValue()
    {
        return displayMode == DisplayMode.Gold ? playerStatus.gold_ : -playerStatus.dept_;
    }

    private void UpdateText()
    {
        goldText.text = GetCurrentValue().ToString("N0");
    }
}
