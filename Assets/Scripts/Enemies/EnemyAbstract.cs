using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;

public abstract class EnemyAbstract : MonoBehaviour, IDamageable
{
    public static event Action enemyKill;

    public float speed = 1f;
    protected Transform target; // Current target
    protected Transform core;   // Core target
    protected bool isAggroed = false; // Whether the enemy is aggroed
    public int health = 4; // Enemy health
    public int attackDamage = 1; // Melee attack damage
    public float attackRange = 0.5f; // Melee attack range
    public float attackCooldown = 1f; // Attack cooldown time
    protected bool canAttack = true;
    protected Rigidbody2D rb;
    protected string enemyType = "Please rename me in StartCall";
    public ParticleSystem deathParticle;
    
    // do not overwrite
    private void OnDisable()
    {
        //enemyKill?.Invoke();
    }

    // do not overwrite
    void Start()
    {
        GameObject coreObject = GameObject.FindGameObjectWithTag("Core");
        if (coreObject != null)
        {
            core = coreObject.transform;
            target = core; // Initial target is the Core
        }

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyEnCol"), LayerMask.NameToLayer("EnemyEnCol"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyEnCol"), LayerMask.NameToLayer("NormalLayer"));

        rb = GetComponent<Rigidbody2D>();
        rb.drag = 2f; 

        StartCall();
    }

    // **Take damage**
    public void TakeDamage(int damage, Transform attacker)
    {
        health -= damage;
        if (health <= 0)
        {
            Debug.Log(enemyType + " enemy dies");
            if(deathParticle != null)
            {
                ParticleSystem deathEffectClone = Instantiate(deathParticle, transform.position, Quaternion.identity);
                //Destroy(deathEffectClone, 2.0f);
            }
            Destroy(gameObject); // Destroy the enemy when health reaches zero
            enemyKill?.Invoke();
        }
        else
        {
            PostDamage(damage, attacker);
        }
    }

    // do not overwrite
    void Update()
    {
        BeforeUpdate();
        if (target == null) return;

        Move();
        TryAttack();
    }

    // **Return to Core if the target is destroyed**
    protected virtual void LateUpdate()
    {
        if (isAggroed && target == null)
        {
            target = core;
            isAggroed = false;
        }
    }

    protected virtual void PostDamage(int damage, Transform attacker)
    {
        SetAggroTarget(attacker); // Switch target to the attacker after taking damage
    }

    // override for addition starting setting
    protected abstract void StartCall();
    // **Set aggro target**
    public abstract void SetAggroTarget(Transform newTarget);

    // how the enemey moves. default is to move toward target
    protected virtual void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    // **Attempt to attack the current target**
    protected virtual void TryAttack()
    {
        if (target != null && Vector2.Distance(transform.position, target.position) <= attackRange && canAttack)
        {
            StartCoroutine(AttackTarget());
        }
    }

    // Any setting before executing update
    protected virtual void BeforeUpdate()
    {

    }

    // **Attack logic**
    protected virtual IEnumerator AttackTarget()
    {
        canAttack = false;
        if (target.GetComponent<Health>() != null)
        {
            target.GetComponent<Health>().TakeDamage(attackDamage, target.tag);
        }
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
