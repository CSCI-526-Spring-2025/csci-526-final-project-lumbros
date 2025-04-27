using UnityEngine;
using UnityEngine.EventSystems;  // Required for handling UI events

public class BaseBuildingDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Reference to the tower we want to build
    public GameObject towerPrefab;

    // Used to show semi-transparent preview while dragging
    private GameObject currentTowerPreview;
    private static GameObject manager; // manage the game state

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager");
    }

    // Called when player starts dragging from the UI button
    public void OnBeginDrag(PointerEventData eventData)
    {
        // if (!manager.GetComponent<CustomSceneManager>().CanAddTower())
        // {
        //     return;
        // }

        // Create a preview instance of the tower
        currentTowerPreview = Instantiate(towerPrefab);
        Destroy(currentTowerPreview.GetComponent<AutoAttack>());
        Destroy(currentTowerPreview.GetComponent<Health>());
        Destroy(currentTowerPreview.GetComponent<Rigidbody2D>());
        Destroy(currentTowerPreview.GetComponent<BoxCollider2D>());

        // Make the preview semi-transparent to distinguish it from actual towers
        SpriteRenderer renderer = currentTowerPreview.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            color.a = 0.5f;  // Set to 50% opacity
            renderer.color = color;
        }
    }

    // Called every frame while dragging
    public void OnDrag(PointerEventData eventData)
    {
        if (currentTowerPreview != null)
        {
            // Convert screen position (mouse) to world position
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;  // Ensure tower stays in 2D plane
            currentTowerPreview.transform.position = worldPos;
        }
    }

    // Called when player releases the mouse button
    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentTowerPreview != null)
        {
            // 获取最终位置
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;

            // 添加的代码开始 - 检查是否在核心附近
            GameObject core = GameObject.FindGameObjectWithTag("Core");
            if (core != null)
            {
                float distance = Vector3.Distance(worldPos, core.transform.position);
                if (distance < 1.0f) // 如果太靠近核心
                {
                    Debug.Log("无法在核心上放置防御塔");
                    Destroy(currentTowerPreview);
                    return; // 直接返回，不创建塔
                }
            }
            // 添加的代码结束

            // 创建实际的塔
            Instantiate(towerPrefab, worldPos, Quaternion.identity);

            // 清理预览
            Destroy(currentTowerPreview);
        }
    }
}