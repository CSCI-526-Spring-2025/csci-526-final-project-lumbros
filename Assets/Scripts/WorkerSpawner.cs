using System.Collections;
using UnityEngine;

public class WorkerSpawner : MonoBehaviour
{
    public GameObject workerPrefab;
    public float spawnInterval = 1.5f;
    public float spawnRadius = 0.5f;
    private Transform spawnPoint;
    private static CustomSceneManager manager;

    void Start()
    {
        manager = CustomSceneManager.instance;
        spawnPoint = GameObject.FindGameObjectWithTag("Core").transform;
        StartCoroutine(SpawnWorkers());
    }

    IEnumerator SpawnWorkers()
    {
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
