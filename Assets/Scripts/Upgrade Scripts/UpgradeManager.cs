using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UpgradeManager : MonoBehaviour
{
    // Start is called before the first frame update
    public List<TMP_Text> mButtons;
    public List<TMP_Text> mCostUI;
    // Holds Upgrade Names and Costs
    public List<string> mUpgradeName;
    public List<string> mUpgradeCost;
    public int mNumberOfUpgrades = 3;

    void Start()
    {
        List<int> randomNum = new List<int>();  

        // Get 3 unique numbers
        while (randomNum.Count < mNumberOfUpgrades)
        {
            randomNum.Add(Random.Range(0, mUpgradeName.Count));
        }
        for(int i = 0; i < mNumberOfUpgrades; i++)
        {
            Debug.Log(randomNum[i]);
        }
        for(int i = 0; i < mNumberOfUpgrades; i++)
        {
            int index = randomNum[i];
            mButtons[i].text = mUpgradeName[index];
            mCostUI[i].text = "$" + mUpgradeCost[index];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
