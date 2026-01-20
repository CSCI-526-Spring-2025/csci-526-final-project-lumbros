using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    //private InventorySlot[] mSlots;
    private List<InventorySlot> mSlots;
    public InventorySlot slotPrefab;
    public GameObject gridObject;

   
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
        //mSlots = FindObjectsOfType<InventorySlot>();

        Vector3 initPos = new Vector3(37.5f, -37.5f, 0.0f);
        float size = 75;
        int rowLength = 11;// 11;
        int colLength = 17;// 17;
        mSlots = new List<InventorySlot>();
        for (int row = 0; row < rowLength; row++)
        {
            for (int col = 0; col < colLength; col++)
            {
                InventorySlot slot = Instantiate(slotPrefab, initPos + new Vector3(size * row, size * (col+1), 0.0f), Quaternion.identity);
                slot.gameObject.transform.parent = gridObject.transform;
                slot.transform.localScale = Vector3.one;
                slot.transform.localPosition = initPos + new Vector3(size * row, size * col, 0.0f);
                mSlots.Add(slot);
            }
        }

        HideGrid();
    }

   
    public void ShowGrid()
    {
        foreach (InventorySlot slot in mSlots)
        {
            Image img = slot.GetComponent<Image>();  
            if (img != null) 
            {
                if(CustomSceneManager.instance.curState == GAMESTATE.Tutorial && !slot.onlyTutorial){
                    continue;
                }
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

    //public InventorySlot[] GetInventorySlots()
    //{
    //    return mSlots;
    //}

    public List<InventorySlot> GetInventorySlots()
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
