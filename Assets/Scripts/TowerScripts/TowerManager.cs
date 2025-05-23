using NavMeshPlus.Components;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    // Static reference to the instance of our SceneManager
    public static TowerManager Instance;
    public int maxTowerCount;
    public int curTowerCount;
    public static int DEFAULT_MAX = 3;
    public static int DEFAULT_CUR = 0;
    public Dictionary<GameObject, InventorySlot> towerSlotMap = new Dictionary<GameObject, InventorySlot>();
    public NavMeshSurface navSurface;
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

    // Start is called before the first frame update
    void Start()
    {
        maxTowerCount = DEFAULT_MAX;
        curTowerCount = DEFAULT_CUR;
        CustomSceneManager.gameStateChange += OnGameStateChange;
    }

    void OnGameStateChange(GAMESTATE newState)
    {
        if (newState == GAMESTATE.GameStart||newState == GAMESTATE.Tutorial)
        {
            navSurface = GameObject.Find("NavMesh Surface").GetComponent<NavMeshSurface>();
            navSurface.BuildNavMesh();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset(){
        maxTowerCount = DEFAULT_MAX;
        curTowerCount = DEFAULT_CUR;
    }

    public void IncreaseTowerCount()
    {
        maxTowerCount++;
    }

    public void AddTower(GameObject tower, InventorySlot slot)

    {
        Debug.Log("TowerManager: adding tower " + tower.name + slot.name);
        towerSlotMap[tower] = slot;
        curTowerCount++;

        // Update NavMesh
        navSurface.UpdateNavMesh(navSurface.navMeshData);
    }

    public void DestoryTower(GameObject tower)
    {
        if (towerSlotMap.TryGetValue(tower, out InventorySlot slot))
        {
            Debug.Log("TowerManager: removing tower " + tower.name + slot.name);
            if (slot != null)
            {
                slot.EmptySlot();
            }
            towerSlotMap.Remove(tower);
        }
        else
        {
            Debug.LogWarning("⚠️ TowerManager: Tower not found in slot map: " + tower.name);
        }

    Destroy(tower);
    curTowerCount--;

    }

    public void RefillSlots()
    {
        // Set InventorySlot containsItem to true for the towers in the towerSlotMap
        foreach (var kvp in towerSlotMap)
        {
            GameObject tower = kvp.Key;
            InventorySlot slot = kvp.Value;

            if (tower != null && slot != null)
            {
                slot.containsItem = true;
            }
        }
    }

        public bool CanAddTower()
    {
        return curTowerCount < maxTowerCount;
    }
}
