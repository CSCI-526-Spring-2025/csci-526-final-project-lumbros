using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    // Static reference to the instance of our SceneManager
    public static TowerManager instance;
    public int maxTowerCount;
    public int curTowerCount;
    public static int DEFAULT_MAX = 3;
    public static int DEFAULT_CUR = 0;
    public Dictionary<GameObject, InventorySlot> towerSlotMap = new Dictionary<GameObject, InventorySlot>();

    private void Awake()
    {
        // Check if instance already exists
        if (instance == null)
        {
            // If not, set instance to this
            instance = this;
        }
        else if (instance != this)
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
        towerSlotMap[tower] = slot;
        curTowerCount++;
    }

    public void DestoryTower(GameObject tower)
    {
        InventorySlot slot = towerSlotMap[tower];
        if(slot != null){
            slot.EmptySlot();
        }
        towerSlotMap.Remove(tower);
        Destroy(tower);
        curTowerCount--;
    }

    public bool CanAddTower()
    {
        return curTowerCount < maxTowerCount;
    }
}
