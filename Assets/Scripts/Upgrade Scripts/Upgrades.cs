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
    private GameObject[] towers;
    public GameObject UpgradeUI;
    public TMP_Text Wave;
    public TMP_Text UpgradeText;
    private int currWave = 0;
    private int phase = 1;
    private System.Action selectedUpgrade;
    private string selectedUpgradeName;
    private int selectedUpgradeCost;
    public int towerHP = 0;
    public int towerAutoHeal = 0;
    public int towerDamage = 0;
    public int towerRange = 0;
    public bool changed = false;
    public static System.Action<string> OnUpgrade;
    public int moneyBefore = 300;

    private Dictionary<string, List<(string, System.Action, int)>> heroUpgrades = new Dictionary<string, List<(string, System.Action, int)>>();
    private Dictionary<string, List<(string, System.Action, int)>> towerUpgrades = new Dictionary<string, List<(string, System.Action, int)>>();

    private HashSet<string> oneTimeUpgrades = new HashSet<string> { "Hero Reborn" };
    private HashSet<string> chosenOneTimeUpgrades = new HashSet<string>();

    void Start()
    {
        hero = GameObject.FindGameObjectWithTag("Player");
        manager = GameObject.FindGameObjectWithTag("Manager");

        AddUpgrade(heroUpgrades, "Hero Damage", "+1 to Hero Damage", () => hero.GetComponent<AutoAttack>().damage += 1, 3);
        AddUpgrade(heroUpgrades, "Hero Damage", "+2 to Hero Damage", () => hero.GetComponent<AutoAttack>().damage += 2, 4);

        AddUpgrade(heroUpgrades, "Attack Speed", "Increase Hero Attack Speed (0.7x)", () => hero.GetComponent<AutoAttack>().attackCooldown *= 0.7f, 5);
        AddUpgrade(heroUpgrades, "Attack Speed", "Increase Hero Attack Speed (0.5x)", () => hero.GetComponent<AutoAttack>().attackCooldown *= 0.4f, 10);

        AddUpgrade(heroUpgrades, "Hero HP", "+2 to Hero HP", () => {
            Health hp = hero.GetComponent<Health>();
            hp.maxHealth += 2;
            hp.currentHealth += 2;
        }, 2);
        AddUpgrade(heroUpgrades, "Hero HP", "+4 to Hero HP", () => {
            Health hp = hero.GetComponent<Health>();
            hp.maxHealth += 4;
            hp.currentHealth += 4;
        }, 4);

        AddUpgrade(heroUpgrades, "Hero Auto Heal", "+1 to Hero Auto Heal", () => hero.GetComponent<Health>().autoHeal += 1, 2);
        AddUpgrade(heroUpgrades, "Hero Auto Heal", "+2 to Hero Auto Heal", () => hero.GetComponent<Health>().autoHeal += 2, 4);

        AddUpgrade(heroUpgrades, "Move Speed", "Increase Move Speed +1", () => hero.GetComponent<HeroMovement>().moveSpeed += 1, 3);
        AddUpgrade(heroUpgrades, "Move Speed", "Increase Move Speed +2", () => hero.GetComponent<HeroMovement>().moveSpeed += 2, 4);

        AddUpgrade(heroUpgrades, "Hero Bounce", "Hero Projectiles Bounce +1", () => hero.GetComponent<AutoAttack>().heroBounces += 1, 3);
        AddUpgrade(heroUpgrades, "Hero Bounce", "Hero Projectiles Bounce +2", () => hero.GetComponent<AutoAttack>().heroBounces += 2, 4);

        AddUpgrade(heroUpgrades, "Hero Reborn", "The hero can respawn within 10 seconds", () => hero.GetComponent<Health>().heroReborn = true, 10);

        AddUpgrade(towerUpgrades, "Tower Range", "+2 to Tower Attack Range", () => {
            foreach (var tower in towers) tower.GetComponent<AutoAttack>().attackRange += 2;
            towerRange += 2;
        }, 3);
        AddUpgrade(towerUpgrades, "Tower Range", "+3 to Tower Attack Range", () => {
            foreach (var tower in towers) tower.GetComponent<AutoAttack>().attackRange += 3;
            towerRange += 3;
        }, 4);

        AddUpgrade(towerUpgrades, "Tower Auto Heal", "+1 to Tower Auto Heal", () => {
            foreach (var tower in towers) tower.GetComponent<Health>().autoHeal += 1;
            towerAutoHeal += 1;
        }, 3);
        AddUpgrade(towerUpgrades, "Tower Auto Heal", "+2 to Tower Auto Heal", () => {
            foreach (var tower in towers) tower.GetComponent<Health>().autoHeal += 2;
            towerAutoHeal += 2;
        }, 5);

        AddUpgrade(towerUpgrades, "Tower Damage", "+1 to Tower Damage", () => {
            foreach (var tower in towers) tower.GetComponent<AutoAttack>().damage += 1;
            towerDamage += 1;
        }, 3);
        AddUpgrade(towerUpgrades, "Tower Damage", "+2 to Tower Damage", () => {
            foreach (var tower in towers) tower.GetComponent<AutoAttack>().damage += 2;
            towerDamage += 2;
        }, 5);

        AddUpgrade(towerUpgrades, "Tower HP", "+2 to Tower HP", () => {
            foreach (var tower in towers) {
                var hp = tower.GetComponent<Health>();
                hp.maxHealth += 2;
                hp.currentHealth += 2;
            }
            towerHP += 2;
        }, 3);
        AddUpgrade(towerUpgrades, "Tower HP", "+4 to Tower HP", () => {
            foreach (var tower in towers) {
                var hp = tower.GetComponent<Health>();
                hp.maxHealth += 4;
                hp.currentHealth += 4;
            }
            towerHP += 4;
        }, 4);

        AssignUpgrades(heroUpgrades);
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
        currWave = WaveManager.Instance.currentWave;

        if (currWave == 2)
        {
            reset();
        }

        Wave.text = "Wave " + (currWave - 1) + " Completed!";
        UpgradeText.text = (phase == 1) ? "Choose an upgrade for hero" : "Choose an upgrade for all towers";

        if (MoneyManager.Instance.mMoney != moneyBefore)
        {
            moneyBefore = MoneyManager.Instance.mMoney;
            changed = true;
        }

        if (phase == 1 && changed)
        {
            AssignUpgrades(heroUpgrades);
            changed = false;
        }

        if (phase == 2 && changed)
        {
            AssignUpgrades(towerUpgrades);
            changed = false;
        }
    }

    void AddUpgrade(Dictionary<string, List<(string, System.Action, int)>> upgradeList, string category, string description, System.Action upgradeAction, int cost)
    {
        if (!upgradeList.ContainsKey(category))
        {
            upgradeList[category] = new List<(string, System.Action, int)>();
        }
        upgradeList[category].Add((description, upgradeAction, cost));
    }

    void AssignUpgrades(Dictionary<string, List<(string, System.Action, int)>> upgradeList)
    {
        List<string> availableCategories = new List<string>(upgradeList.Keys);
        List<(string, System.Action, int)> selectedUpgrades = new List<(string, System.Action, int)>();

        while (selectedUpgrades.Count < 3 && availableCategories.Count > 0)
        {
            int categoryIndex = Random.Range(0, availableCategories.Count);
            string chosenCategory = availableCategories[categoryIndex];
            var upgradesInCategory = upgradeList[chosenCategory];

            var availableUpgrades = upgradesInCategory.FindAll(upg =>
                !oneTimeUpgrades.Contains(chosenCategory) || !chosenOneTimeUpgrades.Contains(chosenCategory)
            );

            if (availableUpgrades.Count == 0)
            {
                availableCategories.RemoveAt(categoryIndex);
                continue;
            }

            var selectedUpgrade = availableUpgrades[Random.Range(0, availableUpgrades.Count)];
            selectedUpgrades.Add(selectedUpgrade);
            availableCategories.RemoveAt(categoryIndex);
        }

        for (int i = 0; i < selectedUpgrades.Count; i++)
        {
            int index = i;
            int upgradeCost = selectedUpgrades[index].Item3;
            string formattedText = selectedUpgrades[index].Item1;

            upgradeTexts[i].text = formattedText;
            costsTexts[i].text = "$" + upgradeCost.ToString();

            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() =>
            {
                if (MoneyManager.Instance.mMoney >= upgradeCost)
                {
                    MoneyManager.Instance.mMoney -= upgradeCost;
                    selectedUpgrade = selectedUpgrades[index].Item2;
                    selectedUpgradeName = selectedUpgrades[index].Item1;
                    selectedUpgradeCost = upgradeCost;

                    foreach (var kvp in upgradeList)
                    {
                        if (kvp.Value.Exists(u => u.Item1 == selectedUpgradeName))
                        {
                            if (oneTimeUpgrades.Contains(kvp.Key))
                            {
                                chosenOneTimeUpgrades.Add(kvp.Key);
                            }
                            break;
                        }
                    }

                    ApplyUpgrade();
                }
                else
                {
                    CustomSceneManager.instance.DisplayWarning();
                }
            });

            upgradeButtons[i].interactable = MoneyManager.Instance.mMoney >= upgradeCost;
        }
    }

    public void ApplyUpgrade()
    {
        if (selectedUpgrade != null)
        {
            selectedUpgrade.Invoke();
            OnUpgrade?.Invoke(selectedUpgradeName);
        }

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
}
