using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 5f;          // Projectile speed
    private Vector2 direction;        // Direction the projectile is moving
    public int damage = 1;           // Damage value

    // Instead of tracking a target, we'll set a direction when fired
    public void SetDirection(Vector2 targetPosition)
    {
        // Calculate direction from shooter to target position
        direction = (targetPosition - (Vector2)transform.position).normalized;

        // Rotate projectile to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        // Move in the set direction
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;


        // Check if we hit a damageable object (Player, Core, or Tower)
        if (other.CompareTag("Player") || other.CompareTag("Core") || other.CompareTag("Tower") || other.CompareTag("Worker"))
        {
            // Deal damage if the object has a Health component
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                Debug.Log("take damage");
                health.TakeDamage(damage, other.tag);
            }

            // Destroy projectile after hitting a valid target
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
