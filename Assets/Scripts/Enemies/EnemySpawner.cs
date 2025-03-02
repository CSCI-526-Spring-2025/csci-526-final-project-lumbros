using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // 敌人预制体数组
    public Transform spawnPoint;      // 生成点
    public float spawnRadius = 2f;    // 生成区域半径

    public float EnemySpawnChance = 0.3f;
    public float rangedEnemySpawnChance = 0.2f;
    public float EnemyStalkerSpawnChance = 0.2f;
    public float EnemyPhantomSpawnChance = 0.2f;
    public float SpawnInterval = 1f;


    public void SpawnWave(int enemyCount)
    {
        StartCoroutine(SpawnEnemies(enemyCount));
    }

    IEnumerator SpawnEnemies(int enemyCount)
    {
        Debug.Log($"Num {enemyCount}");
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(spawnPoint.position.x + randomOffset.x, spawnPoint.position.y + randomOffset.y, spawnPoint.position.z);

            GameObject enemyToSpawn = ChooseRandomEnemy();
            GameObject newEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);

            //Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
            //if (enemyComponent != null)
            //{
            //     enemyComponent.health = Mathf.RoundToInt(enemyComponent.health * healthMultiplier);
            //     enemyComponent.attackDamage *= Mathf.RoundToInt(enemyComponent.attackDamage * speedMultiplier);
            //}

            yield return new WaitForSeconds(SpawnInterval); // 控制敌人生成节奏
        }
    }

    GameObject ChooseRandomEnemy()
    {
        float randomValue = Random.value;
        if (randomValue < EnemySpawnChance) return enemyPrefabs[0];
        if (randomValue < (EnemySpawnChance + rangedEnemySpawnChance)) return enemyPrefabs[1];
        if (randomValue < (EnemySpawnChance + rangedEnemySpawnChance + EnemyStalkerSpawnChance)) return enemyPrefabs[2];
        if (randomValue < (EnemySpawnChance + rangedEnemySpawnChance + EnemyStalkerSpawnChance + EnemyPhantomSpawnChance)) return enemyPrefabs[3];
        return enemyPrefabs[4];
    }
}
