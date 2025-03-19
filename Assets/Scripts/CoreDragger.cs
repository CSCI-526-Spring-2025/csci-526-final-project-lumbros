using UnityEngine;

public class CoreDragger : MonoBehaviour
{ 
    private Vector3 offset;
    private bool isDragging = false;
    private Camera mainCamera;

    private Vector3 originalScale;
    private Color originalColor;

    private SpriteRenderer spriteRenderer;

    [Header("Drag Appearance Settings")]
    public float dragScaleMultiplier = 0.8f;
    public float dragTransparency = 0.5f;
    private GridManager mGridManager;

    private bool canDrag = false;

    void Start()
    {
        mainCamera = Camera.main;
        originalScale = transform.localScale;

        // Get SpriteRenderer if it exists
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        GridManager[] GridArray = FindObjectsOfType<GridManager>();
        if (GridArray.Length > 0)
        {
            mGridManager = GridArray[0];
        }

        CustomSceneManager.gameStateChange += OnGameStateChange;
    }

    void OnGameStateChange(GAMESTATE newState)
    {
        if (newState == GAMESTATE.PlaceCore)
        {
            canDrag = true;
        }
        else
        {
            canDrag = false;
        }
    }

    void OnMouseDown()
    {
        if(!canDrag)
        {
            return;
        }

        mGridManager.ShowGrid();
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();

        // Shrink and make transparent
        transform.localScale = originalScale * dragScaleMultiplier;

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = dragTransparency;
            spriteRenderer.color = c;
        }
    }

    void OnMouseUp()
    {
        mGridManager.HideGrid();
        isDragging = false;

        // Reset to original size and opacity
        transform.localScale = originalScale;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // Add to nearest inventory slot
        InventorySlot[] slots = FindObjectsOfType<InventorySlot>();
        float minDistance = float.MaxValue;
        InventorySlot closestSlot = null;
        foreach (InventorySlot slot in slots)
        {
            float distance = Vector3.Distance(transform.position, slot.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestSlot = slot;
            }
        }
        if (closestSlot != null && closestSlot.CanAddItem())
        {
            closestSlot.AddExistingItem(gameObject);
        }
    }

    void Update()
    {
        if (isDragging && canDrag)
        {
            Vector3 mousePosition = GetMouseWorldPosition() + offset;
            transform.position = mousePosition;
        }
        else if (!canDrag)
        {
            isDragging = false;
        }
    }

    // Helper to get mouse position in world space
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
