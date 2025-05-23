﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    // for flashing
    private Color originalColor;
    public Color flash = new Color(255, 255, 255);
    public Color slowDebuffColor;
    public Color outlineColor;
    private SpriteRenderer spriteRenderer;
    public float delay = 0.1f;
    public int numOfFlash = 4;
    public int healthExpect;
    private bool isDead = false;
    public int healthInit;
    public bool healthChanged = false;
    // do not overwrite
    private void OnDisable()
    {
        // enemyKill?.Invoke();
    }

    void Start()
    {
        GameObject coreObject = GameObject.FindGameObjectWithTag("Core");
        if (coreObject != null)
        {
            core = coreObject.transform;
            target = core; // Initial target is the Core
        }
        healthInit = health;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyEnCol"), LayerMask.NameToLayer("EnemyEnCol"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyEnCol"), LayerMask.NameToLayer("NormalLayer"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyEnCol"), LayerMask.NameToLayer("Projectile"));

        Transform child = transform.GetChild(1);
        if(child == null) child = transform.GetChild(0);

        spriteRenderer = child.GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        slowDebuffColor = new Color(28/255.0f, 188/255.0f, 188/255.0f);
        outlineColor = new Color(188/255.0f, 28/255.0f, 28/255.0f);

        rb = GetComponent<Rigidbody2D>();
        rb.drag = 2f;

        StartCall();
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        if (isDead) return;

        health -= damage;
        if (health <= 0)
        {
            isDead = true;

            if (deathParticle != null)
            {
                Instantiate(deathParticle, transform.position, Quaternion.identity);
            }
            //Debug.Log("Enemy kill event triggered by: " + gameObject.name);

            enemyKill?.Invoke(); //
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(EnemyHitFlash());
            PostDamage(damage, attacker);
        }
    }
    public void TakeExpectedDamage(int dmg)
    {
        healthExpect -= dmg;
        if (healthExpect < 0) healthExpect = 0;
    }
    public int getHealthExpected(){
        return healthExpect;
    }
    
    public void SetSlowDebuff(bool setDebuff){
        Color setColor;
        if(setDebuff){
            setColor = slowDebuffColor;
        }
        else{
            setColor = outlineColor;
        }
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = setColor;
    }

    IEnumerator EnemyHitFlash()
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

    // do not overwrite
    void Update()
    {
        BeforeUpdate();
        if (target == null) return;
        if(!healthChanged){
            healthExpect = health;
        }
        if(health > healthInit &&  !healthChanged){
            healthExpect = health;
            healthChanged = true;
        }
        Move();
        TryAttack();
    }

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
        SetAggroTarget(attacker);
    }

    protected abstract void StartCall();
    public abstract void SetAggroTarget(Transform newTarget);

    protected virtual void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    // **改进的 TryAttack()，优先攻击附近的 Worker、Core、Hero、Tower**
    protected virtual void TryAttack()
    {
        if (canAttack)
        {
            Transform closestTarget = FindClosestAttackableTarget();
            if (closestTarget != null)
            {
                StartCoroutine(AttackTarget(closestTarget));
            }
        }
    }

    // **寻找攻击范围内最近的单位**
    protected Transform FindClosestAttackableTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Worker") || collider.CompareTag("Core") || 
                collider.CompareTag("Player") || collider.CompareTag("Tower"))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = collider.transform;
                }
            }
        }

        return closest;
    }

    // **攻击选中的目标**
    protected virtual IEnumerator AttackTarget(Transform attackTarget)
    {
        canAttack = false;

        Health targetHealth = attackTarget.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage, attackTarget.tag);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    protected virtual void BeforeUpdate()
    {

    }
}
