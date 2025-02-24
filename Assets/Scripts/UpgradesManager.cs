using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
    GameObject hero, manager, towerManager, core, minersManager;
    // Start is called before the first frame update
    void Start()
    {
        hero = GameObject.FindGameObjectWithTag("Player");
        manager = GameObject.FindGameObjectWithTag("Manager");
        core = GameObject.FindGameObjectWithTag("Core");
        //towerManager = GameObject.FindGameObjectWithTag("TowerManager");
        // minersManager = GameObject.FindGameObjectWithTag("Miner");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // hero related upgrades
    public void UpgradeHeroDamage(int damage = 1){
        hero.GetComponent<AutoAttack>().damage += damage;
    }

    public void UpgradeHeroHP(int incBy = 1)
    {
        Health hp = hero.GetComponent<Health>();
        hp.maxHealth += incBy;
        hp.currentHealth += incBy;
    }

    public void UpgradeHeroAttkSpd(float value = 0.1f)
    {
        hero.GetComponent<AutoAttack>().attackCooldown -= value;
    }

    public void UpgradeHeroAttkRange(float value = 0.5f)
    {
        hero.GetComponent<AutoAttack>().attackRange += value;
    }

    // Tower related upgrade
    public void UpgradeMaxTowerCount(int incBy = 1){
        TowerManager.instance.maxTowerCount += incBy;
    }


}
