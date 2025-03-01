using UnityEngine;
using System.Collections;

public class Enemy : EnemyAbstract
{
    private WaveManager wavemanager;

    protected override void StartCall()
    {
        name = "BasicEnemy";
        //初始化血量
        wavemanager = FindObjectOfType<WaveManager>();
        health = Mathf.CeilToInt(health * wavemanager.enemyHealthMultiplier);
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
            //manager.GetComponent<CustomSceneManager>().AddKill();
        }
        else
        {
            SetAggroTarget(attacker); // Switch target to the attacker after taking damage
        }
    }

    // **Set aggro target**
    public void SetAggroTarget(Transform newTarget)
    {
        if (!isAggroed)
        {
            target = newTarget;
            isAggroed = true;
        }
    }

    // **Return to Core if the target is destroyed**
    void LateUpdate()
    {
        if (isAggroed && target == null)
        {
            target = core;
            isAggroed = false;
        }
    }
}
