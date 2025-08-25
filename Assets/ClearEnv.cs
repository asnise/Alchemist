using UnityEngine;

public class ClearEnv : MonoBehaviour
{
    [Header("Clear Settings")]
    [Tooltip("Radius for clearing objects")]
    public float radius = 10f;

    [Tooltip("Tag to identify objects to clear")]
    public string targetTag = "Obstacle";

    [Tooltip("Should we clear objects continuously?")]
    public bool continuousClearing = false;

    [Header("Spawner Reference")]
    [Tooltip("Optional reference to EnvironmentSpawner")]
    public EnvironmentSpawnerSquare spawner;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color activeColor = Color.red;
    [SerializeField] private Color inactiveColor = Color.gray;

    void Update()
    {
        if (continuousClearing)
        {
            ClearObjects();
        }
    }

    public void ClearObjects()
    {
        if (string.IsNullOrEmpty(targetTag)) return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag(targetTag))
            {
                // ถ้ามีการอ้างอิง spawner ให้ลบออกจากลิสต์ด้วย
                if (spawner != null)
                {
                    spawner.RemoveSpawnedObject(collider.gameObject);
                }

                Destroy(collider.gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = continuousClearing ? activeColor : inactiveColor;
        Gizmos.DrawWireSphere(transform.position, radius);

        if (!string.IsNullOrEmpty(targetTag))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius * 0.5f);
        }
    }
}