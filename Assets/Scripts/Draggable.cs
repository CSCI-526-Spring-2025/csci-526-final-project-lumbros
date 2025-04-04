using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private Image mImage;
    private GridManager mGridManager;
    [HideInInspector] public Transform parentAfterDrag; 


    void Awake()
    {
        if (mImage == null) 
        {
            mImage = GetComponent<Image>(); 
        }
        GridManager[] GridArray = FindObjectsOfType<GridManager>();
        mGridManager = GridArray[0];
    }

    public void OnBeginDrag(PointerEventData eventData)
    {   
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        mImage.raycastTarget = false;
        mGridManager.ShowGrid();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        Debug.Log("end");
        transform.SetParent(parentAfterDrag);
        mImage.raycastTarget = true;
        TimerManager.StartTimer(0.15f,  mGridManager.HideGrid, true);
        
    }
}
