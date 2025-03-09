using System.Collections;
using UnityEngine;

public class WorkerSpawner : MonoBehaviour
{
    public static WorkerSpawner Instance { get; private set; }
    public GameObject workerPrefab;
    public float spawnInterval = 1.5f;
    public float spawnRadius = 0.5f;
    private Transform spawnPoint;
    private static CustomSceneManager manager;

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
    void Start()
    {
        manager = CustomSceneManager.instance;
        spawnPoint = GameObject.FindGameObjectWithTag("Core").transform;
        CustomSceneManager.gameStateChange += OnGameStateChange;
    }

    void OnGameStateChange(GAMESTATE newState)
    {
        if (newState == GAMESTATE.BeforeGameStart)
        {
            StartCoroutine(SpawnWorkers());
        }
    }

    IEnumerator SpawnWorkers()
    {
        // Do not do anything if game is not running

        yield return null; // wait for one frame to ensure that the manager has been initialized
        while (manager.CanAddWorker())
        {
            // **Wait for the next spawn interval**
            yield return new WaitForSeconds(spawnInterval);

            manager.AddWorker();

            // **Generate a random spawn position within the defined radius**
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius; // Generates a random offset inside a circle
            Vector3 spawnPosition = new Vector3(spawnPoint.position.x + randomOffset.x, spawnPoint.position.y + randomOffset.y, spawnPoint.position.z);
            Instantiate(workerPrefab, spawnPosition, Quaternion.identity);
        }
        
    }
}
