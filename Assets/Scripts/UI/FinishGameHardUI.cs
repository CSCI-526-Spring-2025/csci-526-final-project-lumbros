using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGameHardUI : MonoBehaviour
{
    // Static reference to the instance of our GameOverUI
    public static FinishGameHardUI instance;

    private void Awake()
    {
        // Check if instance already exists
        if (instance == null)
        {
            // If not, set instance to this
            instance = this;
        }
        else if (instance != this)
        {
            // If instance already exists and it's not this, then destroy this to enforce the singleton.
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
