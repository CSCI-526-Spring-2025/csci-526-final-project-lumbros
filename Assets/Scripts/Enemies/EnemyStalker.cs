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

    private Vector2 lastPosition;
    private Transform player;

    protected override void StartCall()
    {
        enemyType = "Enemy - Stalker";
        attackDamage = 2;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            target = player;
        }
        //初始化血量
        health = Mathf.CeilToInt(health * WaveManager.Instance.enemyHealthMultiplier);
    }

    protected override void Move()
    {
        if ((player == null)||(core == null)) return;

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
