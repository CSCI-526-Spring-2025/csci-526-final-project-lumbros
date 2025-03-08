using UnityEngine;
using UnityEngine.EventSystems;
public class TowerDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Reference to the description panel
    public GameObject descriptionPanel;

    // Display description when mouse enters
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(true);
        }
    }

    // Hide description when mouse exits
    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }
    }
}