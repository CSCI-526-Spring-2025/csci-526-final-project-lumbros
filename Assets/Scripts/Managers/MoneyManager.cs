using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
public class MoneyManager : MonoBehaviour
{  
    private TMP_Text mMoneyUI;
    public int mMoney = 0;
    public static MoneyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicate instances
            return;
        }

        Instance = this; // Assign the singleton instance
    }


    void Update()
    {
        mMoneyUI.text = mMoney.ToString();
    }

    public void UpdateMoney(int change){
        if(change > 0)
        {
            CustomSceneManager.instance.MoneyPopUp(change);
        }
        mMoney += change;
    }

    // Handle UI On load
     private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {   
        SceneManager.sceneLoaded -= OnSceneLoaded; 
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mMoneyUI = GameObject.Find("MoneyTextUI")?.GetComponent<TMP_Text>();
    }
}
