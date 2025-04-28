using System.Collections;
using Unity.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }
    public GameObject[] enemyPrefabs; // 敌人预制体数组
    public float mapMinX = -8f;
    public float mapMaxX = 5.1f;
    public float mapMinY = -5f;
    public float mapMaxY = 4f;

    public float Enemy0SpawnChance = 0.3f;
    public float Enemy1SpawnChance = 0.2f;
    public float Enemy2SpawnChance = 0.2f;
    public float Enemy3SpawnChance = 0.2f;
    public float Enemy4SpawnChance = 0.1f;

    private int basic = 0;
    private int ranged = 1;
    private int stalker = 2;
    private int phantom = 3;
    private int boss = 4;
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

    private Vector3 GetRandomEdgePosition()
{
    int edge = Random.Range(0, 4); // 0: top, 1: bottom, 2: left, 3: right
    float x, y;

    switch (edge)
    {
        case 0: // Top
            x = Random.Range(mapMinX, mapMaxX);
            y = mapMaxY;
            break;
        case 1: // Bottom
            x = Random.Range(mapMinX, mapMaxX);
            y = mapMinY;
            break;
        case 2: // Left
            x = mapMinX;
            y = Random.Range(mapMinY, mapMaxY);
            break;
        case 3: // Right
            x = mapMaxX;
            y = Random.Range(mapMinY, mapMaxY);
            break;
        default:
            x = 0;
            y = 0;
            break;
    }

    return new Vector3(x, y, 0f);
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
        if ((currentWave % 8) == 0)
        {
            Vector3 spawnPosition = GetRandomEdgePosition();
            GameObject BossEnemy = Instantiate(enemyPrefabs[boss], spawnPosition, Quaternion.identity);

            WaveManager.Instance.NotifyBossSpawned(BossEnemy);
        }
        else
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

                Vector3 spawnPosition = GetRandomEdgePosition();

                GameObject enemyToSpawn = ChooseRandomEnemy(currentWave);
                //GameObject enemyToSpawn = enemyPrefabs[boss];

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
    }

    GameObject ChooseRandomEnemy(int currentWave)
    {
        float randomValue = Random.value;
        bool enemySpawn = DifficultyManager.GetEnemySpawn();
        if (enemySpawn == true)
        {
            switch (currentWave)
            {
                    case 1:
                    return enemyPrefabs[basic];

                case 2:
                    return enemyPrefabs[stalker];

                case 3:
                    return enemyPrefabs[ranged];

                default:
                if (randomValue < 0.4) return enemyPrefabs[basic];
                if (randomValue < 0.7) return enemyPrefabs[ranged];
                return enemyPrefabs[stalker];
            }
        }
        else
        {
            switch (currentWave)
            {
                case 1:
                return enemyPrefabs[basic];
                case 2:
                    return enemyPrefabs[stalker];

                case 3:
                    return enemyPrefabs[ranged];

                case 4:
                    return enemyPrefabs[phantom];                    
                
                default:
                if (randomValue < Enemy0SpawnChance) return enemyPrefabs[basic];
                if (randomValue < Enemy0SpawnChance + Enemy1SpawnChance) return enemyPrefabs[ranged];
                if (randomValue < Enemy0SpawnChance + Enemy1SpawnChance + Enemy2SpawnChance) return enemyPrefabs[stalker];
                return enemyPrefabs[phantom];
            }
            
        }
        //return enemyPrefabs[ranged];
    }
}
