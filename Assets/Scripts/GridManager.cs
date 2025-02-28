using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    private InventorySlot[] mSlots; 
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
}
