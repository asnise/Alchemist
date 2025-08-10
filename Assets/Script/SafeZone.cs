using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public Transform player; // Reference to the player (can be assigned in the inspector)
    public float safeZoneRadius = 100f; // Radius of the safe zone (100 meters)
    public GameObject[] objectsToEnable; // Objects to enable when the player enters the safe zone
    public GameObject[] objectsToDisable; // Objects to disable when the player enters the safe zone

    private void LateUpdate()
    {
        // Calculate the distance between the player and the SafeZone center (this object)
        float distance = Vector3.Distance(player.position, transform.position);

        // Check if the player is within the safe zone range
        if (distance <= safeZoneRadius)
        {
            SetObjectsActive(true); // Enable certain objects and disable others
            player.GetComponent<PlayerCombat>().enabled = false; // Enable the PlayerCombat script

        }
        else
        {
            SetObjectsActive(false); // Disable certain objects and enable others
            player.GetComponent<PlayerCombat>().enabled = true; // Enable the PlayerCombat script
        }
    }

    private void SetObjectsActive(bool isActive)
    {
        // Enable objects that should be enabled
        foreach (GameObject obj in objectsToEnable)
        {
            obj.SetActive(isActive);
        }

        // Disable objects that should be disabled
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(!isActive);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere in the Scene view to visualize the safe zone radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, safeZoneRadius);
    }
}
