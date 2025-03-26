using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HeroTextUI : MonoBehaviour
{
    private TMP_Text textComponent;
    private GameObject HeroObject;
    private AutoAttack attackInfo;
    private Health health;
    // Start is called before the first frame update
    void Start()
    {
        textComponent =  GameObject.Find("HeroTextUI").GetComponent<TMP_Text>();
        HeroObject = GameObject.FindGameObjectWithTag("Player");
        health =  HeroObject.GetComponent<Health>();
        attackInfo = HeroObject.GetComponent<AutoAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        textComponent.text = $"Hero\nHealth: {health.currentHealth} / {health.maxHealth} \nAttack Damage: {attackInfo.attackRange} \nAttack Speed: {attackInfo.attackCooldown}";
    }
}
