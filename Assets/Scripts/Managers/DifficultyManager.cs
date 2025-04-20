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
        Debug.Log("CurrentDifficulty: " + CurrentDifficulty);

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
