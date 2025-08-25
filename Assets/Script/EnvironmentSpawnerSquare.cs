using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentObject 
{
    [Tooltip("Prefab ของ Object ที่จะ spawn")]
    public GameObject prefab;

    [Tooltip("Minimum distance required between this object and others.")]
    public float minDistance = 1.5f;
}

public class EnvironmentSpawnerSquare : MonoBehaviour
{
    [Header("Player & Prefabs")]
    public Transform player;
    public EnvironmentObject[] prefabs;

    [Header("Generation Settings")]
    public Vector2 spawnAreaSize = new Vector2(20f, 20f);
    public Vector2 despawnAreaSize = new Vector2(25f, 25f);
    public int density = 50;

    [Header("Random Seed")]
    public int seed = 12345;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Dictionary<GameObject, float> objectMinDistances = new Dictionary<GameObject, float>();
    private System.Random rng;

    void Start()
    {
        rng = new System.Random(seed);
    }

    void Update()
    {
        SpawnEnvironment();
        DespawnFarObjects();
    }

    void SpawnEnvironment()
    {
        int targetCount = density;
        int safetyCounter = 0;

        while (spawnedObjects.Count < targetCount && safetyCounter < 500)
        {
            safetyCounter++;

            // เลือก prefab จาก list
            EnvironmentObject envObj = prefabs[rng.Next(prefabs.Length)];
            GameObject prefab = envObj.prefab;
            float prefabMinDistance = envObj.minDistance;

            // สุ่มตำแหน่งที่ขอบสี่เหลี่ยม
            Vector2 randomPos = RandomEdgePosition(spawnAreaSize) + (Vector2)player.position;

            // ตรวจสอบว่าไม่ overlap
            if (!IsOverlapping(randomPos, prefabMinDistance))
            {
                GameObject obj = Instantiate(prefab, randomPos, Quaternion.identity);
                spawnedObjects.Add(obj);
                objectMinDistances[obj] = prefabMinDistance;
                obj.transform.parent = transform;
            }
        }
    }

    void DespawnFarObjects()
    {
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            Vector2 pos = spawnedObjects[i].transform.position;
            Vector2 pPos = player.position;

            if (Mathf.Abs(pos.x - pPos.x) > despawnAreaSize.x / 2 ||
                Mathf.Abs(pos.y - pPos.y) > despawnAreaSize.y / 2)
            {
                objectMinDistances.Remove(spawnedObjects[i]);
                Destroy(spawnedObjects[i]);
                spawnedObjects.RemoveAt(i);
            }
        }
    }

    // ✅ ตรวจสอบว่า pos ไม่ทับ object อื่น
    bool IsOverlapping(Vector2 pos, float newObjMinDist)
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj == null) continue;

            float otherMinDist = objectMinDistances.ContainsKey(obj) ? objectMinDistances[obj] : 1.5f;

            float dist = Vector2.Distance(pos, obj.transform.position);
            if (dist < Mathf.Max(newObjMinDist, otherMinDist))
                return true;
        }
        return false;
    }

    // ✅ สุ่มตำแหน่งขอบสี่เหลี่ยม
    Vector2 RandomEdgePosition(Vector2 size)
    {
        float halfX = size.x / 2f;
        float halfY = size.y / 2f;
        int side = rng.Next(4);

        float x = 0, y = 0;
        if (side == 0) { x = -halfX; y = (float)(rng.NextDouble() * size.y - halfY); }
        else if (side == 1) { x = halfX; y = (float)(rng.NextDouble() * size.y - halfY); }
        else if (side == 2) { x = (float)(rng.NextDouble() * size.x - halfX); y = halfY; }
        else { x = (float)(rng.NextDouble() * size.x - halfX); y = -halfY; }

        return new Vector2(x, y);
    }

    public void RemoveSpawnedObject(GameObject obj)
    {
        if (spawnedObjects.Contains(obj))
        {
            objectMinDistances.Remove(obj);
            spawnedObjects.Remove(obj);
        }
    }

    // 🔲 Gizmos
    void OnDrawGizmos()
    {
        if (player == null) return;

        // กรอบ spawn
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(player.position, spawnAreaSize);

        // กรอบ despawn
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(player.position, despawnAreaSize);

        // วาด minDistance รอบ object แต่ละตัว
        Gizmos.color = Color.yellow;
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                float radius = objectMinDistances.ContainsKey(obj) ? objectMinDistances[obj] : 1.5f;
                Gizmos.DrawWireSphere(obj.transform.position, radius);
            }
        }
    }
}
