using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        StartCoroutine(DelayedSpawn());
    }

    IEnumerator DelayedSpawn()
    {
        yield return new WaitForEndOfFrame(); // Ensures this runs AFTER all Start() methods

        if (manager.shouldSpawnMine)
        {
            manager.shouldSpawnMine = false;
            SpawnMines();
        }
    }

    void SpawnMines()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "SampleScene")
        {
            /* Spawn in random positions */
            for (int i = 0; i < mineCount; i++)
            {
                Vector2 randomPosition = new Vector2(
                    Random.Range(minSpawnRange.x, maxSpawnRange.x),
                    Random.Range(minSpawnRange.y, maxSpawnRange.y)
                );

                Instantiate(minePrefab, randomPosition, Quaternion.identity);
            }
        }
        else if (sceneName == "MainScene")
        {
            /* Spawn in grid slots */
            InventorySlot[] slots = GridManager.Instance.GetInventorySlots();
            // Choose 3 unique random slots to spawn mines in
            for (int i = 0; i < mineCount; i++)
            {
                int randomSlotIndex = Random.Range(0, slots.Length);
                InventorySlot slot = slots[randomSlotIndex];
                if (!slot.CanAddItem())
                {
                    i--;
                    continue;
                }

                slot.AddItem(minePrefab);
            }
        }
    }
}
