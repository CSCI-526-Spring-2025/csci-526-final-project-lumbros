using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }
    public GameObject[] enemyPrefabs; // 敌人预制体数组
    public Transform spawnPoint;      // 生成点
    public float spawnRadius = 2f;    // 生成区域半径

    public float Enemy0SpawnChance = 0.3f;
    public float Enemy1SpawnChance = 0.2f;
    public float Enemy2SpawnChance = 0.2f;
    public float Enemy3SpawnChance = 0.2f;
    public float Enemy4SpawnChance = 0.1f;

    private bool stopSpawning = false; 

    private void Awake()
    {
        // Check if instance already exists
        if (Instance == null)
        {
            // If not, set instance to this
            Instance = this;
        }
        else if (Instance != this)
        {
            // If instance already exists and it's not this, then destroy this to enforce the singleton.
            Destroy(gameObject);
        }
        
        // Set this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CustomSceneManager.gameStateChange += OnGameStateChanged;
    }

    private void OnGameStateChanged(GAMESTATE newState)
    {
        if (newState == GAMESTATE.GameOver)
        {
            // kill coroutine
            stopSpawning = true;
        }
    }

    public void SpawnWave(int enemyCount, int currentWave, float SpawnInterval)
    {
        stopSpawning = false; // make sure spawning is enabled
        StartCoroutine(SpawnEnemies(enemyCount, currentWave, SpawnInterval));
    }

    IEnumerator SpawnEnemies(int enemyCount, int currentWave, float SpawnInterval)
    {

        Debug.Log($"Spawning {enemyCount} enemies.");
        for (int i = 0; i < enemyCount; i++)
        {
            if (stopSpawning)
            {
                // Necessary for when the game is over but coroutine still exists,
                // so enemies continue spawning when time unpauses on restart.
                Debug.Log("Spawning stopped due to game restart.");
                yield break;
            }

            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(spawnPoint.position.x + randomOffset.x, spawnPoint.position.y + randomOffset.y, spawnPoint.position.z);

            GameObject enemyToSpawn = ChooseRandomEnemy(currentWave);
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

    GameObject ChooseRandomEnemy(int currentWave)
    {
        int ctrlcnt = currentWave % 5;
        float randomValue = Random.value;
        if (currentWave <= 5)
        {
            switch (currentWave)
            {
                case 1:
                    return enemyPrefabs[0];

                case 2:
                    if (randomValue < Enemy0SpawnChance) return enemyPrefabs[0];
                    return enemyPrefabs[1];

                case 3:
                if (randomValue < Enemy0SpawnChance) return enemyPrefabs[0];
                if (randomValue < (Enemy0SpawnChance + Enemy1SpawnChance)) return enemyPrefabs[1];
                return enemyPrefabs[2];

                case 4:
                if (randomValue < Enemy0SpawnChance) return enemyPrefabs[0];
                if (randomValue < (Enemy0SpawnChance + Enemy1SpawnChance)) return enemyPrefabs[1];
                if (randomValue < (Enemy0SpawnChance + Enemy1SpawnChance + Enemy2SpawnChance)) return enemyPrefabs[2];
                return enemyPrefabs[3];

                default:
                    if (randomValue < Enemy0SpawnChance) return enemyPrefabs[0];
                    if (randomValue < (Enemy0SpawnChance + Enemy1SpawnChance)) return enemyPrefabs[1];
                    if (randomValue < (Enemy0SpawnChance + Enemy1SpawnChance + Enemy2SpawnChance)) return enemyPrefabs[2];
                    if (randomValue < (Enemy0SpawnChance + Enemy1SpawnChance + Enemy2SpawnChance + Enemy3SpawnChance)) return enemyPrefabs[3];
                    return enemyPrefabs[4];
            }
        }
        else if (ctrlcnt == 0)
        {
            if (randomValue < Enemy0SpawnChance) return enemyPrefabs[0];
            if (randomValue < (Enemy0SpawnChance + Enemy1SpawnChance)) return enemyPrefabs[1];
            if (randomValue < (Enemy0SpawnChance + Enemy1SpawnChance + Enemy2SpawnChance)) return enemyPrefabs[2];
            if (randomValue < (Enemy0SpawnChance + Enemy1SpawnChance + Enemy2SpawnChance + Enemy3SpawnChance)) return enemyPrefabs[3];
            return enemyPrefabs[4];
        }
        else
        {
            if (randomValue < Enemy0SpawnChance) return enemyPrefabs[0];
            if (randomValue < (Enemy0SpawnChance + Enemy1SpawnChance)) return enemyPrefabs[1];
            if (randomValue < (Enemy0SpawnChance + Enemy1SpawnChance + Enemy2SpawnChance)) return enemyPrefabs[2];
            return enemyPrefabs[3];
        }

    }
}
