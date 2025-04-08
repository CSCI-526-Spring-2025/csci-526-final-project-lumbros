using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    private InventorySlot[] mSlots;

   
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
        mSlots = FindObjectsOfType<InventorySlot>();
        HideGrid();
    }

   
    public void ShowGrid()
    {
        foreach (InventorySlot slot in mSlots)
        {
            Image img = slot.GetComponent<Image>();  
            if (img != null) 
            {
                Color color = img.color; 
                color.a = 1f;              
                img.color = color; 
            }
        }
    }

    public void HideGrid()
    {
        foreach (InventorySlot slot in mSlots)
        {
             Image img = slot.GetComponent<Image>();  
            if (img != null) 
            {
                Color color = img.color; 
                color.a = 0f;              
                img.color = color; 
            }
        }

    }

    public InventorySlot[] GetInventorySlots()
    {
        return mSlots;
    }

    public void EmptyAllSlots()
    {
        foreach (InventorySlot slot in mSlots)
        {
            slot.EmptySlot();
        }
    }
}
