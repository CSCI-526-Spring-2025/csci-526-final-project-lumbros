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

    void Start()
    {
        prefabLookup = new Dictionary<string, GameObject>();
        GamerManager = GameObject.FindGameObjectWithTag("Manager");
        // Initialize the dictionary and map names to prefabs
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Towers");
        foreach (GameObject prefab in prefabs)
        {
            // Use the name of the prefab as the key
            Debug.Log("prefab name: " +  prefab.name);
            prefabLookup[prefab.name] = prefab;  
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Get the name of the dropped object
        GameObject dropped = eventData.pointerDrag;
        string droppedItemName = eventData.pointerDrag.name;
        GameObject prefabToInstantiate = prefabLookup[droppedItemName];

        if (GamerManager.GetComponent<CustomSceneManager>().CanAddTower())
        {
            // Instantiate new object   
            if (prefabLookup.ContainsKey(droppedItemName))
            {
                int Cost = prefabToInstantiate.GetComponent<Tower>().GetCost();

                if(MoneyManager.Instance.mMoney >= Cost)
                {
                    GameObject newItem = Instantiate(prefabToInstantiate, transform);
                    Vector3 pos = newItem.transform.position;
                    // pos.z = 0;
                    newItem.transform.position = pos;
                    //Update Costs
                    MoneyManager.Instance.UpdateMoney(Cost * -1);
                    GamerManager.GetComponent<CustomSceneManager>().AddTower();
                }
            }
            else
            {
                Debug.LogWarning("No matching prefab found for: " + droppedItemName);
            }
        }
    }
}
