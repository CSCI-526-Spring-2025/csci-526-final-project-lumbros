using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UpgradesManager : MonoBehaviour
{
    public TextMeshProUGUI[] upgradeTexts;
    public Button[] upgradeButtons;
    private GameObject hero;
    public TMP_Text[] costsUI;
    private GameObject manager;
    private GameObject[] towers;

    private Dictionary<string, List<(string, System.Action)>> heroUpgrades = new Dictionary<string, List<(string, System.Action)>>();
    private Dictionary<string, List<(string, System.Action)>> towerUpgrades = new Dictionary<string, List<(string, System.Action)>>();
    private int phase = 1; // 1 = Hero Upgrades, 2 = Tower Upgrades
    private System.Action selectedUpgrade; 

    void Start()
    {
        hero = GameObject.FindGameObjectWithTag("Player");
        manager = GameObject.FindGameObjectWithTag("Manager");

        // Hero Upgrades
        AddUpgrade(heroUpgrades, "Hero Damage", "+1 to Hero Damage", () => hero.GetComponent<AutoAttack>().damage += 1);
        AddUpgrade(heroUpgrades, "Hero Damage", "+2 to Hero Damage", () => hero.GetComponent<AutoAttack>().damage += 2);

        AddUpgrade(heroUpgrades, "Attack Speed", "Increase Hero Attack Speed (0.7x)", () => hero.GetComponent<AutoAttack>().attackCooldown *= 0.7f);
        AddUpgrade(heroUpgrades, "Attack Speed", "Increase Hero Attack Speed (0.5x)", () => hero.GetComponent<AutoAttack>().attackCooldown *= 0.4f);

        AddUpgrade(heroUpgrades, "Hero HP", "+1 to Hero HP", () =>
        {
            Health hp = hero.GetComponent<Health>();
            hp.maxHealth++;
            hp.currentHealth++;
        });
        AddUpgrade(heroUpgrades, "Hero HP", "+2 to Hero HP", () =>
        {
            Health hp = hero.GetComponent<Health>();
            hp.maxHealth += 2;
            hp.currentHealth += 2;
        });

        // Tower Upgrades
        AddUpgrade(towerUpgrades, "Tower Count", "+1 to Max Tower Count", () => manager.GetComponent<CustomSceneManager>().maxTowerCount++);

        AddUpgrade(towerUpgrades, "Tower Range", "+1 to Tower Attack Range", () =>
        {
            foreach (var tower in towers)
            {
                Debug.Log("In Tower Loop");
                tower.GetComponent<AutoAttack>().attackRange += 1;
            }
    
        });
        AddUpgrade(towerUpgrades, "Tower Range", "+2 to Tower Attack Range", () =>
        {
            foreach (var tower in towers)
            {
                Debug.Log("In Tower Loop");
                tower.GetComponent<AutoAttack>().attackRange += 2;
            }
                
        });

        AddUpgrade(towerUpgrades, "Tower Damage", "+1 to Tower Damage", () =>
        {
            foreach (var tower in towers)
            {
                Debug.Log("In Tower Loop");
                tower.GetComponent<AutoAttack>().damage += 1;
            }
                
        });
        AddUpgrade(towerUpgrades, "Tower Damage", "+2 to Tower Damage", () =>
        {
            foreach (var tower in towers)
            {
                Debug.Log("In Tower Loop");
                tower.GetComponent<AutoAttack>().damage += 2;
            }
                
        });

        AddUpgrade(towerUpgrades, "Tower HP", "+1 to Tower HP", () =>
        {
            foreach (var tower in towers)
            {
                Health hp = tower.GetComponent<Health>();
                hp.maxHealth++;
                hp.currentHealth++;
            }
        });
        AddUpgrade(towerUpgrades, "Tower HP", "+2 to Tower HP", () =>
        {
            foreach (var tower in towers)
            {
                Health hp = tower.GetComponent<Health>();
                hp.maxHealth += 2;
                hp.currentHealth += 2;
            }
        });

        AssignUpgrades(heroUpgrades);
    }

    void Update(){
        towers = GameObject.FindGameObjectsWithTag("Tower");

    }
    void AddUpgrade(Dictionary<string, List<(string, System.Action)>> upgradeList, string category, string description, System.Action upgradeAction)
    {
        if (!upgradeList.ContainsKey(category))
        {
            upgradeList[category] = new List<(string, System.Action)>();
        }
        upgradeList[category].Add((description, upgradeAction));
    }

    void AssignUpgrades(Dictionary<string, List<(string, System.Action)>> upgradeList)
    {
        List<string> availableCategories = new List<string>(upgradeList.Keys);
        List<(string, System.Action)> selectedUpgrades = new List<(string, System.Action)>();

        while (selectedUpgrades.Count < upgradeButtons.Length && availableCategories.Count > 0)
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
            upgradeTexts[i].text = selectedUpgrades[index].Item1;

            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() =>
            {
                selectedUpgrade = selectedUpgrades[index].Item2;
                ApplyUpgrade();
            });
        }
    }

    public void ApplyUpgrade()
    {
        if (selectedUpgrade != null)
        {
            selectedUpgrade.Invoke(); 
            Debug.Log("Upgrade Applied!");
        }

        if (phase == 1)
        {
            phase = 2; 
            AssignUpgrades(towerUpgrades);
        }
        else
        {
            LoadMainScene(); 
        }
    }

    void LoadMainScene()
    {
        Debug.Log("In Upgrades.cs LoadMainScene");
        int maxKill = manager.GetComponent<CustomSceneManager>().killLimit;
        if(CustomSceneManager.instance != null) 
        {
            manager.GetComponent<CustomSceneManager>().ResetAndLoad(maxKill+3);
        }
    }
}
