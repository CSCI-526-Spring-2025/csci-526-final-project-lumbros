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
    public Slider healthSlider;
    public Vector3 healthBarOffset = new Vector3(0, -0.5f, 0); // Offset from the object's position
    private float immunity = 1.0f;
    public bool heroReborn = true;
    private bool isDead = false;
    public float rebornDelay = 3f;
    public float workerRebornDelay = 5f;

    void Start()
    {
        // Set initial health and configure the UI slider
        manager = GameObject.FindGameObjectWithTag("Manager");
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            // If the slider isn't a child object, position it
            if (healthSlider.transform.parent != transform)
            {
                // Create a Canvas in world space and set it as the slider's parent
                Canvas canvas = healthSlider.GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.transform.SetParent(transform);
                    canvas.transform.localPosition = healthBarOffset;
                }
            }
        }
        InvokeRepeating("AutoHeal", 5f, 5f);
    }

    void Update()
    {
        // Update health bar position if it's not a child object
        if (healthSlider != null && healthSlider.transform.parent != transform)
        {
            healthSlider.transform.position = transform.position + healthBarOffset;
        }
        immunity -= Time.deltaTime;
        currentHealth = currentHealth;


    }

    // Public method for other scripts to deal damage
    public void TakeDamage(int damage, string tag)
    {
        // Prevents auto death
        if(immunity < 0.0f){
            currentHealth -= damage;
            // Update the health bar if it exists
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
            }

            // Destroy the object if health drops to zero or below
            if (currentHealth < 0)
            {
                Die(tag);
            }
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
        healthSlider.transform.position = transform.position + healthBarOffset;
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

            
        }
    }
}
