using UnityEngine;
using System.Collections;

public class EnemyPhantom : EnemyAbstract
{
    protected override void StartCall()
    {
        int enemyLayer = LayerMask.NameToLayer("EnemyDisCol");

        // ignorecollider
        string[] ignoredLayers = { "EnemyDisCol, Obstacle", "Tower" };

        foreach (string layerName in ignoredLayers)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer != -1)
            {
                Physics2D.IgnoreLayerCollision(enemyLayer, layer);
            }
            else
            {
                Debug.LogWarning($"Layer '{layerName}' 不存在，请检查是否正确命名！");
            }
        }

        health = 3; // Enemy health
        speed = 1.5f;
        attackDamage = 1; // Melee attack damage
        attackRange = 0.5f; // Melee attack range
        attackCooldown = 1f; // Attack cooldown time

        //初始化血量
        health = Mathf.CeilToInt(health * WaveManager.Instance.enemyHealthMultiplier);
    }

    public override void SetAggroTarget(Transform newTarget)
    {
        throw new System.NotImplementedException();
    }

    protected override void PostDamage(int damage, Transform attacker)
    {
        // do nothing
    }
}
