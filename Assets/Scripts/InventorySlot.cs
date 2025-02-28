using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;

        if(dropped.GetComponent<Draggable>() != null){
            Draggable draggableItem = dropped.GetComponent<Draggable>();
            draggableItem.parentAfterDrag = transform;
        }
        
        if( dropped.GetComponent<TowerDragger>() != null){
            TowerDragger draggableItem = dropped.GetComponent<TowerDragger>();
            draggableItem.parentAfterDrag = transform;
        }
    }
}
