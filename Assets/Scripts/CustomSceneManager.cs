using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

public enum GAMESTATE{
    BeforeGameStart, // Before "Start Game" is clicked on StartCanvas
    GameStart, // Right after "Start Game" is clicked on StartCanvas,
               // no time is spent here, just used for event notifier
    GamePlay, // During waves
    GameUpgrade, // During upgrade screen
    GameOver // On game over screen
}

public class CustomSceneManager : MonoBehaviour
{
    // Static reference to the instance of our SceneManager
    public static CustomSceneManager instance;
    public static Action<GAMESTATE> gameStateChange;
    public GAMESTATE curState;
    private Vector2 minBounds = new Vector2(0f, 0f); 
    private Vector2 maxBounds = new Vector2(0f, 0f);  
    public GameObject uiPrefab;
    public int killLimit;
    public int totalKills;
    public int maxWorkerCount;
    public int curWorkerCount;

    private TMP_Text mEnemyCountUI;
    private static int GAME_SCREEN_INDEX = 1;
    private List<GameObject> nonDestoryObjects;
    public static bool IsInitialized { get; private set;}
    public GameObject gameOverUI;
    public WaveManager waveManager;
    private int lastCheckedKills = 0;
    public GameObject UpgradeUI;
    public GameObject StartUI;
    public GameObject WarningUI;
    public GameObject MoneyPopUpUI;
    public bool heroUpgrade = true;
    private void Awake()
    {
        // Check if instance already exists
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            // If instance already exists and it's not this, then destroy this to enforce the singleton.
            Destroy(gameObject);
        }
        
        // Set this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateGameState(GAMESTATE gs)
    {
        Debug.Log("changing gameState to: " + gs);
        //Debug.Log(whichIsMe);
        switch(gs)
        {
            case GAMESTATE.BeforeGameStart:
                instance.StartingGame();
                break;
            case GAMESTATE.GameStart:
                break;
            case GAMESTATE.GamePlay:
                break;
            case GAMESTATE.GameUpgrade:
                break;
            case GAMESTATE.GameOver:
                break;
            default:
                break;
        }
        instance.curState = gs;
        gameStateChange?.Invoke(gs);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnemyAbstract.enemyKill += AddKill;
    }

    private void OnDisable()
    {   
        SceneManager.sceneLoaded -= OnSceneLoaded; 
        EnemyAbstract.enemyKill -= AddKill;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("On scene loaded");
        FindUIObjects();
        UpgradeUI.SetActive(false);
        gameOverUI.SetActive(false);
        WarningUI.SetActive(false);
        RemoveMoneyPopUp();
        minBounds = GameObject.Find("MinBounds").transform.position;
        maxBounds = GameObject.Find("MaxBounds").transform.position;
    }


    void Start() 
    {
        waveManager = FindObjectOfType<WaveManager>();
        instance.UpdateGameState(GAMESTATE.BeforeGameStart);
    }

    // Called Start Game Button
    public void StartGame(){
        StartUI.SetActive(false);
        FindUIObjects();
        StartingGame();
        waveManager.LoadStartingWave();
        UpdateGameState(GAMESTATE.GameStart);
        Time.timeScale = 1;
        UpdateGameState(GAMESTATE.GamePlay);
    }


    private void StartingGame(){
        Debug.Log("starting");
        
        totalKills = 0;
        maxWorkerCount = 5;
        curWorkerCount = 0;

        nonDestoryObjects = new List<GameObject>();

        if(gameOverUI == null)
        {
            Debug.Log("no ui found");
        } 
        else
        {
            Debug.Log(gameOverUI.name);
        }
        if(UpgradeUI != null && UpgradeUI.activeSelf)
        {
            UpgradeUI.SetActive(false);
        }
         if(gameOverUI != null && gameOverUI.activeSelf)
        {
            gameOverUI.SetActive(false);
        }

        if(WarningUI != null && WarningUI.activeSelf)
        {
            WarningUI.SetActive(false);
        }

        gameOverUI.SetActive(false);
        WarningUI.SetActive(false);
        RemoveMoneyPopUp();
        IsInitialized = true;
    }

    private void Update()
    {
        if (IsGameStarted() == false){
            return;
        }
        killLimit = WaveManager.Instance.GetKillsCount();
        // Check if the user is on a non-main scene and presses the Escape key
        if (SceneManager.GetActiveScene().buildIndex != 0 && Input.GetKeyDown(KeyCode.Escape))
        {
            // Load the main scene (assuming the main scene is at build index 0)
            Restart();
        }

           
        if(mEnemyCountUI != null)
        {
            int remainingEnemies = killLimit - totalKills;
            if(killLimit == 0){
                remainingEnemies = 0;
            }
            mEnemyCountUI.text = "Enemies you need to kill: " + remainingEnemies.ToString();
        }
        if ((WaveManager.Instance != null) && (waveManager.KillperWave != lastCheckedKills))
        {
            lastCheckedKills = waveManager.KillperWave;
            waveManager.CheckWaveEnd();
        }
        // Debug Press Space and do something
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameOverUI.SetActive(true);            
        }
    
    }

    public void AddWorker()
    {
        curWorkerCount++;
    }

    public bool CanAddWorker()
    {
        return curWorkerCount < maxWorkerCount;
    }

    public void AddKill()
    {
        totalKills++;
        waveManager.KillperWave++;
        if (totalKills >= killLimit){
            UpgradeUI.SetActive(true);
            PauseGame();
            UpdateGameState(GAMESTATE.GameUpgrade);
        }
    }

    // Called when you click on an Upgrade
    public void ResetAndLoad(int killLimit)
    {
        totalKills = 0;
        this.killLimit = killLimit;
        UpgradeUI.SetActive(false);
        UpdateGameState(GAMESTATE.GamePlay);
        Resume();
    }

    public void GameOver(){
        Debug.Log("entering gameover");
        Time.timeScale = 0; // Pause the game
        gameOverUI.SetActive(true); // Show the Game Over UI
        UpdateGameState(GAMESTATE.GameOver);
    }
    private void PauseGame(){
        Time.timeScale = 0;
    }

     private void Resume(){
        Time.timeScale = 1;
    }

    // called when you click "Play Again" on game over screen
    public void Restart() {
        foreach(GameObject go in nonDestoryObjects){
            if(go != null) Destroy(go);
        }
        gameOverUI.SetActive(false);
        TowerManager.Instance.Reset();
        SceneManager.LoadScene("MainScene");
        ShowStartCanvas();
        curWorkerCount = 0;
        UpdateGameState(GAMESTATE.BeforeGameStart);
    }

    public void AddNonDestoryObject(GameObject o) {
        nonDestoryObjects.Add(o);
    }

    // General method to load scenes based on build index
    private void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ShowStartCanvas(){
        StartUI.SetActive(true);
    }

    public bool IsGameStarted()
    {
        return curState == GAMESTATE.GamePlay || curState == GAMESTATE.GameUpgrade;
    }

    public void DisplayWarning()
    {
        WarningUI.SetActive(true);
        TimerManager.StartTimer(1f,  RemoveWarning, true);
    }

     public void RemoveWarning(){
        WarningUI.SetActive(false);
        
    }

    public void MoneyPopUp(int money)
    {
        MoneyPopUpUI.SetActive(true);
        TimerManager.StartTimer(2f,  RemoveMoneyPopUp, true);
    }

    public void RemoveMoneyPopUp(){
        MoneyPopUpUI.SetActive(false);
    }

    private void FindUIObjects()
    {
        if(gameOverUI == null)
        {
            gameOverUI = GameObject.FindGameObjectWithTag("GameOverUI");
        }
        if(UpgradeUI == null)
        {
            UpgradeUI = GameObject.FindGameObjectWithTag("UpgradeUI");

        }
        if(StartUI == null)
        {
            StartUI = GameObject.FindGameObjectWithTag("StartUI");
        }
        
        if(WarningUI == null)
        {
            WarningUI = GameObject.FindGameObjectWithTag("Warning");
        }
        
        if(MoneyPopUpUI == null)
        {
           MoneyPopUpUI = GameObject.FindGameObjectWithTag("MoneyPopUp");
        }
        
        if(mEnemyCountUI == null)
        {
           mEnemyCountUI =  GameObject.Find("EnemiesTextUI")?.GetComponent<TMP_Text>();
        }

    }

    public Vector2 GetMinBounds()
    {
        return minBounds;
    }

    public Vector2 GetMaxBounds()
    {
        return maxBounds;
    }
}
