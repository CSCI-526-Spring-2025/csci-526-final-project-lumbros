using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Upgrades : MonoBehaviour
{
    public TextMeshProUGUI[] upgradeTexts;
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

    private Dictionary<string, List<(string, System.Action, int)>> heroUpgrades = new Dictionary<string, List<(string, System.Action, int)>>();
    private Dictionary<string, List<(string, System.Action, int)>> towerUpgrades = new Dictionary<string, List<(string, System.Action, int)>>();

    public static System.Action<string> OnUpgrade;

    void Start()
    {
        hero = GameObject.FindGameObjectWithTag("Player");
        manager = GameObject.FindGameObjectWithTag("Manager");

        AddUpgrade(heroUpgrades, "Hero Damage", "+1 to Hero Damage", () => hero.GetComponent<AutoAttack>().damage += 1, 50);
        AddUpgrade(heroUpgrades, "Hero Damage", "+2 to Hero Damage", () => hero.GetComponent<AutoAttack>().damage += 2, 100);
        AddUpgrade(heroUpgrades, "Attack Speed", "Increase Hero Attack Speed (0.7x)", () => hero.GetComponent<AutoAttack>().attackCooldown *= 0.7f, 75);
        AddUpgrade(heroUpgrades, "Hero HP", "+1 to Hero HP", () => hero.GetComponent<Health>().maxHealth++, 60);
        AddUpgrade(heroUpgrades, "Hero Auto Heal", "+1 to Hero Auto Heal", () => hero.GetComponent<Health>().autoHeal += 1, 80);
        AddUpgrade(heroUpgrades, "Move Speed", "Increase Move Speed +1", () => hero.GetComponent<HeroMovement>().moveSpeed += 1, 70);

        AddUpgrade(towerUpgrades, "Tower Range", "+1 to Tower Attack Range", () => ApplyTowerUpgrade(tower => tower.GetComponent<AutoAttack>().attackRange += 1), 90);
        AddUpgrade(towerUpgrades, "Tower Auto Heal", "+1 to Tower Auto Heal", () => ApplyTowerUpgrade(tower => tower.GetComponent<Health>().autoHeal += 1), 85);
        AddUpgrade(towerUpgrades, "Tower Damage", "+1 to Tower Damage", () => ApplyTowerUpgrade(tower => tower.GetComponent<AutoAttack>().damage += 1), 110);
        AddUpgrade(towerUpgrades, "Tower HP", "+1 to Tower HP", () => ApplyTowerUpgrade(tower => tower.GetComponent<Health>().maxHealth++), 95);

        AssignUpgrades(heroUpgrades);
    }

    void Update()
    {
        towers = GameObject.FindGameObjectsWithTag("Tower");
        currWave = WaveManager.Instance.currentWave;
        Wave.text = "Wave " + (currWave - 1) + " Completed!";

        UpgradeText.text = (phase == 1) ? "Choose an upgrade for hero" : "Choose an upgrade for all towers";
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
            var selectedUpgrade = upgradesInCategory[Random.Range(0, upgradesInCategory.Count)];

            selectedUpgrades.Add(selectedUpgrade);
            availableCategories.RemoveAt(categoryIndex);
        }

        for (int i = 0; i < selectedUpgrades.Count; i++)
        {
            int index = i;
            int upgradeCost = selectedUpgrades[index].Item3;
            string formattedText = upgradeTexts[i].text = $"{selectedUpgrades[index].Item1}\n<align=center>Cost: {selectedUpgrades[index].Item3}</align>";

            upgradeTexts[i].text = formattedText;

            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() =>
            {
                if (MoneyManager.Instance.mMoney >= upgradeCost)
                {
                    MoneyManager.Instance.mMoney -= upgradeCost;
                    selectedUpgrade = selectedUpgrades[index].Item2;
                    selectedUpgradeName = selectedUpgrades[index].Item1;
                    selectedUpgradeCost = upgradeCost;
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

   

    void ApplyTowerUpgrade(System.Action<GameObject> upgradeAction)
    {
        foreach (var tower in towers)
        {
            upgradeAction(tower);
        }
    }

    public void ApplyUpgrade()
    {
        if (selectedUpgrade != null)
        {
            // MoneyManager.Instance.mMoney -= selectedUpgradeCost;
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
    public void cancel(){
        if(phase == 1){
            phase = 2;
            UpgradeUI.SetActive(false);
            UpgradeUI.SetActive(true); 
        }else{
            phase = 1;
            LoadMainScene();
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
