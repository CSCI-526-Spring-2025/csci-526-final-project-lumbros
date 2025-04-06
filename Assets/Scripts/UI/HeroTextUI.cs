using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class HeroTextUI : MonoBehaviour
{
    private TMP_Text textComponent;
    private TMP_Text HealthBarTextComponent;
    private GameObject HeroObject;
    private AutoAttack attackInfo;
    private HeroMovement movementInfo; 
    private Health health;
    private Button button;
    private Sprite iSprite;
    private Sprite xSprite;
    [SerializeField] private GameObject HeroTextGameObject; 
    bool isDisplay = true;
    // Start is called before the first frame update
    void Start()
    {

        GameObject gameObject = GameObject.Find("HeroInfoButton");
        if (gameObject != null)
        {
            button = gameObject.GetComponent<Button>();
        }
        HeroTextGameObject = GameObject.Find("HeroDescription");
        textComponent =  GameObject.Find("HeroTextUI").GetComponent<TMP_Text>();
        HealthBarTextComponent = GameObject.Find("HeroHealthText").GetComponent<TMP_Text>();
        if (HeroTextGameObject != null)
        {
            Debug.Log("HeroTextGameObject Name: " + HeroTextGameObject.name);
            Debug.Log("HeroTextGameObject activeSelf: " + HeroTextGameObject.activeSelf);
            Debug.Log("HeroTextGameObject activeInHierarchy: " + HeroTextGameObject.activeInHierarchy);
        }
        else
        {
            Debug.LogError("HeroTextGameObject is null!");
        }
         if(xSprite == null){
            xSprite = Resources.Load<Sprite>("Sprites/x");
        }

        if(iSprite == null){
            iSprite = Resources.Load<Sprite>("Sprites/info");
        }

        
        FindHero();
    }

    // Update is called once per frame
void Update()
{
    if (HeroObject == null)
    {
        FindHero();
        return;
    }
    //Debug.Log("hero found " + isDisplay);
    int currentHealth = health.currentHealth;
    int maxHealth = health.maxHealth;

    HeroTextGameObject.SetActive(isDisplay);

    if (isDisplay)
    {
        textComponent.text = $"Health {currentHealth} / {maxHealth} \n" +
                                $"Attack Damage: {attackInfo.damage} \n" +
                                $"Attack Speed: {attackInfo.attackCooldown} \n" +
                                $"Movement Speed: {movementInfo.moveSpeed}";
    }


    if (HealthBarTextComponent != null && health != null)
    {
        HealthBarTextComponent.text = $"{currentHealth} / {maxHealth}";
    }
}


    void FindHero(){

        HeroObject = GameObject.FindGameObjectWithTag("Player");
        if(HeroObject != null){
            health =  HeroObject.GetComponent<Health>();
            attackInfo = HeroObject.GetComponent<AutoAttack>();
            movementInfo = HeroObject.GetComponent<HeroMovement>();
        }
    }

    public void OnClick(){
        isDisplay = !isDisplay;
        if(isDisplay){
            button.image.sprite = xSprite;
        }
        else{
            button.image.sprite = iSprite;
        }
        Debug.Log("hero text " + isDisplay);
    }
}
