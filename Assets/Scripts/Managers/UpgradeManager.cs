using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.UI;
public class UpgradeManager : MonoBehaviour
{

    private GameObject[] towers;
    private GameObject[] workers;
    public int towerHP = 0;
    public int towerAutoHeal = 0;
    public int towerDamage = 0;
    public int towerRange = 0;
    public int workerHP = 0;
    public int workerAutoHeal = 0;
    public static UpgradeManager Instance;
    // private void Awake()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject);  
    //     }
    //     else if (Instance != this)
    //     {
    //         Destroy(gameObject);
    //     }
    // }
private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject); 
    }
}

    void Start()
    {
    }
    public static void reset()
    {
        Instance.towerHP = 0;
        Instance.towerAutoHeal = 0;
        Instance.towerDamage = 0;
        Instance.towerRange = 0;
        Instance.workerHP = 0;
        Instance.workerAutoHeal = 0;
    }
    void Update(){
    towers = GameObject.FindGameObjectsWithTag("Tower");
    workers = GameObject.FindGameObjectsWithTag("Worker");


    foreach (var worker in workers)
    {
        
        var health = worker.GetComponent<Health>();
        if (health == null)
        {
            continue;
        }
        if (!health.updated)
        {

            health.autoHeal += workerAutoHeal;
            health.maxHealth += workerHP;
            health.currentHealth += workerHP;
            health.updated = true;
        }
    }



    foreach (var tower in towers)
    {
        
        var health = tower.GetComponent<Health>();
        if (health == null)
        {
            continue;
        }
        if (!health.updated)
        {
            if( tower.GetComponent<AutoAttack>() != null){
                tower.GetComponent<AutoAttack>().damage += towerDamage;
                tower.GetComponent<AutoAttack>().attackRange += towerRange;
            }
            
            health.autoHeal += towerAutoHeal;
            health.maxHealth += towerHP;
            health.currentHealth += towerHP;
            health.updated = true;
        }
    }
        
    }
    public static void updateTower(){
        Instance.towerHP = DifficultyManager.GetTowerHealthFactor();
    }
}
