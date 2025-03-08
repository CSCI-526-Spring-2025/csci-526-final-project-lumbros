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

    // ���õ���ģʽ
    public bool debugMode = true;

    void Start()
    {
        if (debugMode) Debug.Log($"[TowerTooltip] Started on {gameObject.name}");

        // ���Ԥ����
        if (tooltipPrefab == null)
        {
            Debug.LogError($"[TowerTooltip] Tooltip prefab is missing on {gameObject.name}!");
        }

        // ���Ŀ��Canvas
        if (targetCanvas == null)
        {
            Debug.LogWarning($"[TowerTooltip] Target Canvas is not assigned on {gameObject.name}. Will use FindObjectOfType<Canvas>() instead.");
        }
    }

    void Update()
    {
        // ���ո������
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

        // ���������ʾ��������
        if (tooltipInstance != null)
        {
            if (debugMode) Debug.Log("[TowerTooltip] Destroying existing tooltip");
            Destroy(tooltipInstance);
        }

        // ���Ԥ��������
        if (tooltipPrefab == null)
        {
            Debug.LogError("[TowerTooltip] Tooltip prefab is not assigned!");
            return;
        }

        // ��ȡCanvas
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

        // ��¼��ť��λ����Ϣ
        RectTransform buttonRect = GetComponent<RectTransform>();
        if (debugMode)
        {
            Debug.Log($"[TowerTooltip] Button position: {buttonRect.position}");
            Debug.Log($"[TowerTooltip] Button anchoredPosition: {buttonRect.anchoredPosition}");
            Debug.Log($"[TowerTooltip] Button size: {buttonRect.rect.width} x {buttonRect.rect.height}");
        }

        // ������ʾ��
        if (debugMode) Debug.Log("[TowerTooltip] Instantiating tooltip prefab");
        tooltipInstance = Instantiate(tooltipPrefab, canvas.transform);

        // �����ı�
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

        // ����λ��
        RectTransform tooltipRect = tooltipInstance.GetComponent<RectTransform>();
        if (tooltipRect != null)
        {
            // ��¼tooltip�ĳ�ʼ��С
            if (debugMode)
            {
                Debug.Log($"[TowerTooltip] Tooltip original size: {tooltipRect.rect.width} x {tooltipRect.rect.height}");
            }

            // ʹ�ö��ַ�������λ�ã�����һ����Ч
            // ����1��ʹ��position
            Vector2 position = buttonRect.position;
            position.x -= 200; // �ڰ�ť���200���ش�
            tooltipRect.position = position;

            if (debugMode)
            {
                Debug.Log($"[TowerTooltip] Tooltip set to position: {position}");
                Debug.Log($"[TowerTooltip] Tooltip actual position after setting: {tooltipRect.position}");
            }

            // ȷ��tooltip�Ǽ����
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