using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public class UIElement
    {
        public GameObject uiObject;
        public KeyCode toggleKey;
        public UnityEvent onToggle;
        public Button CloseButton;
        public AudioClip openSound;
    }

    public GameObject Pause_uiElements;
    public List<UIElement> uiElements = new List<UIElement>();
    private GameObject activeUI;
    private bool isPaused = false;  // Track pause state

    void Update()
    {
        // Toggle UI based on key press
        foreach (var element in uiElements)
        {
            if (Input.GetKeyDown(element.toggleKey))
            {
                ToggleUI(element);
                break;
            }
        }

        // Check if Escape is pressed and no UI is active
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (activeUI == null)  // No UI is active, toggle pause
            {
                TogglePause();
            }
            else  // If UI is active, close it
            {
                CloseAllUI();
            }
        }
    }

    void ToggleUI(UIElement element)
    {
        if (activeUI == element.uiObject)
        {
            CloseAllUI();
        }
        else
        {
            if (activeUI != null)
            {
                activeUI.SetActive(false);
            }
            element.uiObject.SetActive(true);
            activeUI = element.uiObject;
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;

            if (element.CloseButton != null)
            {
                element.CloseButton.onClick.AddListener(() =>
                {
                    CloseAllUI();
                });
            }
            // Invoke assigned UnityEvent when UI is opened
            element.onToggle?.Invoke();
            FindAnyObjectByType<DepthOfFieldControl>().FocusInMedium();
        }

        if (element.openSound != null)
        {
            SoundManager.Instance.PlaySoundEffect(element.openSound);
        }
    }

    public void CloseAllUI()
    {
        if (activeUI != null)
        {
            activeUI.SetActive(false);
            activeUI = null;
        }
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        FindAnyObjectByType<DepthOfFieldControl>().FocusOut();

    }

    public void CloseAllUIWithOutLocked()
    {
        if (activeUI != null)
        {
            activeUI.SetActive(false);
            activeUI = null;
        }
        FindAnyObjectByType<DepthOfFieldControl>().FocusOut();

    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        // Pause/Unpause the game by setting the time scale
        Time.timeScale = isPaused ? 0f : 1f;

        // Optionally, show/hide pause UI (e.g., pause menu)
        if (Pause_uiElements != null)
        {
            Pause_uiElements.SetActive(isPaused);
        }

        // Handle cursor state based on pause
        //Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        //Cursor.visible = isPaused;

        if(isPaused)
        {
            DepthOfFieldControl.Instance.FocusIn();
        }
        else
        {
            DepthOfFieldControl.Instance.FocusOut();
        }
    }

    public void OpenUI(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    public void CloseUI(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
