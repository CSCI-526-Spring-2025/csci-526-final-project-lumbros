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

    private Dictionary<string, List<(string, System.Action)>> upgradeCategories = new Dictionary<string, List<(string, System.Action)>>();

    void Start()
    {
        hero = GameObject.FindGameObjectWithTag("Player");
        manager = GameObject.FindGameObjectWithTag("Manager");
        towers = GameObject.FindGameObjectsWithTag("Tower");

        AddUpgradeCategory("Hero Damage", "Increase Hero Damage +1", () => hero.GetComponent<AutoAttack>().damage += 1);
        AddUpgradeCategory("Hero Damage", "Increase Hero Damage +2", () => hero.GetComponent<AutoAttack>().damage += 2);
        
        AddUpgradeCategory("Attack Speed", "Increase Attack Speed (0.7x)", () => hero.GetComponent<AutoAttack>().attackCooldown *= 0.7f);
        AddUpgradeCategory("Attack Speed", "Increase Attack Speed (0.5x)", () => hero.GetComponent<AutoAttack>().attackCooldown *= 0.4f);
        
        AddUpgradeCategory("Max HP", "Increase Max HP +1", () =>
        {
            Health hp = hero.GetComponent<Health>();
            hp.maxHealth++;
            hp.currentHealth++;
        });
        AddUpgradeCategory("Max HP", "Increase Max HP +2", () =>
        {
            Health hp = hero.GetComponent<Health>();
            hp.maxHealth += 2;
            hp.currentHealth += 2;
        });

        AddUpgradeCategory("Hero Auto Heal", "Increase Hero Auto Heal +1", () => hero.GetComponent<Health>().autoHeal += 1);
        AddUpgradeCategory("Hero Auto Heal", "Increase Hero Auto Heal +2", () => hero.GetComponent<Health>().autoHeal += 2);

        AddUpgradeCategory("Move Speed", "Increase Move Speed +1", () => hero.GetComponent<HeroMovement>().moveSpeed += 1);
        AddUpgradeCategory("Move Speed", "Increase Move Speed +2", () => hero.GetComponent<HeroMovement>().moveSpeed += 2);

        AddUpgradeCategory("Tower Count", "Increase Max Towers +1", () => TowerManager.instance.maxTowerCount++);

        AddUpgradeCategory("Tower Attack Range", "Increase All Towers Attack Range +1", () =>
        {
            foreach (var tower in towers)
                tower.GetComponent<AutoAttack>().attackRange += 1;
        });
        AddUpgradeCategory("Tower Attack Range", "Increase All Towers Attack Range +2", () =>
        {
            foreach (var tower in towers)
                tower.GetComponent<AutoAttack>().attackRange += 2;
        });
        
        AddUpgradeCategory("Tower Damage", "Increase All Towers Damage +1", () =>
        {
            foreach (var tower in towers)
                tower.GetComponent<AutoAttack>().damage += 1;
        });
        AddUpgradeCategory("Tower Damage", "Increase All Towers Damage +2", () =>
        {
            foreach (var tower in towers)
                tower.GetComponent<AutoAttack>().damage += 2;
        });
        
        AddUpgradeCategory("Tower HP", "Increase All Towers HP +1", () =>
        {
            foreach (var tower in towers)
            {
                Health hp = tower.GetComponent<Health>();
                hp.maxHealth++;
                hp.currentHealth++;
            }
        });
        AddUpgradeCategory("Tower HP", "Increase All Towers HP +2", () =>
        {
            foreach (var tower in towers)
            {
                Health hp = tower.GetComponent<Health>();
                hp.maxHealth += 2;
                hp.currentHealth += 2;
            }
        });
        
        AddUpgradeCategory("Tower Auto Heal", "Increase All Towers Auto Heal +1", () =>
        {
            foreach (var tower in towers)
                tower.GetComponent<Health>().autoHeal += 1;
        });
        AddUpgradeCategory("Tower Auto Heal", "Increase All Towers Auto Heal +2", () =>
        {
            foreach (var tower in towers)
                tower.GetComponent<Health>().autoHeal += 2;
        });
        AddUpgradeCategory("Hero Bounce", "Hero Projectiles Bounce +1", () => hero.GetComponent<AutoAttack>().heroBounces += 1);
        AddUpgradeCategory("Hero Bounce", "Hero Projectiles Bounce +2", () => hero.GetComponent<AutoAttack>().heroBounces += 2);

        AssignRandomUpgrades();
    }

    void AddUpgradeCategory(string category, string description, System.Action upgradeAction)
    {
        if (!upgradeCategories.ContainsKey(category))
        {
            upgradeCategories[category] = new List<(string, System.Action)>();
        }
        upgradeCategories[category].Add((description, upgradeAction));
    }

    void AssignRandomUpgrades()
    {
        List<string> availableCategories = new List<string>(upgradeCategories.Keys);
        List<(string, System.Action)> selectedUpgrades = new List<(string, System.Action)>();

        while (selectedUpgrades.Count < upgradeButtons.Length && availableCategories.Count > 0)
        {
            int categoryIndex = Random.Range(0, availableCategories.Count);
            string chosenCategory = availableCategories[categoryIndex];

            var upgradesInCategory = upgradeCategories[chosenCategory];
            var selectedUpgrade = upgradesInCategory[Random.Range(0, upgradesInCategory.Count)];

            selectedUpgrades.Add(selectedUpgrade);
            availableCategories.RemoveAt(categoryIndex);
        }

        for (int i = 0; i < selectedUpgrades.Count; i++)
        {
            int index = i;
            upgradeTexts[i].text = selectedUpgrades[index].Item1;

            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() => selectedUpgrades[index].Item2.Invoke());
            upgradeButtons[i].onClick.AddListener(() => LoadMainScene());
        }
    }

    void LoadMainScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SimpleScene");
    }
}
