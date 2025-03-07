using System.Collections;
using System.Collections.Generic;
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

        if (!dropped.CompareTag("Tower") || GamerManager.GetComponent<CustomSceneManager>().CanAddTower())
        {
            // Instantiate new object   
            if (prefabLookup.ContainsKey(droppedItemName))
            {
                int Cost = prefabToInstantiate.GetComponent<Tower>().GetCost();

                if(MoneyManager.Instance.mMoney >= Cost)
                {
                    AddItem(prefabToInstantiate);
                    MoneyManager.Instance.UpdateMoney(Cost * -1);
                    // Check if the dropped object is a tower
                    if (dropped.CompareTag("Tower"))
                    {
                        GamerManager.GetComponent<CustomSceneManager>().AddTower();
                    }
                }
            }
            else
            {
                Debug.LogWarning("No matching prefab found for: " + droppedItemName);
            }
        }
    }


    public void AddItem(GameObject prefab)
    {
        GameObject newItem = Instantiate(prefab, transform.position, Quaternion.identity);
        Vector3 pos = newItem.transform.position;
        pos.z = 0;
        newItem.transform.position = pos;
        newItem.transform.localScale = new Vector3(0.10f,0.10f,0.10f);
        containsItem = true;
    }

    public bool CanAddItem()
    {
        return !containsItem;
    }
}
