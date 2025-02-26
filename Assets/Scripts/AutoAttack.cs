using UnityEngine;
public class AutoAttack : MonoBehaviour
{
    public GameObject projectilePrefab; 
    public float attackRange = 5f;
    public float attackCooldown = 1f;
    private float lastAttackTime;
    public int damage = 1;
    public int heroBounces = 0; 

    void Update()
    {
        GameObject target = FindClosestEnemy();
        if (target != null && Time.time - lastAttackTime > attackCooldown)
        {
            Shoot(target.transform);
            lastAttackTime = Time.time;
        }
    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDistance = attackRange;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }

    void Shoot(Transform target)
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetTarget(target, transform, heroBounces); 
        projectile.GetComponent<Projectile>().damage = damage; 
    }
}
