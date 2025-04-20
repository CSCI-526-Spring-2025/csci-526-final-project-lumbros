using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    public static HeroMovement instance;
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    private void Awake()
    {
        // Check if instance already exists
        if (instance == null)
        {
            // If not, set instance to this
            instance = this;
        }
        else if (instance != this)
        {
            // If instance already exists and it's not this, then destroy this to enforce the singleton.
            Destroy(gameObject);
        }

    }

    void Start()
    {

        GameObject hero = GameObject.FindGameObjectWithTag("Player");
        Health hp = hero.GetComponent<Health>();
        if (hp != null)
        {
            float factor = DifficultyManager.GetHeroHealthMultiplier();
            hp.maxHealth = Mathf.CeilToInt(hp.maxHealth * factor);  
            hp.currentHealth = hp.maxHealth;
        }
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal"); 
        movement.y = Input.GetAxisRaw("Vertical");   
        movement = movement.normalized; 
    }

    void FixedUpdate()
    {
        Vector2 minBounds = CustomSceneManager.instance.GetMinBounds(); 
        Vector2 maxBounds = CustomSceneManager.instance.GetMaxBounds();
        // Calculate new position
        Vector2 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
 
        // Clamp position within the defined boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

        // Apply the movement
        rb.MovePosition(newPosition);
    }
}
