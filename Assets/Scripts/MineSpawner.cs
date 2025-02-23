using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    public GameObject minePrefab; // Assign your mine prefab in the Inspector
    public int mineCount = 3;
    public Vector2 minSpawnRange = new Vector2(-4.5f, -4.5f);
    public Vector2 maxSpawnRange = new Vector2(4.5f, 4.5f);

    private static CustomSceneManager manager;

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<CustomSceneManager>();
        if (manager.shouldSpawnMine)
        {
            manager.shouldSpawnMine = false;
            SpawnMines();
        }
    }

    void SpawnMines()
    {
        for (int i = 0; i < mineCount; i++)
        {
            Vector2 randomPosition = new Vector2(
                Random.Range(minSpawnRange.x, maxSpawnRange.x),
                Random.Range(minSpawnRange.y, maxSpawnRange.y)
            );

            Instantiate(minePrefab, randomPosition, Quaternion.identity);
        }
    }
}
