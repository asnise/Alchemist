using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class DepthOfFieldControl : MonoBehaviour
{
    public static DepthOfFieldControl Instance; // Singleton instance

    public Volume postProcessVolume; // Reference to the Post-process Volume
    private DepthOfField depthOfField; // Reference to the Depth of Field effect

    private float targetFocusDistance; // Target focus distance
    private float currentFocusDistance; // Current focus distance
    public float transitionSpeed = 1.0f; // Speed of transition between focus distances

    void Awake()
    {
        // Implement Singleton pattern to ensure only one instance of DepthOfFieldControl
        if (Instance == null)
        {
            Instance = this; // Set the instance to this object
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates if any
        }
    }

    void Start()
    {
        // Get the Depth of Field effect from the Post-process Volume
        if (postProcessVolume.profile.TryGet<DepthOfField>(out depthOfField))
        {
            // Set initial focus distance (far focus by default)
            currentFocusDistance = 3f;
            targetFocusDistance = 3f;
        }
        else
        {
            Debug.LogError("DepthOfField effect not found in the Volume.");
        }
    }

    void Update()
    {
        // Lerp the current focus distance towards the target
        currentFocusDistance = Mathf.Lerp(currentFocusDistance, targetFocusDistance, Time.deltaTime * transitionSpeed);

        // Set the focus distance in the Depth of Field effect
        if (depthOfField != null)
        {
            depthOfField.focusDistance.value = currentFocusDistance;
        }
    }

    // Method to transition to near focus (0.1)
    public void FocusIn()
    {
        targetFocusDistance = 0.1f; // Set target to near distance
    }

    public void FocusInMedium()
    {
        targetFocusDistance = 1.5f; // Set target to near distance
    }

    // Method to transition to far focus (3)
    public void FocusOut()
    {
        targetFocusDistance = 3f; // Set target to far distance
    }
}
