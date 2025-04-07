using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButton : MonoBehaviour
{
    GameObject infoCanvas;

    void Start()
    {
        infoCanvas = GameObject.Find("InfoPanel"); // "InfoPanel" must match the name of your object in the scene
        if (infoCanvas != null)
        {
            infoCanvas.SetActive(false);
        }
    }

    public void ShowInfo()
    {
        if (infoCanvas != null)
            infoCanvas.SetActive(true);
    }

    public void HideInfo()
    {
        if (infoCanvas != null)
            infoCanvas.SetActive(false);
    }

    // Don't use this with Unity UI button
    public int clickme()
    {
        return 0;
    }
}
