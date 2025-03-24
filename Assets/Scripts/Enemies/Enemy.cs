using UnityEngine;
using System.Collections;

public class Enemy : EnemyAbstract
{
    protected override void StartCall()
    {
        enemyType = "Enemy - Basic";

        health = 5; // Enemy health
        speed = 0.7f;
        attackDamage = 1; // Melee attack damage
        attackRange = 0.5f; // Melee attack range
        attackCooldown = 1f; // Attack cooldown time
        //初始化血量
        health = Mathf.CeilToInt(health * WaveManager.Instance.enemyHealthMultiplier);
    }

    // **Set aggro target**
    public override void SetAggroTarget(Transform newTarget)
    {
        if (!isAggroed)
        {
            target = newTarget;
            isAggroed = true;
        }
    }
}
