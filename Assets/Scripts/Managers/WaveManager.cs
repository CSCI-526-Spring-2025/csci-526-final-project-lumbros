using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class WaveManager : MonoBehaviour
{
    private TMP_Text mWavesUI;
    private int mWaves = 1;
    public static WaveManager Instance { get; private set; }
/*************************Aaron****************************/
    public int currentWave = 1;    // 当前波次

    public int baseEnemyCountMemory = 8;
    public int baseEnemyCount = 8; // 第一波敌人数量
    public float enemyStatMultiplier = 2f; // 每一波敌人属性增强倍率
    public float waveInterval = 5f; // 每波修整时间
    // Number we need to kill to move on to the next wave 
    public int WaveKillLimit = 1;

    public int KillperWave;
    public float enemyHealthMultiplier;
    public float enemyDamageMultiplier;
    private bool isEndingWave = false;
    
    private EnemySpawner enemySpawner;
    private CustomSceneManager sceneManager;
    private bool CurrWave = false;

    private float waveTimer = 5f;

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
        CustomSceneManager.gameStateChange += OnGameStateChange;
        //StartWave();
        //CurrWave = true;
    }

    void OnGameStateChange(GAMESTATE newState)
    {
        if(newState == GAMESTATE.GamePlay)
        {
            StartCoroutine(StartWaveCoroutine());
        }
    }

    IEnumerator StartWaveCoroutine()
    {
        waveTimer = 5f;
        yield return new WaitForSeconds(waveInterval);
        StartWave();
    }

    public void LoadStartingWave(){
        currentWave = 1;
        baseEnemyCount = baseEnemyCountMemory;
        WaveKillLimit = 1;
    }
    public void StartWave()
    {
        // For the timer
        CurrWave = true;
        Debug.Log($"Wave {currentWave} starting...");
        KillperWave = 0; // 重置击杀数
        mWavesUI.text = "Wave " + currentWave.ToString();
        int enemyCount = baseEnemyCount + (currentWave - 1) * 3; // 随波次增加敌人
        enemyHealthMultiplier = Mathf.Pow(enemyStatMultiplier, currentWave - 1);
        enemyDamageMultiplier = Mathf.Pow(enemyStatMultiplier, currentWave - 1);

        enemySpawner.SpawnWave(enemyCount);
    }

    public void CheckWaveEnd()
    {
        if (KillperWave >= WaveKillLimit && !isEndingWave)
        {
            CurrWave = false;
            waveTimer = 5f;
            int tempWave = currentWave + 1;
            mWavesUI.text = "Wave "  + currentWave.ToString() + " starting in " + FormatTime(waveTimer);
            //StartCoroutine(EndWave());
            EndWave();
        }
    }

     void EndWave()
    {
        if (isEndingWave == false)
        {
            isEndingWave = true;
            Debug.Log($"Wave {currentWave} ended! Clearing enemies...");

            // 停止敌人生成
            enemySpawner.StopAllCoroutines(); 
            ClearAllEnemies();
            //yield return new WaitForSeconds(waveInterval);

            WaveKillLimit = WaveKillLimit + 2;
            currentWave++;

            //StartWave();
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
        if (mWavesUI != null){
            mWavesUI.text = "Wave " + currentWave.ToString();
        }

        if( CurrWave == false)
        {
            waveTimer -= Time.deltaTime;
            int tempWave = currentWave + 1;
            mWavesUI.text = "Wave "  + currentWave.ToString() + " starting in " + FormatTime(waveTimer);
        }        

    }


    public void NextWave(){
        mWaves += 1;
    }

    public int GetKillsCount(){
        return WaveKillLimit;
    }

    string FormatTime(float timeInSeconds)
    {
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:0}", seconds);
    }

     private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Handle UI 
    private void OnDisable()
    {   
        SceneManager.sceneLoaded -= OnSceneLoaded; 
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mWavesUI = GameObject.Find("WaveTextUI")?.GetComponent<TMP_Text>();
    }
}
