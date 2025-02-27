using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WaveManager : MonoBehaviour
{
    public TMP_Text mWavesUI;
    private int mWaves = 1;
    public static WaveManager Instance { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mWavesUI.text = "Wave " + mWaves.ToString();
    }


    public void NextWave(){
        mWaves += 1;
    }
}
