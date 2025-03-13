using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    private Transform target;
    public Transform shooter;
    public int damage = 1;
    public int bouncesLeft = 0;
    public bool isEnemyProjectile = false; 

    public void SetTarget(Transform newTarget, Transform shooter, int extraBounces, bool isEnemy)
    {
        target = newTarget;
        this.shooter = shooter;
        bouncesLeft = extraBounces;
        isEnemyProjectile = isEnemy;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        
        var damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage, shooter);
        }

        if (bouncesLeft > 0)
        {
            Transform nextTarget = FindNextEnemy(target);
            Debug.Log("bounces left: " + bouncesLeft + "target: " + nextTarget);
            if (nextTarget != null)
            {
                target = nextTarget;
                bouncesLeft--;
                return;
            }
        }
        Debug.Log("bounces left: " + bouncesLeft);
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

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = enemy.transform;
            }
        }

        return bestTarget;
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     GameObject other = collision.gameObject; 

    //     if (isEnemyProjectile && (other.CompareTag("Tower") || other.CompareTag("Core") || other.layer == LayerMask.NameToLayer("Buildings")))
    //     {
    //         var damageable = other.GetComponent<IDamageable>();
    //         if (damageable != null)
    //         {
    //             damageable.TakeDamage(damage, shooter);
    //         }
    //         Destroy(gameObject);
    //     }
    // }


    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
