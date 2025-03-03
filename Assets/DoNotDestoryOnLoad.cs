using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestoryOnLoad : MonoBehaviour
{
   private void Awake()
    {        
        // Set this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }
}
