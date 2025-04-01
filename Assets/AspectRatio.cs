using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatio : MonoBehaviour
{
    void Start(){
       Debug.Log("adjust");
        Adjust();
    }
    public void Adjust(){
        float targetaspect = 16.0f / 10.0f;
        float windowaspect = (float)Screen.width / (float) Screen.height;
        float scaleHeight = windowaspect / targetaspect;
        if(scaleHeight < 1.0f){
             Debug.Log("Scale Height");
            Rect rect = GetComponent<Camera>().rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            GetComponent<Camera>().rect = rect; 
        }
        else{
             Debug.Log("Scale Width");
            float scaleWidth = 1.0f/ scaleHeight;
            Rect rect = GetComponent<Camera>().rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            GetComponent<Camera>().rect = rect;
        }
    }

}
