using UnityEngine;
using System.Collections;

public class Enemy : EnemyAbstract
{
    protected override void StartCall()
    {
        enemyType = "Enemy - Basic";
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
