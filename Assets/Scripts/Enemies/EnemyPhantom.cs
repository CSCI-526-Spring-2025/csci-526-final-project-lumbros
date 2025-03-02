using UnityEngine;
using System.Collections;

public class EnemyPhantom : MonoBehaviour, IDamageable
{
    public float speed = 2f;
    private Transform target; // Current target
    private Transform core;   // Core target
    private static GameObject manager; // manage the game state
    private bool isAggroed = false; // Whether the enemy is aggroed
    public int health = 3; // Enemy health
    public int attackDamage = 1; // Melee attack damage
    public float attackRange = 0.5f; // Melee attack range
    public float attackCooldown = 1f; // Attack cooldown time
    private bool canAttack = true;
    private WaveManager wavemanager;
    private Rigidbody2D rb;

    void Start()
    {
        GameObject coreObject = GameObject.FindGameObjectWithTag("Core");
        if (coreObject != null)
        {
            core = coreObject.transform;
            target = core;
        }
        manager = GameObject.FindGameObjectWithTag("Manager");

        int enemyLayer = LayerMask.NameToLayer("EnemyDisCol");

    // 需要忽略碰撞的层
        string[] ignoredLayers = { "EnemyDisCol, EnemyEnCol, Obstacle", "Hero", "Tower"};

        foreach (string layerName in ignoredLayers)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer != -1) // 确保层存在
            {
                Physics2D.IgnoreLayerCollision(enemyLayer, layer);
            }
        }
        rb = GetComponent<Rigidbody2D>();
        rb.drag = 2f; 

        //初始化血量
        wavemanager = FindObjectOfType<WaveManager>();
        health = Mathf.CeilToInt(health * wavemanager.enemyHealthMultiplier);
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            TryAttack();
        }
    }

    // **Attempt to attack the current target**
    void TryAttack()
    {
        if (target != null && Vector2.Distance(transform.position, target.position) <= attackRange && canAttack)
        {
            StartCoroutine(AttackTarget());
        }
    }

    // **Attack logic**
    IEnumerator AttackTarget()
    {
        canAttack = false;
        if (target.GetComponent<Health>() != null)
        {
            target.GetComponent<Health>().TakeDamage(attackDamage, target.tag);
        }
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // **Take damage**
    public void TakeDamage(int damage, Transform attacker)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject); // Destroy the enemy when health reaches zero
            manager.GetComponent<CustomSceneManager>().AddKill();
        }
        // else
        // {
        //     SetAggroTarget(attacker); // Switch target to the attacker after taking damage
        // }
    }

    // // **Set aggro target**
    // public void SetAggroTarget(Transform newTarget)
    // {
    //     if (!isAggroed)
    //     {
    //         target = newTarget;
    //         isAggroed = true;
    //     }
    // }

    // // **Return to Core if the target is destroyed**
    // void LateUpdate()
    // {
    //     if (isAggroed && target == null)
    //     {
    //         target = core;
    //         isAggroed = false;
    //     }
    // }
}
