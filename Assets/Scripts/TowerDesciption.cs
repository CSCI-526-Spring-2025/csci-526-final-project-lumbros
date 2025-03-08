using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // ��Ӧ���������
    public GameObject descriptionPanel;

    // ������ʱ��ʾ����
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(true);
        }
    }

    // ����뿪ʱ��������
    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }
    }
}