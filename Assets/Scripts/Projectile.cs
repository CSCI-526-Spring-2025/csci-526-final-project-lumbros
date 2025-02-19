using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    private Transform target;
    public Transform shooter;
    public int damage = 1;

    public void SetTarget(Transform newTarget, Transform shooter)
    {
        target = newTarget;
        this.shooter = shooter; // Store the attacker
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
            // New damage handling system - looks for anything that can take damage
            var damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, shooter);
            }
            Destroy(gameObject);
        }
    }
}
