using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    // Map object names/types to prefabs
    private Dictionary<string, GameObject> prefabLookup;
    private static GameObject GamerManager; // manage the game state
    private bool containsItem;
    public static System.Action<string> AddedTower;

    void Start()
    {
        prefabLookup = new Dictionary<string, GameObject>();
        GamerManager = GameObject.FindGameObjectWithTag("Manager");
        // Initialize the dictionary and map names to prefabs
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Towers");
        foreach (GameObject prefab in prefabs)
        {
            // Use the name of the prefab as the key
            // Debug.Log("prefab name: " +  prefab.name);
            prefabLookup[prefab.name] = prefab;  
        }
        containsItem = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (containsItem)
            return;

        // Get the name of the dropped object
        GameObject dropped = eventData.pointerDrag;
        string droppedItemName = eventData.pointerDrag.name;
        GameObject prefabToInstantiate = prefabLookup[droppedItemName];

        Debug.Log("something dropped " + droppedItemName);

        // If Core
        if(droppedItemName == "Core")
        {
            AddExistingItem(dropped);
        }
        // Instantiate new object   
        else if (prefabLookup.ContainsKey(droppedItemName))
        {
            int Cost = prefabToInstantiate.GetComponent<Tower>().GetCost();

            if(MoneyManager.Instance.mMoney >= Cost)
            {
                GameObject newItem = AddNewItem(prefabToInstantiate);
                MoneyManager.Instance.UpdateMoney(Cost * -1);
                // AddedTower?.Invoke(prefabToInstantiate.name);
                Debug.Log("In Inventory Slot Dopped " + newItem.tag);
                // Check if the dropped object is a tower
                if (newItem.CompareTag("Tower"))
                {
                    TowerManager.Instance.AddTower(newItem, this);
                    // GamerManager.GetComponent<CustomSceneManager>().AddTower(newItem, this);
                }
            }
        }
        else
        {
            Debug.LogWarning("No matching prefab found for: " + droppedItemName);
        }
    }


    public GameObject AddNewItem(GameObject prefab)
    {
        GameObject newItem = Instantiate(prefab, transform.position, Quaternion.identity);
        Vector3 pos = newItem.transform.position;
        pos.z = 0;
        newItem.transform.position = pos;
        newItem.transform.localScale = new Vector3(0.15f,0.15f,0.15f);
        containsItem = true;
        return newItem;
    }

    public void AddExistingItem(GameObject item)
    {
        Vector3 pos = transform.position;
        pos.z = 0;
        item.transform.position = pos;
        //item.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        containsItem = true;
    }

    public bool CanAddItem()
    {
        return !containsItem;
    }
    public bool EmptySlot()
    {
        return containsItem = false;
    }

}
