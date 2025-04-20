using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour
{
    // Basic health settings - can be adjusted in Unity Inspector
    public int maxHealth = 5; 
    public int currentHealth;
    public int autoHeal = 0;
    private static GameObject manager; // manage the game state
    public bool updated = false;
    // Reference to the health bar slider
    private Slider healthSlider;
    public Vector3 healthBarOffset = new Vector3(0, -0.5f, 0); // Offset from the object's position
    private float immunity = 1.0f;
    public bool heroReborn = false;
    private bool isDead = false;
    public int healthExpect;
    public float rebornDelay = 3f;
    public float workerRebornDelay = 5f;

    public string name = "";

    // for flashing
    public Color originalColor;
    public Color flash = new Color(255, 255, 255);
    private SpriteRenderer spriteRenderer;
    public float delay = 0.1f;
    public int numOfFlash = 4;

    // for health sprite changing
    public Sprite[] healthSprites; // Array of 4 sprites (full to empty)

    void Start()
    {
        // Set initial health and configure the UI slider
        manager = GameObject.FindGameObjectWithTag("Manager");
        if(name == "core"){
            healthSlider = GameObject.Find("CoreHealthBar").GetComponent<Slider>();
        }
        else if(name == "hero"){
            healthSlider = GameObject.Find("HeroHealthBar").GetComponent<Slider>();
        }
        currentHealth = maxHealth;
        healthExpect = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            // // If the slider isn't a child object, position it
            // if (healthSlider.transform.parent != transform)
            // {
            //     // Create a Canvas in world space and set it as the slider's parent
            //     Canvas canvas = healthSlider.GetComponentInParent<Canvas>();
            //     if (canvas != null)
            //     {
            //         canvas.renderMode = RenderMode.WorldSpace;
            //         canvas.transform.SetParent(transform);
            //         canvas.transform.localPosition = healthBarOffset;
            //     }
            // }
        }
        InvokeRepeating("AutoHeal", 5f, 5f);

        if(transform.tag == "Tower"){
            spriteRenderer = GetComponent<SpriteRenderer>();
            flash = new Color(255, 0, 0);
            originalColor = spriteRenderer.color;
        }
        else if(transform.tag == "Worker"){
            Transform child = transform.GetChild(0);
            spriteRenderer = child.GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;
        }

        UpdateSprite();
    }

    void Update()
    {
        // Update health bar position if it's not a child object
        // if (healthSlider != null && healthSlider.transform.parent != transform)
        // {
        //     healthSlider.transform.position = transform.position + healthBarOffset;
        // }
        immunity -= Time.deltaTime;
        currentHealth = currentHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }


    }

    // Public method for other scripts to deal damage
    public void TakeDamage(int damage, string tag)
    {
        // Prevents auto death
        if(immunity < 0.0f){
            currentHealth -= damage;
            // Update the health bar if it exists
            // if (healthSlider != null)
            // {
            //     healthSlider.maxValue = maxHealth;
            //     healthSlider.value = currentHealth;
            // }

            // Destroy the object if health drops to zero or below
            if (currentHealth <= 0)
            {
                Die(tag);
            }
            else if(spriteRenderer != null){
                StartCoroutine(HitFlash());
            }
        }
        UpdateSprite();
    }

    public void TakeExpectedDamage(int dmg)
    {
        healthExpect -= dmg;
        if (healthExpect < 0) healthExpect = 0;
    }

    IEnumerator HitFlash()
    {
        int howManyFlash = numOfFlash;

        while(howManyFlash > 0)
        {
            spriteRenderer.color = flash; // flash
            yield return new WaitForSeconds(delay);
            yield return null;
            spriteRenderer.color = originalColor; // original color
            yield return new WaitForSeconds(delay);
            yield return null;
            howManyFlash--;
        }
    }

    void Die(string tag)
    {
        if(tag == "Player" || tag == "Core"){
            Debug.Log(heroReborn +" test " + tag);
            if(heroReborn && tag == "Player"){
                isDead = true;
                gameObject.SetActive(false);
                CoroutineRunner.Instance.StartCoroutine(RebornAfterDelay(rebornDelay)); 
            }else{
                Debug.Log("game over");
                manager.GetComponent<CustomSceneManager>().GameOver();
            }
            
        }
        else if (tag == "Worker")
        {
            isDead = true;
            gameObject.SetActive(false);
            CoroutineRunner.Instance.StartCoroutine(RebornAfterDelay(workerRebornDelay));
     
        }
        else if(tag == "Tower") 
        {
            Debug.Log("Health DIe");
            TowerManager.Instance.DestoryTower(gameObject);
        }else{
            Destroy(gameObject);
        }
        
    }
    IEnumerator RebornAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Reborn();
    }

void Reborn()
{
    currentHealth = maxHealth;
    immunity = 1.0f;

    gameObject.SetActive(true); // Reactivate Hero first

    if (healthSlider == null)
    {
        healthSlider = GetComponentInChildren<Slider>();
    }

    if (healthSlider != null)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        //healthSlider.transform.position = transform.position + healthBarOffset;
    }

    Debug.Log(gameObject.name + " has reborn!");
}
    void AutoHeal()
    {
        if (currentHealth > 0 && currentHealth < maxHealth) 
        {
            currentHealth += autoHeal;
            if (currentHealth > maxHealth) currentHealth = maxHealth;

            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
            }

            UpdateSprite();
        }
    }

    void UpdateSprite()
    {
        if (healthSprites == null || healthSprites.Length == 0)
        {
            return;
        }

        float healthPercent = (float)currentHealth / maxHealth;

        // Convert health percent to an index: 0 = full, 3 = almost dead
        int spriteIndex = Mathf.FloorToInt((1 - healthPercent) * (healthSprites.Length));
        spriteIndex = Mathf.Clamp(spriteIndex, 0, healthSprites.Length - 1);

        spriteRenderer.sprite = healthSprites[spriteIndex];
    }
}
