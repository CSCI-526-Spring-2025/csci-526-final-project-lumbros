using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    GameObject hero, manager;
    // Start is called before the first frame update
    void Start()
    {
        hero = GameObject.FindGameObjectWithTag("Player");
        manager = GameObject.FindGameObjectWithTag("Manager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpgradeHeroDamage(){
        hero.GetComponent<AutoAttack>().damage++;
    }

    public void UpgradeMaxTowerCount(){
        manager.GetComponent<CustomSceneManager>().maxTowerCount++;
    }

    public void UpgradeHeroHP(){
        Health hp = hero.GetComponent<Health>();
        hp.maxHealth++;
        hp.currentHealth++;
    }
}
