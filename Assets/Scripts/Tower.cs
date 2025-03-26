using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public class Tower : MonoBehaviour, IPointerClickHandler
{
    
    public int Cost;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            GameObject TowerDetailsPopupUI = CustomSceneManager.instance.TowerInfoUI;
            if (hit.collider != null)
            {
                TowerDetailsPopupUI.SetActive(true);
                GameObject ObjectTower = hit.collider.gameObject;
                Debug.Log("Clicked on: " + ObjectTower.name);
                TMP_Text popupText = GameObject.Find("TowerDetailsPopupText")?.GetComponent<TMP_Text>();
                popupText.text = "";
                if(ObjectTower.tag == "Tower")
                {
                    Health health = ObjectTower.GetComponent<Health>();
                    AutoAttack attackInfo = ObjectTower.GetComponent<AutoAttack>();
                    int currentHealth = health.currentHealth;
                    int maxHealth = health.maxHealth;
                   
                  
                    TMP_Text towerNameText = GameObject.Find("TowerName")?.GetComponent<TMP_Text>();

        
                    string name = ObjectTower.name.Split('(')[0]; 
                    towerNameText.text = name;

                    if(popupText != null)
                    {
                        popupText.text = $"Health: {currentHealth} / {maxHealth} \n";
                    } 

                    // For Wall Door...
                    if(attackInfo != null){
                        float damage = attackInfo.damage;
                        float range = attackInfo.attackRange;
                        popupText.text += $"Attack Damage: {damage} \n";
                        popupText.text += $"Range: {range} \n";
                    }


                }
                else{
                    TowerDetailsPopupUI.SetActive(false);
                }
            }
            else{
                TowerDetailsPopupUI.SetActive(false);
            }


        }
    }

    public int GetCost(){
        return Cost;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(this.name + "UI Element Clicked!");
    }

    public void ShowDebug(){
         Debug.Log(this.name + "UI Element Clicked!");
    }
}
