using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Vector2 minBounds = new Vector2(0f, 0f);
    private Vector2 maxBounds = new Vector2(0f, 0f);

    private bool waitingForNextButton = false;

    public GameObject gameOverUI;
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
    public GameObject HeroDescriptionButton;
    public GameObject HeroInfoUI;
    public GameObject bossHealthBar;
    public GameObject GameModePanel;
    public TMP_Text mEnemyCountUI;
    private TMP_Text coreHealthUI;
    public Button PauseButton;
    public GameObject pausedTextObj;
    private Sprite PauseSprite;
    private Sprite ResumeSprite;
    public Button TutorialNextButton;
    private GameObject pausedText;

    private void Awake()
    {
        // Check if instance already exists
        if (Instance == null)
        {
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

    // Start is called before the first frame update
    void Start()
    {
        CustomSceneManager.gameStateChange += OnGameStateChange;
        FindUIObjects();
    }

    // Update is called once per frame
    void Update()
    {
        FindUIObjects();
    }

    void OnGameStateChange(GAMESTATE newState)
    {
        switch (newState)
        {
            case GAMESTATE.BeforeGameStart:
                ShowStartCanvas();
                break;
            default:
                StartUI.SetActive(false);
                break;
        }
    }

    public void ShowStartCanvas()
    {
        StartUI.SetActive(true);
    }

    public void DisplayWarning()
    {
        WarningUI.SetActive(true);
        TimerManager.StartTimer(3f, RemoveWarning, true);
    }

    public void RemoveWarning()
    {
        WarningUI.SetActive(false);
    }

    public void MoneyPopUp(int money)
    {
        MoneyPopUpUI.SetActive(false);
        TimerManager.StartTimer(0.15f, ShowMoneyPopUp, true);
        TimerManager.StartTimer(3f, RemoveMoneyPopUp, true);
    }

    public void ShowMoneyPopUp()
    {
        MoneyPopUpUI.SetActive(true);
    }

    public void RemoveMoneyPopUp()
    {
        MoneyPopUpUI.SetActive(false);
    }

    private void FindUIObjects()
    {
        if (gameOverUI == null)
        {
            gameOverUI = GameObject.Find("GameOverCanvas");
        }
        if (UpgradeUI == null)
        {
            UpgradeUI = GameObject.FindGameObjectWithTag("UpgradeUI");

        }
        if (StartUI == null)
        {
            StartUI = GameObject.FindGameObjectWithTag("StartUI");
        }

        if (WarningUI == null)
        {
            WarningUI = GameObject.FindGameObjectWithTag("Warning");
        }

        if (MoneyPopUpUI == null)
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
            mEnemyCountUI = GameObject.Find("EnemiesTextUI")?.GetComponent<TMP_Text>();
        }

        if (TowerInfoUI == null)
        {
            TowerInfoUI = GameObject.Find("TowerDetailsPopup");
        }

        if (coreHealthUI == null)
        {
            coreHealthUI = GameObject.Find("CoreTextUI")?.GetComponent<TMP_Text>();
        }

        if (FinishedGameUI == null)
        {
            FinishedGameUI = DifficultyManager.Instance.GetFinishedGameUI();
            // FinishedGameUI =  GameObject.Find("FinishedGameUI");
        }

        if (PauseButton == null)
        {
            GameObject button = GameObject.Find("PauseButton");

            if (button != null)
            {
                PauseButton = button.GetComponent<Button>();
            }
        }

        if (PauseSprite == null)
        {
            PauseSprite = Resources.Load<Sprite>("Sprites/Pause Button");
        }

        if (ResumeSprite == null)
        {
            ResumeSprite = Resources.Load<Sprite>("Sprites/Resume Button");
        }
        if (TutorialNextButton == null && TutorialUI != null)
        {
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

        if (HeroDescriptionButton == null)
        {
            HeroDescriptionButton = GameObject.Find("HeroInfoButton");
        }

        if (HeroInfoUI == null)
        {
            HeroInfoUI = GameObject.Find("HeroInfoUI");
        }

        if (pausedText == null)
        {
            pausedText = GameObject.Find("PausedText");
        }

        if (bossHealthBar == null)
        {
            bossHealthBar = GameObject.Find("BossHealthBar");
        }

        if (GameModePanel == null)
        {
            GameModePanel = GameObject.Find("GameModePanel");
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

    public void OnTutorialNextButtonClicked()
    {
        waitingForNextButton = false;
    }
}
