using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGameUI : MonoBehaviour
{
    // Static reference to the instance of our GameOverUI
    public static FinishGameUI instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
