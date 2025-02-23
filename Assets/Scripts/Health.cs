using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    // Basic health settings - can be adjusted in Unity Inspector
    public int maxHealth = 5; 
    public int currentHealth;
    private static GameObject manager; // manage the game state

    // Reference to the health bar slider
    public Slider healthSlider;
    public Vector3 healthBarOffset = new Vector3(0, -0.5f, 0); // Offset from the object's position

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
    }

    void Update()
    {
        // Update health bar position if it's not a child object
        if (healthSlider != null && healthSlider.transform.parent != transform)
        {
            healthSlider.transform.position = transform.position + healthBarOffset;
        }
    }

    // Public method for other scripts to deal damage
    public void TakeDamage(int damage, string tag)
    {
        currentHealth -= damage;

        // Update the health bar if it exists
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // Destroy the object if health drops to zero or below
        if (currentHealth <= 0)
        {
            Die(tag);
        }
    }

    void Die(string tag)
    {
        if(tag == "Player" || tag == "Core") manager.GetComponent<CustomSceneManager>().GameOver();
        if(tag == "Tower") TowerManager.instance.DestoryTower();
        Destroy(gameObject);
    }
}
