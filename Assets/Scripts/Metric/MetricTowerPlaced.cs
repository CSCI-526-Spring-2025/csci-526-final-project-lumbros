using System.Collections.Generic;
using System;

public class MetricTowerPlaced : MetricAbstract
{
    private int waveFirstTowerPlaced = -1;
    private int _curWave;
    private Dictionary<string, int> numOfEachTowerPlaced;
    private string[] towerNames = { "Sniper", "Door", "Wall", "IceTower", "SpeedTower", "AOETower" };
    public MetricTowerPlaced()
    {
        numOfEachTowerPlaced = new Dictionary<string, int>();

        formTags = new List<string>();
        url = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfsdlnGlNSOyqi1CrU3d2gHO6hEIZfQni-XrRAXOv14ahS9zQ/formResponse";
        formTags.Add("entry.512377219"); // tag for sessionId
        formTags.Add("entry.907953535");  // tag for wave first tower placed
        
        formTags.Add("entry.889270221"); // sniper
        formTags.Add("entry.846278318"); // door
        formTags.Add("entry.96119830");  // wall
        formTags.Add("entry.1385047999");// ice tower
        formTags.Add("entry.1699989262");// speed tower
        formTags.Add("entry.1042955849");// aoe tower

        InventorySlot.AddedTower += TowerPlaced;
        CustomSceneManager.gameStateChange += OnGameOver;
        WaveManager.waveBegin += WaveBegin;
    }

    private void WaveBegin(int curWave, int waveKillLimit)
    {
        _curWave = curWave;
    }

    private void TowerPlaced(string towerName)
    {
        if(numOfEachTowerPlaced.Count == 0)
        {
            waveFirstTowerPlaced = _curWave;
        }

        int num = 0;
        numOfEachTowerPlaced.TryGetValue(towerName, out num);
        numOfEachTowerPlaced[towerName] = num + 1;
    }

    private void OnGameOver(GAMESTATE gs)
    {
        if (gs == GAMESTATE.GameOver)
        {
            List<string> formValues = new List<string>();
            formValues.Add("space for sessionId");
            formValues.Add(waveFirstTowerPlaced.ToString());
            foreach(string towerName in towerNames)
            {
                int num = 0;
                numOfEachTowerPlaced.TryGetValue(towerName, out num);
                formValues.Add(num.ToString());
            }
            Post(formValues);

            // reset
            waveFirstTowerPlaced = -1;
            numOfEachTowerPlaced = new Dictionary<string, int>();
        }
    }
}