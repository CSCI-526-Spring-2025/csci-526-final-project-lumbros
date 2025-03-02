using UnityEngine;
using System.Collections;

public class RangedEnemy : MonoBehaviour, IDamageable
{
    // Basic enemy properties
    public float speed = 2f;
    private static GameObject manager; // manage the game state
    private Transform target;
    private Transform core;
    private bool isAggroed = false;
    public int health = 3;

    // Ranged attack properties
    public GameObject projectilePrefab;
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    private WaveManager wavemanager;
    private bool canAttack = true;
    private Rigidbody2D rb;
    // public float acceleration = 5f;  
    // public float maxSpeed = 2f;   
    void Start()
    {
        // Initialize reference to core
        GameObject coreObject = GameObject.FindGameObjectWithTag("Core");
        if (coreObject != null)
        {
            core = coreObject.transform;
        }
        manager = GameObject.FindGameObjectWithTag("Manager");
        target = FindClosestTarget(); // Instead of directly targeting core
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyEnCol"), LayerMask.NameToLayer("EnemyEnCol"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyEnCol"), LayerMask.NameToLayer("Hero"));
        rb = GetComponent<Rigidbody2D>();
        rb.drag = 2f; 

        //初始化血量
        wavemanager = FindObjectOfType<WaveManager>();
        health = Mathf.CeilToInt(health * wavemanager.enemyHealthMultiplier);
    }

    void Update()
    {
        // Regularly update target to find closest one
        if (!isAggroed)  // Only search for new targets if not aggroed
        {
            target = FindClosestTarget();
        }

        if (target == null)
        {
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        // If within attack range, stop and shoot
        if (distanceToTarget <= attackRange && canAttack)
        {
            StartCoroutine(ShootAtTarget());
        }
        // If outside attack range, move closer
        else if (distanceToTarget > attackRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    // void FixedUpdate()
    //     {
    //         if (target != null)
    //         {
                
    //             rb.velocity = Vector2.zero;

                
    //             Vector2 direction = (target.position - transform.position).normalized;
    //             rb.AddForce(direction * acceleration, ForceMode2D.Force);

    //             if (rb.velocity.magnitude > maxSpeed)
    //             {
    //                 rb.velocity = rb.velocity.normalized * maxSpeed;
    //             }
    //             // TryAttack();
    //         }
    //     }

    // New method to find closest target
    Transform FindClosestTarget()
    {
        if (GameObject.FindGameObjectWithTag("Core") == null)
        {
            return null;
        }
        // Create a list of possible targets
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = float.MaxValue;
        Transform closestTarget = core; // Default to core if no closer targets found

        // Check distance to core
        float coreDistance = Vector2.Distance(transform.position, core.position);
        minDistance = coreDistance;

        // Check distance to each player
        foreach (GameObject player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTarget = player.transform;
            }
        }

        return closestTarget;
    }

    IEnumerator ShootAtTarget()
    {
        canAttack = false;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<EnemyProjectile>().SetDirection(target.position, transform);
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
            manager.GetComponent<CustomSceneManager>().AddKill();
        }
        else
        {
            SetAggroTarget(attacker);
        }
    }

    public void SetAggroTarget(Transform newTarget)
    {
        if (!isAggroed)
        {
            target = newTarget;
            isAggroed = true;
        }
    }

    void LateUpdate()
    {
        // If aggroed target is destroyed, return to normal targeting
        if (isAggroed && target == null)
        {
            isAggroed = false;
            target = FindClosestTarget();
        }
    }
}