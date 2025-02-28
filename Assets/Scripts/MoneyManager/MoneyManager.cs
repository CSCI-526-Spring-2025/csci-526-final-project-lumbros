using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MoneyManager : MonoBehaviour
{  
    public TMP_Text mMoneyUI;
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
        mMoneyUI.text = "Money: $" +mMoney.ToString();
    }

    public void UpdateMoney(int change){
        mMoney += change;
    }
}
