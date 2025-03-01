using UnityEngine;
using System.Collections;

public class RangedEnemy : EnemyAbstract
{
<<<<<<<< HEAD:Assets/Scripts/Enemies/RangedEnemy.cs
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
========
    // Ranged attack properties
    public GameObject projectilePrefab;

    protected override void StartCall()
    {
        enemyType = "Enemy - Range";
        attackCooldown = 2f;
        attackRange = 5f;
        target = FindClosestTarget();
>>>>>>>> 810ddcd (Finish refactoring enemies):Assets/Scripts/EnemyScripts/RangedEnemy.cs
    }

    protected override void Move()
    {
        // If outside attack range, move closer
        if (Vector2.Distance(transform.position, target.position) > attackRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    // **Attempt to attack the current target**
    protected override void TryAttack()
    {
        if (target != null && Vector2.Distance(transform.position, target.position) <= attackRange && canAttack)
        {
            StartCoroutine(ShootAtTarget());
        }
    }

    protected override void BeforeUpdate()
    {
        // Regularly update target to find closest one
        if (!isAggroed)  // Only search for new targets if not aggroed
        {
            target = FindClosestTarget();
        }
    }

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

<<<<<<<< HEAD:Assets/Scripts/Enemies/RangedEnemy.cs
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
========
    public override void SetAggroTarget(Transform newTarget)
>>>>>>>> 810ddcd (Finish refactoring enemies):Assets/Scripts/EnemyScripts/RangedEnemy.cs
    {
        if (!isAggroed)
        { 
            target = newTarget;
            isAggroed = true;
        }
    }

    protected override void LateUpdate()
    {
        if (isAggroed && target == null)
        {
            target = FindClosestTarget();
            isAggroed = false;
        }
    }
}