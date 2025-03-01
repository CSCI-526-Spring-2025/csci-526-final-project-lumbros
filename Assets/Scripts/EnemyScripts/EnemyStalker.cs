using UnityEngine;
using System.Collections;

public class EnemyStalker : EnemyAbstract
{
    public float normalSpeed = 1.5f;  // 初始移动速度
    public float chargeSpeed = 10f;   // 冲刺速度
    public float detectionRange = 5f; // 发现Player的范围
    public float chargeDelay = 1.5f;  // 冲刺前的延迟时间
    public float chargeCooldown = 2f; // 冲刺冷却时间
    public float rotationSpeed = 500f; // 旋转速度（越大越快）
    public float rotationOffset = 0; // 角度偏移（让你可以调整朝向）
    private bool isCharging = false;  // 是否正在冲刺
    private bool canCharge = true;    // 是否可以进行冲刺
    private Vector2 chargeTargetPos;  // 记录冲刺目标位置

<<<<<<<< HEAD:Assets/Scripts/Enemies/EnemyStalker.cs
    public int health = 3;           // 敌人生命值
    public int attackDamage = 2;     // 近战攻击伤害
    public float attackRange = 0.5f; // 近战攻击范围
    public float attackCooldown = 1f; // 攻击冷却时间
    private bool canAttack = true;    // 是否可以攻击

    private Transform target;         // 目标（默认为Player）
    private Transform player;
    private Transform core;
    
    private static GameObject manager; // 游戏管理器
    private WaveManager wavemanager;
========
>>>>>>>> 810ddcd (Finish refactoring enemies):Assets/Scripts/EnemyScripts/EnemyStalker.cs
    private Vector2 lastPosition;

    protected override void StartCall()
    {
<<<<<<<< HEAD:Assets/Scripts/Enemies/EnemyStalker.cs
        GameObject coreObject = GameObject.FindGameObjectWithTag("Core");
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if ((coreObject != null)&&(playerObject != null))
        {
            core = coreObject.transform;
            player = playerObject.transform;
            target = player;
        }
        manager = GameObject.FindGameObjectWithTag("Manager");
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyEnCol"), LayerMask.NameToLayer("EnemyEnCol"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyEnCol"), LayerMask.NameToLayer("Hero"));

        //初始化血量
        wavemanager = FindObjectOfType<WaveManager>();
        health = Mathf.CeilToInt(health * wavemanager.enemyHealthMultiplier);
========
        enemyType = "Enemy - Stalker";
        attackDamage = 2;
        target = GameObject.FindGameObjectWithTag("Player").transform;
>>>>>>>> 810ddcd (Finish refactoring enemies):Assets/Scripts/EnemyScripts/EnemyStalker.cs
    }

    protected override void Move()
    {
<<<<<<<< HEAD:Assets/Scripts/Enemies/EnemyStalker.cs
        if ((player == null)||(core == null)) return;

========
>>>>>>>> 810ddcd (Finish refactoring enemies):Assets/Scripts/EnemyScripts/EnemyStalker.cs
        // **计算移动方向**
        Vector2 movementDirection = ((Vector2)transform.position - lastPosition).normalized;

        // **更新位置信息**
        lastPosition = transform.position;

        // **如果移动方向有效，更新旋转**
        if (movementDirection.magnitude > 0.01f)
        {
            RotateTowardsMovementDirection(movementDirection);
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        // **处理冲刺逻辑**
        if (!isCharging && canCharge && distanceToTarget <= detectionRange)
        {
            StartCoroutine(PrepareCharge());
        }

        // **普通移动**
        if (!isCharging)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, normalSpeed * Time.deltaTime);
        }
        // **冲刺移动**
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, chargeTargetPos, chargeSpeed * Time.deltaTime);

            // **停止冲刺的条件：到达目标点**
            if (Vector2.Distance(transform.position, chargeTargetPos) < 0.2f)
            {
                StopCharge();
            }
        }
    }

    void RotateTowardsMovementDirection(Vector2 direction)
    {
        // **计算目标角度**
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;

        // **平滑旋转**
        float angle = Mathf.LerpAngle(transform.rotation.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    IEnumerator PrepareCharge()
    {
        canCharge = false; // 进入冲刺CD
        yield return new WaitForSeconds(chargeDelay);

        // **记录冲刺目标位置**
        chargeTargetPos = target.position;
        isCharging = true;
    }

    void StopCharge()
    {
        isCharging = false;
        StartCoroutine(ChargeCooldown());
    }

    IEnumerator ChargeCooldown()
    {
        yield return new WaitForSeconds(chargeCooldown);
        canCharge = true; // 冲刺冷却结束，可以再次冲刺
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
