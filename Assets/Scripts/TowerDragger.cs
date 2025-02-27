using UnityEngine;
using UnityEngine.EventSystems;  // Required for handling UI events
using UnityEngine.UI;

public class TowerDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Reference to the tower we want to build
    public GameObject towerPrefab;

    // Used to show semi-transparent preview while dragging
    private GameObject currentTowerPreview;
    private static GameObject manager; // manage the game state
    private GridManager mGridManager;
    [HideInInspector] public Transform parentAfterDrag; 
    private Image mImage;
    private int mCost;

    void Start()
    {

        manager = GameObject.FindGameObjectWithTag("Manager");
        if (mImage == null) 
        {
            mImage = GetComponent<Image>(); 
        }
        GridManager[] GridArray = FindObjectsOfType<GridManager>();
        if(GridArray.Length > 0)
        {
            mGridManager = GridArray[0];
        }
        mCost = towerPrefab.GetComponent<Tower>().GetCost();
    }

    // Called when player starts dragging from the UI button
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (manager.GetComponent<CustomSceneManager>().CanAddTower() && MoneyManager.Instance.mMoney > mCost)
        {
            // Grid 
            mImage.raycastTarget = false;
            if(mGridManager != null)
            {
                mGridManager.ShowGrid();
            }
                

            // Create a preview instance of the tower
            currentTowerPreview = Instantiate(towerPrefab);
            Destroy(currentTowerPreview.GetComponent<AutoAttack>());
            Destroy(currentTowerPreview.GetComponent<Health>());
            Destroy(currentTowerPreview.GetComponent<Rigidbody2D>());
            Destroy(currentTowerPreview.GetComponent<BoxCollider2D>());
            // Adjust scale
            currentTowerPreview.transform.localScale = new Vector3(0.1f, 0.1f, 1f);  
            // Make the preview semi-transparent to distinguish it from actual towers
            SpriteRenderer sr = currentTowerPreview.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Debug.Log("render * 50%");
                Color color = sr.color;
                color.a = 0.5f;  // Set to 50% opacity
                sr.color = color;
            }
        }
    }

    // Called every frame while dragging
    public void OnDrag(PointerEventData eventData)
    {
       if (currentTowerPreview != null)
        {
            // Convert screen position (mouse) to world position
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            currentTowerPreview.transform.position = worldPos;
        }
    }

    // Called when player releases the mouse button
    public void OnEndDrag(PointerEventData eventData)
    {   
        Destroy(currentTowerPreview);
        mImage.raycastTarget = true;
        if (manager.GetComponent<CustomSceneManager>().CanAddTower() && currentTowerPreview != null)
        {
            // Instantiate in inventory slot
            TimerManager.StartTimer(0.15f,  mGridManager.HideGrid, true);
        }
        else
        {
            mGridManager.HideGrid();
        }
      
    }
}