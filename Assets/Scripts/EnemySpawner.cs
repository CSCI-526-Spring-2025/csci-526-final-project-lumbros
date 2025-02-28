using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Array of different enemy prefabs
    public Transform spawnPoint;      // The central spawn position
    public float spawnInterval = 10f;  // Time interval between enemy spawns

    [Range(0, 1)]
    public float EnemySpawnChance = 0.3f; // 30% chance to spawn a ranged enemy
    public float rangedEnemySpawnChance = 0.2f;
    public float EnemyStalkerSpawnChance = 0.2f;
    public float EnemyPhantomSpawnChance = 0.2f;
    //public float EnemyBossSpawnChance = 0.5f;

    public float spawnRadius = 2f; // Defines the random spawn radius around spawnPoint

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // **Generate a random spawn position within the defined radius**
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius; // Generates a random offset inside a circle
            Vector3 spawnPosition = new Vector3(spawnPoint.position.x + randomOffset.x, spawnPoint.position.y + randomOffset.y, spawnPoint.position.z);

            // **Randomly select an enemy type to spawn**
            GameObject enemyToSpawn;
            if (Random.value < EnemySpawnChance)
            {
                enemyToSpawn = enemyPrefabs[0]; // Spawn a
            }
            else if (Random.value < (EnemySpawnChance + rangedEnemySpawnChance))
            {
                enemyToSpawn = enemyPrefabs[1]; // Spawn a
            }
            else if (Random.value < (EnemySpawnChance + rangedEnemySpawnChance + EnemyStalkerSpawnChance))
            {
                enemyToSpawn = enemyPrefabs[2]; // Spawn a
            }
            else if (Random.value < (EnemySpawnChance + rangedEnemySpawnChance + EnemyStalkerSpawnChance + EnemyPhantomSpawnChance))
            {
                enemyToSpawn = enemyPrefabs[3]; // Spawn a
            }
            else
            {
                enemyToSpawn = enemyPrefabs[4]; // Spawn a
            }
            //enemyToSpawn = enemyPrefabs[4];

            // **Instantiate the enemy at the chosen spawn position**
            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);

            // **Wait for the next spawn interval**
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
