using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WaveManager : MonoBehaviour
{
    public TMP_Text mWavesUI;
    private int mWaves = 1;
    public static WaveManager Instance { get; private set; }
/*************************Aaron****************************/
    public int currentWave = 1;    // 当前波次
    public int baseEnemyCount = 8; // 第一波敌人数量
    public float enemyStatMultiplier = 2f; // 每一波敌人属性增强倍率
    public float waveInterval = 5f; // 每波修整时间
    public int WaveKillLimit = 1;
    public int KillperWave;
    public float enemyHealthMultiplier;
    public float enemyDamageMultiplier;
    private bool isEndingWave = false;
    
    private EnemySpawner enemySpawner;
    private CustomSceneManager sceneManager;

/*************************Aaron****************************/
    private void Awake()
    {
        // Check if instance already exists
        if (Instance == null)
        {
            // If not, set instance to this
            Instance = this;
        }
        else if (Instance != this)
        {
            // If instance already exists and it's not this, then destroy this to enforce the singleton.
            Destroy(gameObject);
        }
        
        // Set this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        sceneManager = FindObjectOfType<CustomSceneManager>();
        StartWave();
    }

    public void StartWave()
    {
        Debug.Log($"Wave {currentWave} starting...");
        KillperWave = 0; // 重置击杀数
        
        int enemyCount = baseEnemyCount + (currentWave - 1) * 3; // 随波次增加敌人
        enemyHealthMultiplier = Mathf.Pow(enemyStatMultiplier, currentWave - 1);
        enemyDamageMultiplier = Mathf.Pow(enemyStatMultiplier, currentWave - 1);

        enemySpawner.SpawnWave(enemyCount);
    }

    public void CheckWaveEnd()
    {
        if (KillperWave >= WaveKillLimit && !isEndingWave)
        {
            StartCoroutine(EndWave());
        }
    }

    IEnumerator EndWave()
    {
        if (isEndingWave == false)
        {
            isEndingWave = true;
            Debug.Log($"Wave {currentWave} ended! Clearing enemies...");

            // 停止敌人生成
            enemySpawner.StopAllCoroutines(); 
            ClearAllEnemies();
            yield return new WaitForSeconds(waveInterval);

            WaveKillLimit = WaveKillLimit + 2;
            currentWave++;
            
            StartWave();
            isEndingWave = false;
        }
    }

    void ClearAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
/*************************Aaron****************************/

    // Update is called once per frame
    void Update()
    {
    //    mWavesUI.text = "Wave " + mWaves.ToString();
    }


    public void NextWave(){
        mWaves += 1;
    }
}
