    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using UnityEngine.SceneManagement;
    using System;
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
        private void Awake()
        {
            // Check if instance already exists
            if (Instance == null)
            {
                // If not, set instance to this
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


        void Start()
        {

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
    }
