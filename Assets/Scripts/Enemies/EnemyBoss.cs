using UnityEngine;
using System.Collections;

public class BossEnemy : EnemyAbstract
{
    public float detectionRange = 5f; // 发现 Player 的范围
    public int maxHealth; // Boss 最大生命值

    public GameObject projectilePrefab; // 弹幕子弹
    public float fanAngle = 60f; // 扇形弹幕角度
    public int bulletCount = 6; // 子弹数量
    public float shootCooldown = 1f; // 射击冷却时间
    private bool canShoot = true; // 是否能射击
    private bool isEnraged = false; // 是否进入弹幕模式

    public GameObject[] enemyPrefabs; // 小怪 Prefabs
    public int summonCount; // 召唤总数量
    private float summonRadius = 2.5f; // 召唤半径
    private float enemySpawnChance = 0.6f; // Enemy 召唤概率（RangedEnemy 概率 = 1 - enemySpawnChance）
    private float rangedEnemySpawnChance = 0.4f;

    private Transform player;

    private bool summonedAt75 = false;
    private bool summonedAt50 = false;
    private bool summonedAt25 = false;

    protected override void StartCall()
    {
        int enemyLayer = LayerMask.NameToLayer("EnemyDisCol");

        // ignorecollider
        string[] ignoredLayers = { "EnemyDisCol", "Default", "Buildings", "DoorLayer", "EnemyEnCol", "NormalLayer"};

        foreach (string layerName in ignoredLayers)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer != -1)
            {
                Physics2D.IgnoreLayerCollision(enemyLayer, layer);
            }
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        detectionRange = 5;
        maxHealth = 50;
        speed = 0.3f;
        attackDamage = 2; // Melee attack damage
        attackRange = 0.5f; // Melee attack range
        attackCooldown = 1f; // Attack cooldown time
        summonCount = 10;
        health = maxHealth;

        //初始化血量
        health = Mathf.CeilToInt(health * WaveManager.Instance.enemyHealthMultiplier);
    }

    protected override void Move()
    {
        if ((player == null)||(core == null)) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // **如果 Player 在 detectionRange 内，切换目标为 Player**
        if (distanceToPlayer <= detectionRange)
        {
            target = player;
        }
        // **如果 Player 离开 detectionRange，切换回 Core**
        else
        {
            target = core;
        }

        // **如果生命值低于 50%，进入弹幕模式**
        if (!isEnraged && health <= (maxHealth / 2))
        {
            isEnraged = true;
            StartCoroutine(ShootPattern());
        }

        // **75% 血量时召唤**
        if (!summonedAt75 && health <= (maxHealth * 0.75f))
        {
            summonedAt75 = true;
            SummonMinions();
        }

        // **50% 血量时召唤**
        if (!summonedAt50 && health <= (maxHealth * 0.5f))
        {
            summonedAt50 = true;
            SummonMinions();
        }

        // **25% 血量时召唤**
        if (!summonedAt25 && health <= (maxHealth * 0.25f))
        {
            summonedAt25 = true;
            SummonMinions();
        }

        // **移动向目标**
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    void SummonMinions()
    {
        for (int i = 0; i < summonCount; i++)
        {
            // **计算均匀分布角度**
            float angle = (360f / summonCount) * i;
            float summonPosX = transform.position.x + summonRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float summonPosY = transform.position.y + summonRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector2 summonPosition = new Vector2(summonPosX, summonPosY);

            // **按照概率选择小怪种类**
            GameObject minionPrefab = null;
            float randomValue = Random.value;

            if (randomValue < enemySpawnChance) // 敌人 A 出现概率
            {
                if(enemyPrefabs.Length > 0){
                    minionPrefab = enemyPrefabs[0]; // `Enemy`
                }
                else{
                    Debug.Log("In EnemyBoss.cs No Prefab");
                }
               
            }
            else if (randomValue < enemySpawnChance + rangedEnemySpawnChance) // 敌人 B 出现概率
            {
                // `RangedEnemy`
                if(enemyPrefabs.Length > 1)
                {
                    minionPrefab = enemyPrefabs[1];
                } 
                else{
                    Debug.Log("In EnemyBoss.cs No Ranged Enemy Set");
                }
            }
            // // **如果未来增加更多小怪，这里可以继续扩展**
            // else if (randomValue < enemySpawnChance + rangedEnemySpawnChance + phantomEnemySpawnChance)
            // {
            //     minionPrefab = enemyPrefabs[2]; // `PhantomEnemy`
            // }
            // else
            // {
            //     minionPrefab = enemyPrefabs[3]; // 其他新类型小怪
            // }

            // **实例化小怪**
            if (minionPrefab != null)
            {
                Instantiate(minionPrefab, summonPosition, Quaternion.identity);
            }
        }
    }

    // **弹幕模式**
    IEnumerator ShootPattern()
    {
        while (health > 0) // 只要 `Boss` 存活，就持续发射弹幕
        {
            if (canShoot)
            {
                FireProjectile();
                yield return new WaitForSeconds(shootCooldown);
            }
            else
            {
                yield return null;
            }
        }
    }

    void FireProjectile()
    {
        canShoot = false;

        // **计算 `Boss` 指向 `Target` 的角度**
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        // **计算子弹起始角度**
        float startAngle = targetAngle - (fanAngle / 2);
        float angleStep = fanAngle / (bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            float projectileDirX = Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            float projectileDirY = Mathf.Sin(currentAngle * Mathf.Deg2Rad);
            Vector2 projectileDirection = new Vector2(projectileDirX, projectileDirY);

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<EnemyProjectile>().SetDirection(transform.position + (Vector3)projectileDirection);
        }

        StartCoroutine(ResetShootCooldown());
    }

    IEnumerator ResetShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    protected override void PostDamage(int damage, Transform attacker)
    {
        // do nothing
    }

    public override void SetAggroTarget(Transform newTarget)
    {
        throw new System.NotImplementedException();
    }
}
