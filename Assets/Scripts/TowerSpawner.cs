using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerSpawner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
  
    private Image mImage;
    private GridManager mGridManager;
    [HideInInspector] public Transform parentAfterDrag; 
    public int mCost;

    public GameObject towerPrefab;

    private Vector3 initalPosition;

    void Awake()
    {
        if (mImage == null) 
        {
            mImage = GetComponent<Image>(); 
        }
        GridManager[] GridArray = FindObjectsOfType<GridManager>();
        mGridManager = GridArray[0];
        initalPosition = transform.position;
        Instantiate(towerPrefab, initalPosition, Quaternion.identity);
        Debug.Log("Tower Spawner " + initalPosition);
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
