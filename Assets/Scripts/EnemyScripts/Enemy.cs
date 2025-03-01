using UnityEngine;
using System.Collections;

public class Enemy : EnemyAbstract
{        
    protected override void StartCall()
    {
        enemyType = "Enemy - Basic";
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
