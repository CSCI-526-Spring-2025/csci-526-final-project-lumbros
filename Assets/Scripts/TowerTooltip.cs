using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TowerTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipPrefab;
    [TextArea(3, 10)]
    public string towerDescription;
    public Canvas targetCanvas;

    private GameObject tooltipInstance;

    // 启用调试模式
    public bool debugMode = true;

    void Start()
    {
        if (debugMode) Debug.Log($"[TowerTooltip] Started on {gameObject.name}");

        // 检查预制体
        if (tooltipPrefab == null)
        {
            Debug.LogError($"[TowerTooltip] Tooltip prefab is missing on {gameObject.name}!");
        }

        // 检查目标Canvas
        if (targetCanvas == null)
        {
            Debug.LogWarning($"[TowerTooltip] Target Canvas is not assigned on {gameObject.name}. Will use FindObjectOfType<Canvas>() instead.");
        }
    }

    void Update()
    {
        // 按空格键测试
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (debugMode) Debug.Log("[TowerTooltip] Space key pressed - testing tooltip");

            if (tooltipInstance == null)
            {
                ShowTooltip();
            }
            else
            {
                HideTooltip();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (debugMode) Debug.Log($"[TowerTooltip] OnPointerEnter triggered on {gameObject.name}");
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (debugMode) Debug.Log($"[TowerTooltip] OnPointerExit triggered on {gameObject.name}");
        HideTooltip();
    }

    private void ShowTooltip()
    {
        if (debugMode) Debug.Log("[TowerTooltip] ShowTooltip called");

        // 如果已有提示框，先销毁
        if (tooltipInstance != null)
        {
            if (debugMode) Debug.Log("[TowerTooltip] Destroying existing tooltip");
            Destroy(tooltipInstance);
        }

        // 检查预制体引用
        if (tooltipPrefab == null)
        {
            Debug.LogError("[TowerTooltip] Tooltip prefab is not assigned!");
            return;
        }

        // 获取Canvas
        Canvas canvas = targetCanvas;
        if (canvas == null)
        {
            if (debugMode) Debug.Log("[TowerTooltip] Searching for Canvas using FindObjectOfType");
            canvas = FindObjectOfType<Canvas>();

            if (canvas == null)
            {
                Debug.LogError("[TowerTooltip] No Canvas found in the scene!");
                return;
            }
            else
            {
                if (debugMode) Debug.Log($"[TowerTooltip] Found Canvas: {canvas.name}");
            }
        }

        // 记录按钮的位置信息
        RectTransform buttonRect = GetComponent<RectTransform>();
        if (debugMode)
        {
            Debug.Log($"[TowerTooltip] Button position: {buttonRect.position}");
            Debug.Log($"[TowerTooltip] Button anchoredPosition: {buttonRect.anchoredPosition}");
            Debug.Log($"[TowerTooltip] Button size: {buttonRect.rect.width} x {buttonRect.rect.height}");
        }

        // 创建提示框
        if (debugMode) Debug.Log("[TowerTooltip] Instantiating tooltip prefab");
        tooltipInstance = Instantiate(tooltipPrefab, canvas.transform);

        // 设置文本
        Text tooltipText = tooltipInstance.GetComponentInChildren<Text>();
        if (tooltipText != null)
        {
            tooltipText.text = towerDescription;
            if (debugMode) Debug.Log($"[TowerTooltip] Set tooltip text: {towerDescription}");
        }
        else
        {
            Debug.LogError("[TowerTooltip] No Text component found in tooltip prefab!");
        }

        // 设置位置
        RectTransform tooltipRect = tooltipInstance.GetComponent<RectTransform>();
        if (tooltipRect != null)
        {
            // 记录tooltip的初始大小
            if (debugMode)
            {
                Debug.Log($"[TowerTooltip] Tooltip original size: {tooltipRect.rect.width} x {tooltipRect.rect.height}");
            }

            // 使用多种方法设置位置，看哪一种有效
            // 方法1：使用position
            Vector2 position = buttonRect.position;
            position.x -= 200; // 在按钮左侧200像素处
            tooltipRect.position = position;

            if (debugMode)
            {
                Debug.Log($"[TowerTooltip] Tooltip set to position: {position}");
                Debug.Log($"[TowerTooltip] Tooltip actual position after setting: {tooltipRect.position}");
            }

            // 确保tooltip是激活的
            tooltipInstance.SetActive(true);
        }
        else
        {
            Debug.LogError("[TowerTooltip] Tooltip prefab does not have a RectTransform component!");
        }
    }

    private void HideTooltip()
    {
        if (debugMode) Debug.Log("[TowerTooltip] HideTooltip called");

        if (tooltipInstance != null)
        {
            if (debugMode) Debug.Log("[TowerTooltip] Destroying tooltip");
            Destroy(tooltipInstance);
            tooltipInstance = null;
        }
    }

    private void OnDisable()
    {
        HideTooltip();
    }
}