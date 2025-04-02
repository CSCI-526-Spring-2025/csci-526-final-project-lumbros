using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    public GameObject projectilePrefab; 
    public float attackRange = 5f;
    public float attackCooldown = 1f;
    private float lastAttackTime;
    public int damage = 1;
    public int heroBounces = 0; 
    public bool isEnemy = false; 

    void Update()
    {
        GameObject target = FindClosestTarget();
        if (target != null && Time.time - lastAttackTime > attackCooldown)
        {
            Shoot(target.transform);
            lastAttackTime = Time.time;
        }
    }

    GameObject FindClosestTarget()
    {
        string targetTag = isEnemy ? "Tower" : "Enemy";
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        GameObject closest = null;
        float minDistance = attackRange;

        foreach (GameObject target in targets)
        {
            var damageable = target.GetComponent<IDamageable>();
        if (this.CompareTag("Player") == false && damageable != null && damageable.getHealthExpected() <= 0)
        {
            continue;
        }
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = target;
            }
        }

        return closest;
    }

    void Shoot(Transform target)
    {
        
        var damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeExpectedDamage(damage);
        }
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetTarget(target, transform, heroBounces, isEnemy); 
        projectile.GetComponent<Projectile>().damage = damage; 
    }
}
