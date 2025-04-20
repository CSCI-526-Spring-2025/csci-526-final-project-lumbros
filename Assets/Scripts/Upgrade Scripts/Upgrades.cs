using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Upgrades : MonoBehaviour
{
    public TextMeshProUGUI[] upgradeTexts;
    public TextMeshProUGUI[] costsTexts;
    public Button[] upgradeButtons;
    private GameObject hero;
    private GameObject manager;
    private GameObject upgradeManager;
    private GameObject[] towers;
    private GameObject[] workers;
    public GameObject UpgradeUI;
    public TMP_Text Wave;
    public TMP_Text UpgradeText;
    private int currWave = 0;
    private int phase = 1;
    private System.Action selectedUpgrade;
    private string selectedUpgradeName;
    private int selectedUpgradeCost;
    public int towerHP = 0;
    public int workerHP = 0;
    public int towerAutoHeal = 0;
    public int workerAutoHeal = 0;
    public int towerDamage = 0;
    public int towerRange = 0;
    public bool changed = false;
    public static System.Action<string> OnUpgrade;
    public int moneyBefore = 300;
    
    
    private Dictionary<string, List<(string, System.Action, int, float)>> heroUpgrades = new();
    private Dictionary<string, List<(string, System.Action, int, float)>> towerUpgrades = new();

    private HashSet<string> oneTimeUpgrades = new() { "Hero Reborn" };
    private HashSet<string> chosenOneTimeUpgrades = new();
    private float upgradeBoostMultiplier = 1f;

    void Start()
    {
        hero = GameObject.FindGameObjectWithTag("Player");
        manager = GameObject.FindGameObjectWithTag("Manager");
        towers = GameObject.FindGameObjectsWithTag("Tower");
        workers = GameObject.FindGameObjectsWithTag("Worker");
        
        float factorTower = DifficultyManager.GetTowerGoldCostMultiplier();
        float factorHero = DifficultyManager.GetHeroGoldCostMultiplier();
        AddUpgrade(heroUpgrades, "Hero Damage", "+2 to Hero Damage", () => hero.GetComponent<AutoAttack>().damage += 2, Mathf.CeilToInt(3 * factorHero), 1f);
        AddUpgrade(heroUpgrades, "Hero Damage", "+4 to Hero Damage", () => hero.GetComponent<AutoAttack>().damage += 4, Mathf.CeilToInt(4 * factorHero), 1f);

        AddUpgrade(heroUpgrades, "Attack Speed", "Increase Hero Attack Speed (0.7x)", () => hero.GetComponent<AutoAttack>().attackCooldown *= 0.7f, Mathf.CeilToInt(4 * factorHero), 1f);
        AddUpgrade(heroUpgrades, "Attack Speed", "Increase Hero Attack Speed (0.5x)", () => hero.GetComponent<AutoAttack>().attackCooldown *= 0.4f, Mathf.CeilToInt(8 * factorHero), 1f);

        AddUpgrade(heroUpgrades, "Hero HP", "+5 to Hero HP", () => {
            Health hp = hero.GetComponent<Health>();
            hp.maxHealth += 5;
            hp.currentHealth += 5;
        }, Mathf.CeilToInt(2 * factorHero), 1f);
        AddUpgrade(heroUpgrades, "Hero HP", "+10 to Hero HP", () => {
            Health hp = hero.GetComponent<Health>();
            hp.maxHealth += 10;
            hp.currentHealth += 10;
        }, Mathf.CeilToInt(4 * factorHero), 1f);

        AddUpgrade(heroUpgrades, "Hero Auto Heal", "+2 to Hero Auto Heal", () => hero.GetComponent<Health>().autoHeal += 2, Mathf.CeilToInt(2 * factorHero), 1f);
        AddUpgrade(heroUpgrades, "Hero Auto Heal", "+4 to Hero Auto Heal", () => hero.GetComponent<Health>().autoHeal += 4, Mathf.CeilToInt(4 * factorHero), 1f);

        AddUpgrade(heroUpgrades, "Hero Move Speed", "Increase Move Speed +1", () => hero.GetComponent<HeroMovement>().moveSpeed += 1, Mathf.CeilToInt(3 * factorHero), 0.5f);
        AddUpgrade(heroUpgrades, "Hero Move Speed", "Increase Move Speed +2", () => hero.GetComponent<HeroMovement>().moveSpeed += 2, Mathf.CeilToInt(4 * factorHero), 0.5f);

        AddUpgrade(heroUpgrades, "Hero Bounce", "Hero Projectiles Bounce +2", () => hero.GetComponent<AutoAttack>().heroBounces += 2, Mathf.CeilToInt(5 * factorHero), 1f);
        AddUpgrade(heroUpgrades, "Hero Bounce", "Hero Projectiles Bounce +4", () => hero.GetComponent<AutoAttack>().heroBounces += 4, Mathf.CeilToInt(10 * factorHero), 1f);

        AddUpgrade(heroUpgrades, "Hero Reborn", "The hero can respawn within 10 seconds", () => hero.GetComponent<Health>().heroReborn = true, Mathf.CeilToInt(10 * factorHero), 0.5f);

        AddUpgrade(towerUpgrades, "Tower Range", "+2 to Tower Attack Range", () => {
            foreach (var tower in towers) tower.GetComponent<AutoAttack>().attackRange += 2;
            UpgradeManager.Instance.towerRange += 2;
        }, Mathf.CeilToInt(5 * factorTower), 0.7f);

        AddUpgrade(towerUpgrades, "Tower Range", "+3 to Tower Attack Range", () => {
            foreach (var tower in towers) tower.GetComponent<AutoAttack>().attackRange += 3;
            UpgradeManager.Instance.towerRange += 3;
        }, Mathf.CeilToInt(7 * factorTower), 0.7f);

        AddUpgrade(towerUpgrades, "Tower Auto Heal", "+1 to Tower Auto Heal", () => {
            foreach (var tower in towers) tower.GetComponent<Health>().autoHeal += 1;
            UpgradeManager.Instance.towerAutoHeal += 1;
        }, Mathf.CeilToInt(6 * factorTower), 1f);

        AddUpgrade(towerUpgrades, "Tower Auto Heal", "+2 to Tower Auto Heal", () => {
            foreach (var tower in towers) tower.GetComponent<Health>().autoHeal += 2;
            UpgradeManager.Instance.towerAutoHeal += 2;
        }, Mathf.CeilToInt(9 * factorTower), 1f);

        AddUpgrade(towerUpgrades, "Tower Damage", "+1 to Tower Damage", () => {
            foreach (var tower in towers) tower.GetComponent<AutoAttack>().damage += 1;
            UpgradeManager.Instance.towerDamage += 1;
        }, Mathf.CeilToInt(4 * factorTower), 1f);

        AddUpgrade(towerUpgrades, "Tower Damage", "+2 to Tower Damage", () => {
            foreach (var tower in towers) tower.GetComponent<AutoAttack>().damage += 2;
            UpgradeManager.Instance.towerDamage += 2;
        }, Mathf.CeilToInt(6 * factorTower), 1f);

        AddUpgrade(towerUpgrades, "Tower HP", "+2 to Tower HP", () => {
            foreach (var tower in towers) {
                var hp = tower.GetComponent<Health>();
                hp.maxHealth += 2;
                hp.currentHealth += 2;
            }
            UpgradeManager.Instance.towerHP += 2;
        }, Mathf.CeilToInt(4 * factorTower), 1f);

        AddUpgrade(towerUpgrades, "Tower HP", "+5 to Tower HP", () => {
            foreach (var tower in towers) {
                var hp = tower.GetComponent<Health>();
                hp.maxHealth += 5;
                hp.currentHealth += 5;
            }
            UpgradeManager.Instance.towerHP += 5;
        }, Mathf.CeilToInt(7 * factorTower), 1f);

        AddUpgrade(towerUpgrades, "Worker Auto Heal", "+1 to Worker Auto Heal", () => {
            foreach (var worker in workers){
                if( worker.GetComponent<Health>() != null){
                    worker.GetComponent<Health>().autoHeal += 1;
                }
            }
        }, Mathf.CeilToInt(3 * factorTower), 0.3f);

        AddUpgrade(towerUpgrades, "Worker Auto Heal", "+2 to Worker Auto Heal", () => {
            foreach (var worker in workers) {
                if( worker.GetComponent<Health>() != null){
                    worker.GetComponent<Health>().autoHeal += 2;
                }
            }
            UpgradeManager.Instance.workerAutoHeal += 2;
        }, Mathf.CeilToInt(5 * factorTower), 0.1f);

        AddUpgrade(towerUpgrades, "Worker HP", "+2 to Worker HP", () => {
            foreach (var worker in workers) {
                if(worker.GetComponent<Health>() != null){
                    var hp = worker.GetComponent<Health>();
                    hp.maxHealth += 2;
                    hp.currentHealth += 2;
                }
            }
            UpgradeManager.Instance.workerHP += 2;
        }, Mathf.CeilToInt(3 * factorTower), 0.3f);

        AddUpgrade(towerUpgrades, "Worker HP", "+4 to Worker HP", () => {
            foreach (var worker in workers) {
                if(worker.GetComponent<Health>() != null){
                    var hp = worker.GetComponent<Health>();
                    hp.maxHealth += 4;
                    hp.currentHealth += 4;
                }
            }
            UpgradeManager.Instance.workerHP += 4;
        }, Mathf.CeilToInt(6 * factorTower), 0.1f);


        AssignUpgrades(heroUpgrades);
    }

    void AddUpgrade(Dictionary<string, List<(string, System.Action, int, float)>> upgradeList, string category, string description, System.Action action, int cost, float weight)
    {
        if (!upgradeList.ContainsKey(category))
            upgradeList[category] = new();

        upgradeList[category].Add((description, action, cost, weight));
    }

    void AssignUpgrades(Dictionary<string, List<(string, System.Action, int, float)>> upgradeList)
    {
        List<string> availableCategories = new(upgradeList.Keys);
        List<(string, System.Action, int, float)> weightedPool = new();

        foreach (var category in availableCategories)
        {
            if (oneTimeUpgrades.Contains(category) && chosenOneTimeUpgrades.Contains(category))
                continue;

            foreach (var upg in upgradeList[category])
            {
                int copies = Mathf.RoundToInt(upg.Item4 * upgradeBoostMultiplier * 10);
                for (int i = 0; i < copies; i++)
                    weightedPool.Add(upg);
            }
        }

        List<(string, System.Action, int, float)> selectedUpgrades = new();
        while (selectedUpgrades.Count < 3 && weightedPool.Count > 0)
        {
            int idx = Random.Range(0, weightedPool.Count);
            selectedUpgrades.Add(weightedPool[idx]);
            weightedPool.RemoveAll(u => u.Item1 == weightedPool[idx].Item1);
        }

        for (int i = 0; i < selectedUpgrades.Count; i++)
        {
            int index = i;
            var upg = selectedUpgrades[i];

            upgradeTexts[i].text = upg.Item1;
            costsTexts[i].text = "" + upg.Item3;
            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() => {
                if (MoneyManager.Instance.mMoney >= upg.Item3)
                {
                    MoneyManager.Instance.mMoney -= upg.Item3;
                    selectedUpgrade = upg.Item2;
                    selectedUpgradeName = upg.Item1;
                    selectedUpgradeCost = upg.Item3;

                    foreach (var kvp in upgradeList)
                        if (kvp.Value.Exists(u => u.Item1 == upg.Item1) && oneTimeUpgrades.Contains(kvp.Key))
                            chosenOneTimeUpgrades.Add(kvp.Key);

                    ApplyUpgrade();
                }
                else CustomSceneManager.instance.DisplayWarning();
            });

            upgradeButtons[i].interactable = MoneyManager.Instance.mMoney >= upg.Item3;
        }
    }

    public void ApplyUpgrade()
    {
        selectedUpgrade?.Invoke();
        OnUpgrade?.Invoke(selectedUpgradeName);

        AssignUpgrades(phase == 1 ? towerUpgrades : heroUpgrades);



        if (phase == 1)
        {
            phase = 2;
            UpgradeUI.SetActive(false);
            UpgradeUI.SetActive(true);
        }
        else
        {
            phase = 1;
            LoadMainScene();
        }
    }

    public void cancel()
    {
        if (phase == 1)
        {
            phase = 2;
            UpgradeUI.SetActive(false);
            UpgradeUI.SetActive(true);
            AssignUpgrades(towerUpgrades);
        }
        else
        {
            phase = 1;
            LoadMainScene();
            AssignUpgrades(heroUpgrades);
        }
    }

    void LoadMainScene()
    {
        int maxKill = manager.GetComponent<CustomSceneManager>().killLimit;
        if (CustomSceneManager.instance != null)
        {
            manager.GetComponent<CustomSceneManager>().ResetAndLoad(maxKill + 3);
        }
    }

    public void BoostGoodUpgradeChance(float multiplier)
    {
        upgradeBoostMultiplier = multiplier;
    }

    public void reset()
    {
        towerHP = 0;
        towerAutoHeal = 0;
        towerDamage = 0;
        towerRange = 0;
    }

    void Update()
    {
        towers = GameObject.FindGameObjectsWithTag("Tower");
        workers = GameObject.FindGameObjectsWithTag("Worker");
        // Debug.Log("worker debug: " + workers[0]);
        currWave = WaveManager.Instance.currentWave;


        if (currWave == 2) reset();
        Wave.text = "Wave " + (currWave - 1) + " Completed!";
        UpgradeText.text = (phase == 1) ? "Choose an upgrade for hero" : "Choose an upgrade for all towers";

        if (MoneyManager.Instance.mMoney != moneyBefore)
        {
            moneyBefore = MoneyManager.Instance.mMoney;
            changed = true;
        }

        if (phase == 1 && changed) { AssignUpgrades(heroUpgrades); changed = false; }
        if (phase == 2 && changed) { AssignUpgrades(towerUpgrades); changed = false; }
    }
}
