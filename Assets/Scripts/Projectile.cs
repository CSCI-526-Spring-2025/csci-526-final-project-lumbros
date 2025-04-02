using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public Transform target;
    public Transform shooter;
    public int damage = 1;
    public int bouncesLeft = 0;
    public bool isEnemyProjectile = false;

    private bool reachedTarget = false;
    private Vector3 targetPosition;

    private float lifeTime = 0f;
    private const float maxLifeTime = 5f; 
    public void SetTarget(Transform newTarget, Transform shooter, int extraBounces, bool isEnemy)
    {
        target = newTarget;

        if (target != null)
        {
            targetPosition = target.position;

            // var damageable = target.GetComponent<IDamageable>();
            // damageable?.TakeExpectedDamage(damage);
        }

        this.shooter = shooter;
        bouncesLeft = extraBounces;
        isEnemyProjectile = isEnemy;
        reachedTarget = false;
    }

    void Update()
    {
        if (target != null)
        {
            targetPosition = target.position;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (!reachedTarget && Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            reachedTarget = true;
            HitTarget();
        }

        lifeTime += Time.deltaTime;
        if (lifeTime > maxLifeTime)
        {
            Destroy(gameObject);
        }
    }

    void HitTarget()
    {
        if (target != null)
        {
            var damageable = target.GetComponent<IDamageable>();
            damageable?.TakeDamage(damage, shooter);
        }

        if (bouncesLeft > 0)
        {
            Transform nextTarget = FindNextEnemy(target);
            if (nextTarget != null)
            {
                SetTarget(nextTarget, shooter, bouncesLeft - 1, isEnemyProjectile);
                return;
            }
        }

        Destroy(gameObject);
    }

    Transform FindNextEnemy(Transform previousTarget)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform bestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (enemy.transform == previousTarget) continue;

            var damageable = enemy.GetComponent<IDamageable>();
            if (damageable == null) continue;
            if (damageable.getHealthExpected() <= 0) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = enemy.transform;
            }
        }

        if (bestTarget == null)
        {
            closestDistance = Mathf.Infinity;
            foreach (GameObject enemy in enemies)
            {
                if (enemy.transform == previousTarget) continue;

                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = enemy.transform;
                }
            }
        }

        if (bestTarget != null)
        {
            var dmg = bestTarget.GetComponent<IDamageable>();
            dmg?.TakeExpectedDamage(damage); 
        }

        return bestTarget;
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
