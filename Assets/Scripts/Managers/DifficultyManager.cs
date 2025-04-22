using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.UI;
public enum DifficultyLevel { Easy, Hard }

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyLevel CurrentDifficulty = DifficultyLevel.Easy;
    public static GameObject FinishedGameUIHard;
    public static GameObject FinishedGameUIEasy;
    public static DifficultyManager Instance;

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
        
        FinishedGameUIHard = GameObject.Find("FinishedGameUI-Hard");
        FinishedGameUIHard.SetActive(false);
        FinishedGameUIEasy = GameObject.Find("FinishedGameUI-Easy");
        FinishedGameUIEasy.SetActive(false);
        DontDestroyOnLoad(FinishedGameUIHard);
        DontDestroyOnLoad(FinishedGameUIEasy);
    }

    public static void setDifficultyToEasy(){
        
         CurrentDifficulty = DifficultyLevel.Easy;
         
    }
    public static void setDifficultyToHard(){
         CurrentDifficulty = DifficultyLevel.Hard;

    }
    public static float GetHeroGoldCostMultiplier()
    {
        switch (CurrentDifficulty)
        {
            case DifficultyLevel.Easy:
                return 1.0f; 
            case DifficultyLevel.Hard:
                return 2.0f; 
            default:
                return 1f;
        }
    }
    public static GameObject GetFinishedGameUI()
    {

      Debug.Log("CurrentDifficulty1: " + CurrentDifficulty);
        Debug.Log("CurrentDifficulty3: " + (FinishedGameUIHard == null));
        Debug.Log("CurrentDifficulty2: ");
        Debug.Log("CurrentDifficulty1: " + (FinishedGameUIEasy == null));
        switch (CurrentDifficulty)
        {
            
            case DifficultyLevel.Easy:
                return FinishedGameUIEasy; 
            case DifficultyLevel.Hard:
                return FinishedGameUIHard;
            default:
                return FinishedGameUIHard;
        }
    }
    public static float GetTowerGoldCostMultiplier()
    {
        switch (CurrentDifficulty)
        {
            case DifficultyLevel.Easy:
                return 1.0f; 
            case DifficultyLevel.Hard:
                return 3.0f; 
            default:
                return 1f;
        }
    }
    public static float GetCoreHealthMultiplier()
    {
        switch (CurrentDifficulty)
        {
            case DifficultyLevel.Easy: return 1.0f;
            case DifficultyLevel.Hard: return 0.5f;
            default: return 0.7f;
        }
    }
    public static float GetHeroHealthMultiplier()
    {
        switch (CurrentDifficulty)
        {
            case DifficultyLevel.Easy: return 1.0f;
            case DifficultyLevel.Hard: return 0.5f;
            default: return 1f;
        }
    }
    public static int GetTowerHealthFactor()
    {
       

        switch (CurrentDifficulty)
        {
            case DifficultyLevel.Easy: return 3;
            case DifficultyLevel.Hard: return 0;
            default: return 0;
        }
    }
    public static bool GetEnemySpawn()
    {
        switch (CurrentDifficulty)
        {
            case DifficultyLevel.Easy: return true;
            case DifficultyLevel.Hard: return false;
            default: return true;
        }
    }
    void Start(){

    }

}
