using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.UI;
public enum GAMESTATE{
    BeforeGameStart, // Before "Start Game" is clicked on StartCanvas
    PlaceCore, // When Core is draggable,
    DummyState, // Dummy state for testing
    GameStart, // Right after "Start Game" is clicked on StartCanvas,
               // no time is spent here, just used for event notifier
    GamePlay, // During waves
    GameUpgrade, // During upgrade screen
    GameOver, // On game over screen
    GameSuccess,
    Tutorial,
    GameTutorialPauseAndReadTowers,
    GameTutorialHeroMoveAndAttack,
    GameTutorialUpgrades,
    GameTutorialEnd,
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
    public bool isPause = false;
    private TMP_Text mEnemyCountUI;

    private TMP_Text coreHealthUI;
    
    private static int GAME_SCREEN_INDEX = 1;
    private List<GameObject> nonDestoryObjects;
    public static bool IsInitialized { get; private set;}
    public GameObject gameOverUI;
    public WaveManager waveManager;
    private int lastCheckedKills = 0;
    public GameObject UpgradeUI;
    public GameObject StartUI;
    public GameObject WarningUI;
    public GameObject TowerInfoUI;
    public GameObject MoneyPopUpUI;
    public GameObject TutorialUI;
    public GameObject TowerBarUI;
    public GameObject FinishedGameUI;
    public GameObject Hero;
    public GameObject HeroDescription;
    public bool heroUpgrade = true;
    public int tutorialstep = 0;
    public bool isTutorialMode = false;
    public Button TutorialNextButton;
    private bool waitingForNextButton = false;
    private IEnumerator currentTutorialCoroutine = null;
    public Button PauseButton;
    private Sprite PauseSprite;
    private Sprite ResumeSprite;
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
        switch(gs)
        {
            case GAMESTATE.BeforeGameStart:
                instance.StartingGame();
                break;
            case GAMESTATE.PlaceCore:
                break;
            case GAMESTATE.GameStart:
                {
                    GameObject hero = GameObject.FindGameObjectWithTag("Hero");
                    if(hero != null) Destroy(hero);
                }
                Instantiate(instance.Hero);
                break;
            case GAMESTATE.GamePlay:
                break;
            case GAMESTATE.GameUpgrade:
                break;
            case GAMESTATE.GameOver:
                break;
            case GAMESTATE.GameSuccess:
                break;
            case GAMESTATE.Tutorial:
                break;
            case GAMESTATE.GameTutorialPauseAndReadTowers:
                instance.pause();
                // spawn text tell player the game is pause and they can take their time to read the towers on the right
                // whenever the player is ready, they can unpause to move on
                StartCoroutine(TutorialReadTowers());
                break;
            case GAMESTATE.GameTutorialHeroMoveAndAttack:
                // spawn text explaining the hero and how to move
                // then how to hero auto attack
                {
                    GameObject hero = GameObject.FindGameObjectWithTag("Hero");
                    if(hero != null) Destroy(hero);
                }
                Instantiate(instance.Hero);
                
                // 在英雄移动攻击教程阶段显示HeroDescription
                if (HeroDescription != null)
                {
                    HeroDescription.SetActive(true);
                }
                
                StartCoroutine(TutorialHeroMoveAndAttack());
                break;
            case GAMESTATE.GameTutorialUpgrades:
                // after killing the last enemey show upgrades and explain how upgrades work
                TutorialUpgrades();
                break;
            case GAMESTATE.GameTutorialEnd:
                TutorialEnd();
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
        WaveManager.waveEnd += OnWaveEnd;
        WaveManager.GameSuccess += HandleGameSuccess;
    }

    private void OnDisable()
    {   
        SceneManager.sceneLoaded -= OnSceneLoaded; 
        EnemyAbstract.enemyKill -= AddKill;
        WaveManager.waveEnd -= OnWaveEnd;
        WaveManager.GameSuccess -= HandleGameSuccess;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("On scene loaded");
        FindUIObjects();
        UpgradeUI.SetActive(false);
        gameOverUI.SetActive(false);
        WarningUI.SetActive(false);
        TutorialUI.SetActive(false);
        RemoveMoneyPopUp();
        minBounds = GameObject.Find("MinBounds").transform.position;
        maxBounds = GameObject.Find("MaxBounds").transform.position;
    }


    void Start() 
    {
        waveManager = FindObjectOfType<WaveManager>();
        instance.UpdateGameState(GAMESTATE.BeforeGameStart);
    }

    public void StartTutorial()
    {
        // Hide startUI
        StartUI.SetActive(false);
        FindUIObjects();
        StartingGame();

        // 确保HeroDescription在教程开始时隐藏
        if (HeroDescription != null)
        {
            HeroDescription.SetActive(false);
        }

        isTutorialMode = true;
        // 先进入PlaceCore状态，允许核心拖动
        UpdateGameState(GAMESTATE.PlaceCore);
        TowerBarUI.SetActive(false);
        if (TutorialUI == null)
        {
            TutorialUI = GameObject.FindGameObjectWithTag("TutorialUI");
        }

        if (TutorialUI != null)
        {
            TutorialUI.SetActive(true);
            TMP_Text tutorialText = TutorialUI.GetComponentInChildren<TMP_Text>();
            if (tutorialText != null)
            {
                tutorialText.text = "Welcome to the game tutorial! The goal of the game is to protect the core for 8 waves.";
            }
        }

        //StartCoroutine(TutorialSequence());
        currentTutorialCoroutine = TutorialSequence();
        StartCoroutine(currentTutorialCoroutine);
    }

    // Called Start Game Button
    public void StartGame(){
        StartUI.SetActive(false);
        FindUIObjects();
        StartingGame();
        waveManager.LoadStartingWave();
        if (TutorialUI != null)
        {
            TutorialUI.SetActive(false);
        }
        isTutorialMode = false;
        UpdateGameState(GAMESTATE.GameStart);
        Time.timeScale = 1;
        UpdateGameState(GAMESTATE.GamePlay);
        //UpdateGameState(GAMESTATE.GameTutorialPauseAndReadTowers);
    }

    private void DisablePlayerControls()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            HeroMovement movement = player.GetComponent<HeroMovement>();
            if (movement != null)
            {
                movement.enabled = false;
            }

            AutoAttack attack = player.GetComponent<AutoAttack>();
            if (attack != null)
            {
                attack.enabled = false;
            }
        }
    }

    private void EnablePlayerControls()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            HeroMovement movement = player.GetComponent<HeroMovement>();
            if (movement != null)
            {
                movement.enabled = true;
                Debug.Log("Player movement restored");
            }
            AutoAttack attack = player.GetComponent<AutoAttack>();
            if (attack != null)
            {
                attack.enabled = true;
                Debug.Log("Player attack restored");
            }
        }
    }
    private IEnumerator TutorialSequence()
    {
        GameObject core = GameObject.FindGameObjectWithTag("Core");

        SpawnTutorialMines();
        UpdateTutorialText("Welcome to Brotower!");
        yield return StartCoroutine(WaitForNextButton());
        UpdateTutorialText("These brown structures are mines. They generate resources.");
        yield return StartCoroutine(WaitForNextButton());

        UpdateTutorialText("The small characters are workers. They collect resources from mines.");
        yield return StartCoroutine(WaitForNextButton());

        UpdateTutorialText("Workers bring resources to your core to earn gold.");
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.UpdateMoney(30);
        }


       
        

        // yield return new WaitForSeconds(10f);
        yield return StartCoroutine(WaitForNextButton());
        UpdateTutorialText("Now, please drag the green core to a suitable position. The core is the base you need to protect.");
        
        float waitTime = 0;
        bool corePlaced = false;
        Vector3 initialCorePosition = core ? core.transform.position : Vector3.zero;

        while (!corePlaced && waitTime < 30f)
        {
            yield return new WaitForSeconds(0.5f);
            waitTime += 0.5f;

            if (core && Vector3.Distance(initialCorePosition, core.transform.position) > 0.5f)
            {
                Debug.Log("Core position change detected, considering it placed");
                yield return new WaitForSeconds(1f); 
                corePlaced = true;
            }
        }

        UpdateGameState(GAMESTATE.Tutorial);

        UpdateTutorialText("Beware! Enemies will attack your core. The first wave of enemies is coming.");
        // yield return new WaitForSeconds(5f);
        yield return StartCoroutine(WaitForNextButton());
        TowerBarUI.SetActive(true);
        SetTowersUI(false);
        UpdateTutorialText("Build defense towers to protect your core. You can drag the tower from the right bar and place them.");
        

        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner != null)
        {
            Debug.Log("Found EnemySpawner, attempting to generate enemies directly");
            try
            {
                int enemyCount = 1;
                int currentWave = 1;
                float spawnInterval = 1.0f;
                WaveManager.Instance.enemyHealthMultiplier = 1;
                enemySpawner.SpawnWave(enemyCount, currentWave, spawnInterval);
            }
            catch (Exception e)
            {
                Debug.LogError("Error calling SpawnWave: " + e.Message + "\n" + e.StackTrace);
            }
        }

        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.UpdateMoney(50);
        }

        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        UpdateGameState(GAMESTATE.GameTutorialPauseAndReadTowers);
    }

    private IEnumerator WaitForNextButton()
    {
        waitingForNextButton = true;
        
        // 确保Next按钮可见
        if (TutorialNextButton != null)
        {
            TutorialNextButton.gameObject.SetActive(true);
        }
        
        // 等待waitingForNextButton变为false（通过点击按钮）
        while (waitingForNextButton)
        {
            yield return null;
        }
    }
    
    // Next按钮点击处理方法
    public void OnTutorialNextButtonClicked()
    {
        waitingForNextButton = false;
    }
    private void SetTowersUI(bool b){
        //this is hardcode need to fix later.
        if(TowerBarUI != null){
            // hide/unhide wall and door
            for(int i = 4; i < 7; ++i){
                var gOUI = TowerBarUI.transform.GetChild(i).gameObject;
                Debug.Log(gOUI.transform.childCount);
                for(int j = 1; j < 3; ++j){
                    gOUI.transform.GetChild(j).gameObject.SetActive(b);
                }
            }

            // hide/unhide ice, speed, aoe tower
            for(int i = 7; i < 10; ++i){
                TowerBarUI.transform.GetChild(i).gameObject.SetActive(b);
            }
        }
    }

    private void UpdateTutorialText(string text)
    {
        if (TutorialUI != null)
        {
            TMP_Text tutorialText = TutorialUI.GetComponentInChildren<TMP_Text>();
            if (tutorialText != null)
            {
                tutorialText.text = text;
            }
        }
    }
    public void FinishTutorial()
    {
        if (TutorialUI != null)
        {
            TutorialUI.SetActive(false);
        }

        EnablePlayerControls();

        isTutorialMode = false;

        StartGame();
    }

    public void ExitTutorial()
    {
        if (TutorialUI != null)
        {
            TutorialUI.SetActive(false);
        }

        EnablePlayerControls();

        isTutorialMode = false;

        Restart();
    }

    private IEnumerator TutorialReadTowers(){
        SetTowersUI(true);
        UpdateTutorialText("The game is pause now. Please take your time to read the tower info on the right by hovering over them.");
        // yield return new WaitForSecondsRealtime(5);
        yield return StartCoroutine(WaitForNextButton());
        UpdateTutorialText("You can also click on tower to see their stats. Once done, click the 'Pause' Button to continue.");
    }

    private IEnumerator TutorialHeroMoveAndAttack(){
        if (TutorialUI != null)
        {
            TutorialUI.SetActive(true);
            TMP_Text tutorialText = TutorialUI.GetComponentInChildren<TMP_Text>();
            if (tutorialText != null)
            {
                tutorialText.text = "Your hero is the blue guy and you can move him with AWSD.";
                if (HeroDescription != null)
                {
                    HeroDescription.SetActive(true);
                }
                // yield return new WaitForSeconds(5);
                yield return StartCoroutine(WaitForNextButton());
                tutorialText.text = "Your hero will autoattack the closest enemy within its attack range.";
            }
        }
    }

    private void TutorialUpgrades(){
        if (TutorialUI != null)
        {
            TutorialUI.SetActive(true);
            TMP_Text tutorialText = TutorialUI.GetComponentInChildren<TMP_Text>();
            if (tutorialText != null)
            {
                tutorialText.text = "After clearing the wave, you can spend gold to upgrade your hero and tower.";
            }
        }
    }

    private void TutorialEnd(){
        if (TutorialUI != null)
        {
            TutorialUI.SetActive(true);
            TMP_Text tutorialText = TutorialUI.GetComponentInChildren<TMP_Text>();
            if (tutorialText != null)
            {
                tutorialText.text = "Tutorial is completed.\nYou can now continue to the game or return to the main menu.";
            }
        }
        // Show start game button
        Transform StartGameButton = TutorialUI.transform.Find("StartGameButton");
        if (StartGameButton != null)
        {
            StartGameButton.gameObject.SetActive(true);
        }

        // Show exit to menu button
        Transform ExitToMenuButton = TutorialUI.transform.Find("ExitToMenuButton");
        if (ExitToMenuButton != null)
        {
            ExitToMenuButton.gameObject.SetActive(true);
        }
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

        if(TowerInfoUI != null && TowerInfoUI.activeSelf){
            TowerInfoUI.SetActive(false);
        }

        if(FinishedGameUI != null && FinishedGameUI.activeSelf){
            FinishedGameUI.SetActive(false);
        }

       
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
            if(WaveManager.Instance.currentWave == 8){   
                mEnemyCountUI.text = "Kill the boss!";
            }
            else{
                mEnemyCountUI.text = "Enemies you need to kill: " + remainingEnemies.ToString();
            }
            
        }
        if(coreHealthUI != null)
        {
            GameObject core = GameObject.FindGameObjectWithTag("Core");
            Health health =  core.GetComponent<Health>();
            coreHealthUI.text = $"{health.currentHealth} / {health.maxHealth} \n";
        }
        // if ((WaveManager.Instance != null) && (waveManager.KillperWave != lastCheckedKills))
        // {
        //     lastCheckedKills = waveManager.KillperWave;
        //     waveManager.CheckWaveEnd();
        // }

        
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

        if (WaveManager.Instance != null)
        {
            waveManager.DelayedCheckWaveEnd();
        }
        // if (!isTutorialMode && totalKills >= killLimit){
        //     UpgradeUI.SetActive(true);
        //     PauseGame();
        //     UpdateGameState(GAMESTATE.GameUpgrade);
        // }
    }

    private void OnWaveEnd(int curwave)
    {
        Debug.Log("Wave cleared, entering upgrade state");
        if(instance.curState == GAMESTATE.GameTutorialHeroMoveAndAttack){
            UpgradeUI.SetActive(true);
            PauseGame();
            UpdateGameState(GAMESTATE.GameTutorialUpgrades);
        }
        else if (!isTutorialMode)
        {
            UpgradeUI.SetActive(true);
            PauseGame();
            UpdateGameState(GAMESTATE.GameUpgrade);
        }
    }

    private void HandleGameSuccess()
    {
        Debug.Log("CustomSceneManager receive success");

        if (!isTutorialMode)
        {
            UpdateGameState(GAMESTATE.GameSuccess);
            FinishedGameUI.SetActive(true); //UI part
            PauseGame();
        }
    }

    // Called when you click on an Upgrade
    public void ResetAndLoad(int killLimit)
    {
        totalKills = 0;
        this.killLimit = killLimit;
        UpgradeUI.SetActive(false);

        if(instance.curState == GAMESTATE.GameTutorialUpgrades){
            UpdateGameState(GAMESTATE.GameTutorialEnd);
        }
        else{
            UpdateGameState(GAMESTATE.GamePlay);
            Resume();
        }
    }

    public void GameOver(){
        Debug.Log("entering gameover");
        Time.timeScale = 0; // Pause the game
        gameOverUI.SetActive(true); // Show the Game Over UI
        UpdateGameState(GAMESTATE.GameOver);
    }
    private void PauseGame(){
        //change button pic 
        PauseButton.image.sprite = PauseSprite;
        Time.timeScale = 0;
    }

     private void Resume(){
        if(instance.curState == GAMESTATE.GameTutorialPauseAndReadTowers){
            UpdateGameState(GAMESTATE.GameTutorialHeroMoveAndAttack);
        }
            //change button pic 
        PauseButton.image.sprite = ResumeSprite;
        Time.timeScale = 1;
    }

    public void pause(){
        if(!isPause){
            PauseGame();
            isPause = true;
        }else{
            Resume();
            isPause = false;
        }
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
        TimerManager.StartTimer(3f,  RemoveWarning, true);
    }

     public void RemoveWarning(){
        WarningUI.SetActive(false);
        
    }

    public void MoneyPopUp(int money)
    {
        MoneyPopUpUI.SetActive(false);
        TimerManager.StartTimer(0.15f, ShowMoneyPopUp, true);
        TimerManager.StartTimer(3f,  RemoveMoneyPopUp, true);
    }

    public void ShowMoneyPopUp()
    {
        MoneyPopUpUI.SetActive(true);
    }

    public void RemoveMoneyPopUp(){
        MoneyPopUpUI.SetActive(false);
    }

    private void FindUIObjects()
    {
        if(gameOverUI == null)
        {
            gameOverUI = GameObject.Find("GameOverCanvas");
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

        if (TowerBarUI == null)
        {
            TowerBarUI = GameObject.FindGameObjectWithTag("TowerBarUI");
        }

        if (TutorialUI == null)
        {
            TutorialUI = GameObject.FindGameObjectWithTag("TutorialUI");
        }

        if (mEnemyCountUI == null)
        {
           mEnemyCountUI =  GameObject.Find("EnemiesTextUI")?.GetComponent<TMP_Text>();
        }

        if(TowerInfoUI == null){
            TowerInfoUI = GameObject.Find("TowerDetailsPopup");
        }

        if(coreHealthUI == null){
            coreHealthUI = GameObject.Find("CoreTextUI")?.GetComponent<TMP_Text>();
        }

        if(FinishedGameUI == null)
        {
            FinishedGameUI =  GameObject.Find("FinishedGameUI");
        }

        if(PauseButton == null){
            GameObject button = GameObject.Find("PauseButton");

            if (button != null)
            {
                PauseButton = button.GetComponent<Button>();
            }
        }

        if(PauseSprite == null){
            PauseSprite = Resources.Load<Sprite>("Sprites/Black Pause Button");
        }

        if(ResumeSprite == null){
            ResumeSprite = Resources.Load<Sprite>("Sprites/Black Resume Buttom");
        }
        if (TutorialNextButton == null && TutorialUI != null){
            Transform nextButton = TutorialUI.transform.Find("NextTutorialButton");
            if (nextButton != null)
            {
                TutorialNextButton = nextButton.GetComponent<Button>();
                
                if (TutorialNextButton != null)
                {
                    TutorialNextButton.onClick.AddListener(OnTutorialNextButtonClicked);
                }
            }
        }

        if (HeroDescription == null)
        {
            HeroDescription = GameObject.Find("HeroDescription");
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
    private void SpawnTutorialMines()
    {
        Debug.Log("Attempting to generate mines in tutorial");

        // Find MineSpawner
        MineSpawner mineSpawner = FindObjectOfType<MineSpawner>();
        if (mineSpawner != null && mineSpawner.minePrefab != null)
        {
            Debug.Log("Found MineSpawner, preparing to generate mines");

            try
            {
                // Get mine prefab
                GameObject minePrefab = mineSpawner.minePrefab;

                // Clear existing mines
                GameObject[] existingMines = GameObject.FindGameObjectsWithTag("Mine");
                foreach (GameObject mine in existingMines)
                {
                    Destroy(mine);
                }

                // Get grid slots
                GridManager gridManager = FindObjectOfType<GridManager>();
                if (gridManager != null)
                {
                    InventorySlot[] slots = gridManager.GetInventorySlots();
                    if (slots != null && slots.Length > 0)
                    {
                        Debug.Log($"Found {slots.Length} grid slots");

                        // Generate 3 mines in random grid slots
                        int minesCreated = 0;
                        List<int> usedSlotIndices = new List<int>(); // Track used slots

                        while (minesCreated < 3 && usedSlotIndices.Count < slots.Length)
                        {
                            // Select a random unused slot
                            int randomSlotIndex;
                            do
                            {
                                randomSlotIndex = UnityEngine.Random.Range(0, slots.Length);
                            } while (usedSlotIndices.Contains(randomSlotIndex));

                            InventorySlot slot = slots[randomSlotIndex];
                            usedSlotIndices.Add(randomSlotIndex);

                            if (slot.CanAddItem())
                            {
                                GameObject newMine = slot.AddNewItem(minePrefab);
                                Debug.Log($"Generated mine #{minesCreated + 1} in slot #{randomSlotIndex}");
                                minesCreated++;
                            }
                        }

                        if (minesCreated == 0)
                        {
                            Debug.LogWarning("Failed to generate any mines in grid, all slots may be occupied");
                        }
                    }
                    else
                    {
                        Debug.LogError("No grid slots found");
                    }
                }
                else
                {
                    Debug.LogError("GridManager not found");
                }

                // Generate workers
                SpawnTutorialWorkers();
            }
            catch (Exception e)
            {
                Debug.LogError("Error generating mines: " + e.Message + "\n" + e.StackTrace);
            }
        }
        else
        {
            Debug.LogError("MineSpawner or mine prefab not found");
        }
    }

    private void SpawnTutorialWorkers()
    {
        Debug.Log("Attempting to generate workers in tutorial");

        // Find WorkerSpawner
        WorkerSpawner workerSpawner = FindObjectOfType<WorkerSpawner>();
        if (workerSpawner != null && workerSpawner.workerPrefab != null)
        {
            try
            {
                // Get worker prefab
                GameObject workerPrefab = workerSpawner.workerPrefab;

                // Find core position
                GameObject core = GameObject.FindGameObjectWithTag("Core");
                if (core != null)
                {
                    // Generate 3 workers around the core
                    for (int i = 0; i < 3; i++)
                    {
                        // Generate at random position near core
                        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * 2f;
                        Vector3 spawnPosition = core.transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

                        GameObject worker = Instantiate(workerPrefab, spawnPosition, Quaternion.identity);
                        Debug.Log($"Generated worker #{i + 1} at position {spawnPosition}");

                        // Increase worker count
                        AddWorker();
                    }
                }
                else
                {
                    Debug.LogError("Core not found, cannot generate workers");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error generating workers: " + e.Message + "\n" + e.StackTrace);
            }
        }
        else
        {
            Debug.LogError("WorkerSpawner or worker prefab not found");
        }
    }

    public void ContinueGame(){
        if(FinishedGameUI)
            FinishedGameUI.SetActive(false);
        //continute game
    }
}
