using UnityEngine;
using System.Collections;

public class EnemyPhantom : EnemyAbstract
{
    protected override void StartCall()
    {
        int enemyLayer = LayerMask.NameToLayer("EnemyDisCol");

        // 需要忽略碰撞的层
        string[] ignoredLayers = { "EnemyDisCol, Obstacle", "Tower" };

        foreach (string layerName in ignoredLayers)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer != -1) // 确保层存在
            {
                Physics2D.IgnoreLayerCollision(enemyLayer, layer);
            }
            else
            {
                Debug.LogWarning($"Layer '{layerName}' 不存在，请检查是否正确命名！");
            }
        }

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
