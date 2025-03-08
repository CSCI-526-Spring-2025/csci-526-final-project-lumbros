using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 对应的描述面板
    public GameObject descriptionPanel;

    // 鼠标进入时显示描述
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(true);
        }
    }

    // 鼠标离开时隐藏描述
    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }
    }
}