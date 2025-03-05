using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

public enum GAMESTATE{
    GameStart,
    GamePlay,
    GameUpgrade,
    GameOver
}

public class CustomSceneManager : MonoBehaviour
{
    // Static reference to the instance of our SceneManager
    public static CustomSceneManager instance;
    public static Action<GAMESTATE> gameStateChange;
    public GAMESTATE curState;
    public string whichIsMe = "please change me";

    public GameObject uiPrefab;
    public int killLimit;
    public int totalKills;

    public int maxTowerCount;
    public int curTowerCount;
    public int maxWorkerCount;
    public int curWorkerCount;

    public TMP_Text mTowerCountUI;
    public TMP_Text mEnemyCountUI;
    public bool shouldSpawnMine = true;

    private static int GAME_SCREEN_INDEX = 1;
    // private static int UPGRADE_SCREEN_INDEX = 2;
    private List<GameObject> nonDestoryObjects;
    public static bool IsInitialized { get; private set;}
    public GameObject gameOverUI;
    public WaveManager waveManager;
    private int lastCheckedKills = 0;
    public GameObject UpgradeUI;
    public GameObject StartUI;
    public GameObject WarningUI;
    public GameObject MoneyPopUpUI;

    private bool isGameStarted = false;
    private void Awake()
    {
        // Check if instance already exists
        if (instance == null)
        {
            // If not, set instance to this
            whichIsMe = "i got change";
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
        Debug.Log(whichIsMe);
        switch(gs)
        {
            case GAMESTATE.GameStart:
                instance.StartingGame();
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
        EnemyAbstract.enemyKill += AddKill;
    }

    private void OnDisable()
    {
        EnemyAbstract.enemyKill -= AddKill;
    }

    void Start() {
        gameOverUI = GameObject.FindGameObjectWithTag("GameOverUI");
        UpgradeUI = GameObject.FindGameObjectWithTag("UpgradeUI");
        StartUI = GameObject.FindGameObjectWithTag("StartUI");
        WarningUI = GameObject.FindGameObjectWithTag("Warning");
        MoneyPopUpUI = GameObject.FindGameObjectWithTag("MoneyPopUp");
        waveManager = FindObjectOfType<WaveManager>();
        instance.UpdateGameState(GAMESTATE.GameStart);
    }

    private void StartingGame(){
        Debug.Log("starting");
        
        totalKills = 0;
        maxWorkerCount = 5;
        curWorkerCount = 0;

        nonDestoryObjects = new List<GameObject>();

        //gameOverUI = Instantiate(uiPrefab);
        if(gameOverUI == null)
        {
            Debug.Log("no ui found");
        } 
        else
        {
            Debug.Log(gameOverUI.name);
        }
        UpgradeUI.SetActive(false);
        gameOverUI.SetActive(false);
        WarningUI.SetActive(false);
        RemoveMoneyPopUp();
        IsInitialized = true;
    }

    private void Update()
    {
        if (isGameStarted == false){
            return;
        }
        killLimit = WaveManager.Instance.GetKillsCount();
        // Check if the u
        // ser is on a non-main scene and presses the Escape key
        if (SceneManager.GetActiveScene().buildIndex != 0 && Input.GetKeyDown(KeyCode.Escape))
        {
            // Load the main scene (assuming the main scene is at build index 0)
            Restart();
        }
        // if(mTowerCountUI != null)
        // {
        //     mTowerCountUI.text = "Towers: " + curTowerCount.ToString() + "/" + maxTowerCount.ToString();
        // }
           
        if(mEnemyCountUI != null)
        {
            int remainingEnemies = killLimit - totalKills;
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
            // UpgradeUI.SetActive(true);            
        }
    
    }

    public void IncreaseTowerCount()
    {
        maxTowerCount++;
    }

    public void AddTower()
    {
        curTowerCount++;
    }

    public void DestoryTower()
    {
        curTowerCount--;
    }

    public bool CanAddTower()
    {
        // return curTowerCount < maxTowerCount;
        return true;
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
            //SceneManager.LoadScene("UpgradeScene");
        }
    }

    // Called when you click on an Upgrade
    public void ResetAndLoad(int killLimit)
    {
        totalKills = 0;
        this.killLimit = killLimit;
        UpgradeUI.SetActive(false);
        Resume();
        // LoadScene(GAME_SCREEN_INDEX);
    }

    public void GameOver(){
        Debug.Log("entering gameover");
        Debug.Log(whichIsMe);
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
    public void Restart() {
        isGameStarted = false;
        foreach(GameObject go in nonDestoryObjects){
            if(go != null) Destroy(go);
        }
        shouldSpawnMine = true;
        curWorkerCount = 0;
        gameOverUI.SetActive(false);
        //TowerManager.instance.Reset();
        Time.timeScale = 1; // start the game again
        SceneManager.LoadScene("MainScene");
        ShowStartCanvas();
        Debug.Log(whichIsMe);
        UpdateGameState(GAMESTATE.GameStart);
    }

    public void AddNonDestoryObject(GameObject o) {
        nonDestoryObjects.Add(o);
    }

    // General method to load scenes based on build index
    private void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void StartGame(){
        StartUI.SetActive(false);
        MineSpawner.Instance.StartMineSpawner();
        WorkerSpawner.Instance.StartWorkerSpawner();
        isGameStarted = true;
        Debug.Log(whichIsMe);
        UpdateGameState(GAMESTATE.GamePlay);
    }

    public void ShowStartCanvas(){
        StartUI.SetActive(true);
    }

    public bool GetGameStatus(){
        return isGameStarted;
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
        TextMeshProUGUI tmpText = MoneyPopUpUI.GetComponent<TextMeshProUGUI>();
        if(tmpText != null)
            tmpText.text = "$" + money;
        MoneyPopUpUI.SetActive(true);
        TimerManager.StartTimer(2f,  RemoveMoneyPopUp, true);
    }

    public void RemoveMoneyPopUp(){
        MoneyPopUpUI.SetActive(false);
        
    }
}
