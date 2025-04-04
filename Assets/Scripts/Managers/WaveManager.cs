using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
public class WaveManager : MonoBehaviour
{
    private TMP_Text mWavesUI;
    private GameObject mWavesCountdown;
    private TMP_Text mWavesCountdownUI;
    private int mWaves = 1;
    public static WaveManager Instance { get; private set; }
    public static event Action<int, int> waveBegin;
    public static event Action<int> waveEnd;

    /*************************Aaron****************************/
    public int currentWave = 1;    // 当前波次
    public BossEnemy bossEnemy;
    private float enemyStatMultiplier = 1.1f; // 每一波敌人属性增强倍率
    private float Span_Interval_Multiplier = 1.3f; // 生成间隔加快倍率
    private float KillNumMultiplier = 1.4f; 
    private float waveInterval = 5f; // 每波修整时间
    // Number we need to kill to move on to the next wave 
    private int WaveKillLimit;
    private int WaveKillLimit_Intial = 5;

    public int KillperWave;
    public float enemyHealthMultiplier;
    public float enemyDamageMultiplier;
    public float SpanIntervalMultiplier;
    private float SpawnInterval = 1f;
    private bool isEndingWave = false;
    
    private EnemySpawner enemySpawner;
    private CustomSceneManager sceneManager;
    private bool CurrWave = false;

    private float waveTimer = 5f;
    private int enemyCount;
    private bool bossSpawned = false;
    private GameObject currentBoss = null;


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
        if(newState == GAMESTATE.GameTutorialHeroMoveAndAttack){
            StartCoroutine(StartTutorialCoroutine());
        }
        if(newState == GAMESTATE.GamePlay)
        {
            StartCoroutine(StartWaveCoroutine());
        }
    }

    IEnumerator StartTutorialCoroutine()
    {
        yield return new WaitForSeconds(5);
        // spawn melee enemies
        enemyHealthMultiplier = 1;
        WaveKillLimit = 2;
        KillperWave = 0;
        enemySpawner.SpawnWave(2, 1, 4);
    }

    IEnumerator StartWaveCoroutine()
    {
        waveTimer = 5f;

        /* Uncomment this block to test core drag and place */
        //sceneManager.UpdateGameState(GAMESTATE.PlaceCore);
        //yield return new WaitForSeconds(waveInterval);
        //sceneManager.UpdateGameState(GAMESTATE.DummyState);

        yield return new WaitForSeconds(waveInterval);
        StartWave();
    }

    public void LoadStartingWave(){
        currentWave = 1;
        //baseEnemyCount = baseEnemyCountMemory;
        WaveKillLimit = WaveKillLimit_Intial;
    }

    public void StartWave()
    {
        // For the timer
        CurrWave = true;
        Debug.Log($"Wave {currentWave} starting...");
        KillperWave = 0; // 重置击杀数
        mWavesUI.text = "Wave " + currentWave.ToString();
        //int enemyCount = baseEnemyCount + (currentWave - 1) * 3; // 随波次增加敌人
        enemyCount = WaveKillLimit;
        Debug.Log($"enemyCount {enemyCount}");
        Debug.Log($"WK {WaveKillLimit}");
        enemyHealthMultiplier = Mathf.Pow(enemyStatMultiplier, currentWave - 1);
        enemyDamageMultiplier = Mathf.Pow(enemyStatMultiplier, currentWave - 1);
        SpanIntervalMultiplier = Mathf.Pow(Span_Interval_Multiplier, currentWave - 1);
        KillNumMultiplier = Mathf.Pow(Span_Interval_Multiplier, currentWave - 1);

        Debug.Log($"Multiplier {enemyDamageMultiplier}");
        SpawnInterval = 1 / SpanIntervalMultiplier;

        enemySpawner.SpawnWave(enemyCount, currentWave, SpawnInterval);
        waveBegin?.Invoke(currentWave, WaveKillLimit);
    }

    public void NotifyBossSpawned(GameObject boss)
    {
        currentBoss = boss;
        bossSpawned = true;
    }

    public void CheckWaveEnd()
    {
        if (isEndingWave) return;
        if (currentWave % 10 == 0)
        {
            // Boss wave
            if (!bossSpawned) return;
            if (currentBoss == null)
            {
                CurrWave = false;
                waveTimer = 5f;
                int tempWave = currentWave + 1;
                mWavesUI.text = "Wave "  + currentWave.ToString() + " starting in " + FormatTime(waveTimer);
                //StartCoroutine(EndWave());
                EndWave();
            }
        }
        else if (KillperWave >= WaveKillLimit && !isEndingWave)
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

            waveEnd?.Invoke(currentWave);

            // 停止敌人生成
            enemySpawner.StopAllCoroutines(); 
            ClearAllEnemies();
            //yield return new WaitForSeconds(waveInterval);
            if (currentWave % 10 == 9)
            {
                WaveKillLimit = 31;
            }
            //WaveKillLimit = (int)Mathf.Round(WaveKillLimit * KillNumMultiplier);
            else
            {
                WaveKillLimit = Mathf.CeilToInt(WaveKillLimit_Intial * KillNumMultiplier);
            }
            currentWave++;

            //StartWave();
            isEndingWave = false;
        }
    }

    void ClearAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] Bullets = GameObject.FindGameObjectsWithTag("bullet");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        foreach (GameObject bullet in Bullets)
        {
            Destroy(bullet);
        }
    }
/*************************Aaron****************************/

    // Update is called once per frame
    void Update()
    {
        if (mWavesUI != null){
            mWavesUI.text = "Wave " + currentWave.ToString();
            if(mWavesCountdown.activeSelf){
                DisableCountdown();
            }
        }

        if( CurrWave == false 
            && (CustomSceneManager.instance.curState != GAMESTATE.GameTutorialUpgrades
                && CustomSceneManager.instance.curState != GAMESTATE.GameUpgrade))
        {
            waveTimer -= Time.deltaTime;
            if(waveTimer <= 0)
            {
                waveTimer = 0f;
            }
            int tempWave = currentWave + 1;
            mWavesUI.text = "Wave "  + currentWave.ToString() + " starting in " + FormatTime(waveTimer);
            mWavesCountdownUI.text = FormatTime(waveTimer);
            if(!mWavesCountdown.activeSelf){
                mWavesCountdown.SetActive(true);
            }
            if(mWavesCountdownUI.text == "0"){
                DisableCountdown();
            }
        }        

    }


    public void NextWave(){
        mWaves += 1;
    }

    public int GetKillsCount(){
        if (!CurrWave )
        {
            return 0;
        }
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
        Debug.Log("onload");
        mWavesUI = GameObject.Find("WaveTextUI")?.GetComponent<TMP_Text>();
        if(mWavesCountdown == null) mWavesCountdown = GameObject.Find("WaveCountdownUI");
        if(mWavesCountdown != null){
            mWavesCountdownUI = mWavesCountdown.GetComponent<TMP_Text>();
        }
    }

    private void DisableCountdown(){
       mWavesCountdown.SetActive(false);
    }
}
