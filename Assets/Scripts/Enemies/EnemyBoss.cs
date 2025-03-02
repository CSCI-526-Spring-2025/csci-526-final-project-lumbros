using UnityEngine;
using System.Collections;

public class BossEnemy : MonoBehaviour, IDamageable
{
    public float detectionRange = 10f; // 发现 Player 的范围
    public float speed = 1f; // 移动速度
    public int maxHealth = 10; // Boss 最大生命值
    public int health; // 当前生命值
    public int attackDamage = 1; // 近战伤害
    public float attackRange = 1f; // 近战攻击范围
    public float attackCooldown = 1.5f; // 近战攻击冷却
    public bool canAttack = true; // 是否能攻击

    public GameObject projectilePrefab; // 弹幕子弹
    public float fanAngle = 60f; // 扇形弹幕角度
    public int bulletCount = 6; // 子弹数量
    public float shootCooldown = 1f; // 射击冷却时间
    private bool canShoot = true; // 是否能射击
    private bool isEnraged = false; // 是否进入弹幕模式

    public GameObject[] enemyPrefabs; // 小怪 Prefabs
    public int summonCount = 3; // 召唤总数量
    public float summonRadius = 3f; // 召唤半径
    public float enemySpawnChance = 0.7f; // Enemy 召唤概率（RangedEnemy 概率 = 1 - enemySpawnChance）
    public float rangedEnemySpawnChance = 0.3f;

    private Transform target; // 当前攻击目标
    private Transform core; // `Core` 位置
    private Transform player; // `Player` 位置
    private static GameObject manager; // 游戏管理器
    private WaveManager wavemanager;

    private bool summonedAt50 = false;
    private bool summonedAt25 = false;

    void Start()
    {
        // **初始化目标为 Core**
        GameObject coreObject = GameObject.FindGameObjectWithTag("Core");
        if (coreObject != null)
        {
            core = coreObject.transform;
            target = core;
        }
        else
        {
            Debug.LogError("BossEnemy: 找不到 `Core`，请检查是否存在带 `Core` 标签的对象！");
        }
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // **初始化血量**
        wavemanager = FindObjectOfType<WaveManager>();
        maxHealth = Mathf.CeilToInt(maxHealth * wavemanager.enemyHealthMultiplier);
        health = maxHealth;

        // **获取 Manager**
        manager = GameObject.FindGameObjectWithTag("Manager");
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyEnCol"), LayerMask.NameToLayer("EnemyEnCol"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyEnCol"), LayerMask.NameToLayer("Hero"));
    }

    void Update()
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

        TryAttack();
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
            float randomValue = Random.value; // 0~1 之间的随机数

            if (randomValue < enemySpawnChance) // 敌人 A 出现概率
            {
                minionPrefab = enemyPrefabs[0]; // `Enemy`
            }
            else if (randomValue < enemySpawnChance + rangedEnemySpawnChance) // 敌人 B 出现概率
            {
                minionPrefab = enemyPrefabs[1]; // `RangedEnemy`
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


    // **近战攻击逻辑**
    void TryAttack()
    {
        if (!canAttack) return;

        if (Vector2.Distance(transform.position, target.position) <= attackRange)
        {
            StartCoroutine(AttackTarget());
        }
    }

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
            projectile.GetComponent<EnemyProjectile>().SetDirection(transform.position + (Vector3)projectileDirection, transform);
        }

        StartCoroutine(ResetShootCooldown());
    }

    IEnumerator ResetShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    // **受到伤害**
    public void TakeDamage(int damage, Transform attacker)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
            if (manager != null && manager.GetComponent<CustomSceneManager>() != null)
            {
                manager.GetComponent<CustomSceneManager>().AddKill();
            }
        }
    }
}
