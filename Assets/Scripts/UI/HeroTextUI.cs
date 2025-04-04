using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HeroTextUI : MonoBehaviour
{
    private TMP_Text textComponent;
    private GameObject HeroObject;
    private AutoAttack attackInfo;
    private HeroMovement movementInfo; 
    private Health health;
    // Start is called before the first frame update
    void Start()
    {
        textComponent =  GameObject.Find("HeroTextUI").GetComponent<TMP_Text>();
        FindHero();
    }

    // Update is called once per frame
    void Update()
    {
        FindHero();
        if(HeroObject!= null)
            textComponent.text = $"Hero\nHealth: {health.currentHealth} / {health.maxHealth} \nAttack Damage: {attackInfo.damage} \nAttack Speed: {attackInfo.attackCooldown} \nMovement Speed: {movementInfo.moveSpeed}";
    }

    void FindHero(){
        HeroObject = GameObject.FindGameObjectWithTag("Player");
        if(HeroObject != null){
            health =  HeroObject.GetComponent<Health>();
            attackInfo = HeroObject.GetComponent<AutoAttack>();
            movementInfo = HeroObject.GetComponent<HeroMovement>();
        }
    }
}
