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
        // towerManager = GameObject.FindGameObjectWithTag("TowerManager");
        // minersManager = GameObject.FindGameObjectWithTag("Miner");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpgradeHeroDamage(){
        hero.GetComponent<AutoAttack>().damage++;
    }

    public void UpgradeMaxTowerCount(){
        TowerManager.instance.maxTowerCount++;
    }

    public void UpgradeHeroHP(){
        Health hp = hero.GetComponent<Health>();
        hp.maxHealth++;
        hp.currentHealth++;
    }
}
