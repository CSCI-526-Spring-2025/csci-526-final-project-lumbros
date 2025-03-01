using UnityEngine;
using System.Collections;

public class RangedEnemy : EnemyAbstract
{
    // Ranged attack properties
    public GameObject projectilePrefab;

    protected override void StartCall()
    {
        enemyType = "Enemy - Range";
        attackCooldown = 2f;
        attackRange = 5f;
        target = FindClosestTarget();
    }

    protected override void Move()
    {
        // If outside attack range, move closer
        if (Vector2.Distance(transform.position, target.position) > attackRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    // **Attempt to attack the current target**
    protected override void TryAttack()
    {
        if (target != null && Vector2.Distance(transform.position, target.position) <= attackRange && canAttack)
        {
            StartCoroutine(ShootAtTarget());
        }
    }

    protected override void BeforeUpdate()
    {
        // Regularly update target to find closest one
        if (!isAggroed)  // Only search for new targets if not aggroed
        {
            target = FindClosestTarget();
        }
    }

    // New method to find closest target
    Transform FindClosestTarget()
    {
        if (GameObject.FindGameObjectWithTag("Core") == null)
        {
            return null;
        }
        // Create a list of possible targets
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = float.MaxValue;
        Transform closestTarget = core; // Default to core if no closer targets found

        // Check distance to core
        float coreDistance = Vector2.Distance(transform.position, core.position);
        minDistance = coreDistance;

        // Check distance to each player
        foreach (GameObject player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTarget = player.transform;
            }
        }

        return closestTarget;
    }

    IEnumerator ShootAtTarget()
    {
        canAttack = false;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<EnemyProjectile>().SetDirection(target.position, transform);
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public override void SetAggroTarget(Transform newTarget)
    {
        if (!isAggroed)
        { 
            target = newTarget;
            isAggroed = true;
        }
    }

    protected override void LateUpdate()
    {
        if (isAggroed && target == null)
        {
            target = FindClosestTarget();
            isAggroed = false;
        }
    }
}