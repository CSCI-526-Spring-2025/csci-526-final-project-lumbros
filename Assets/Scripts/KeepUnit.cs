using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepUnit : MonoBehaviour
{
    public CustomSceneManager aInstance;
    private void Awake()
    {        
        // Set this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    IEnumerator Start(){
        yield return new WaitUntil(() => CustomSceneManager.IsInitialized);
        GameObject manager = GameObject.FindGameObjectWithTag("Manager");
        manager.GetComponent<CustomSceneManager>().AddNonDestoryObject(gameObject);
    }
}
