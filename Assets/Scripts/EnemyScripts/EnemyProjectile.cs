using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 5f;          // Projectile speed
    private Vector2 direction;        // Direction the projectile is moving
    public int damage = 1;           // Damage value

    // Instead of tracking a target, we'll set a direction when fired
    public void SetDirection(Vector2 targetPosition, Transform shooter)
    {
        // Calculate direction from shooter to target position
        direction = (targetPosition - (Vector2)transform.position).normalized;

        // Optional: Rotate projectile to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        // Move in the set direction
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // Optional: Destroy bullet after certain time or distance
        // You might want to add this to prevent bullets from flying forever
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check what we hit
        if (other.CompareTag("Player") || other.CompareTag("Core") || other.CompareTag("Tower"))
        {
            // Deal damage if we hit player or core
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage, other.tag);
            }

            // Destroy the projectile after hitting
            Destroy(gameObject);
        }
    }
}