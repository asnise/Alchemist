using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public int SpawnLevel = 1;
    public Text EnemyLevelDisplay;
    public GameObject[] enemyPrefabs;

    public Vector2 areaSize;
    public int maxEnemies = 100;
    public float spawnInterval = 2f;
    public float SpawnLevelIntervalPreSecon = 1;
    public Transform player;
    public Transform Portal_in_dungeon;
    public float enemyRemovalDistance = 100f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    public EnemyDifficulty enemyDifficulty;
    private float nextSpawnTime;

    public ElementalReader elementalReader;
    public ItemReader itemReader;

    private void Start()
    {
        enemyDifficulty = GetComponent<EnemyDifficulty>();
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime && activeEnemies.Count < maxEnemies)
        {
            SpawnRandomEnemy();
            nextSpawnTime = Time.time + spawnInterval;
            transform.position = player.position;
        }

        UpdateSpawnLevel();

        CheckAndRemoveDistantEnemies();
    }

    private void UpdateSpawnLevel()
    {
        if (Portal_in_dungeon == null || player == null) return;

        float distanceToPortal = Vector2.Distance(player.position, Portal_in_dungeon.position);
        SpawnLevel = 1 + Mathf.FloorToInt(distanceToPortal / 10);
        //EnemyLevelDisplay.text = "Enemy Level: " + SpawnLevel;
    }

    private void SpawnRandomEnemy()
    {
        float randomX = UnityEngine.Random.Range(-areaSize.x / 2, areaSize.x / 2);
        float randomY = UnityEngine.Random.Range(-areaSize.y / 2, areaSize.y / 2);
        Vector2 randomPosition = new Vector2(randomX, randomY) + (Vector2)transform.position;

        GameObject enemyPrefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
        GameObject enemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
        enemy.GetComponent<Enemy>().health = Convert.ToInt32(5 + (SpawnLevel * 0.5f));
        enemy.GetComponent<Enemy>().damage = Convert.ToInt32(1 + (SpawnLevel * 0.1f));
        enemy.GetComponent<Enemy>().elementals.Add(elementalReader.GetRandomElementByState(enemy.GetComponent<Enemy>().elementState));
        enemy.GetComponent<Enemy>().items.Add(itemReader.GetRandom());
        activeEnemies.Add(enemy);
    }

    private void CheckAndRemoveDistantEnemies()
    {
        if (player == null) return;

        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] != null)
            {
                float distance = Vector2.Distance(player.position, activeEnemies[i].transform.position);
                if (distance > enemyRemovalDistance)
                {
                    Destroy(activeEnemies[i]);
                    activeEnemies.RemoveAt(i);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 center = transform.position;
        Vector3 size = new Vector3(areaSize.x, areaSize.y, 0);
        Gizmos.DrawWireCube(center, size);
    }

    public void DestoryAllEnemy()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            Destroy(enemy);
        }
        activeEnemies.Clear();
    }
}
