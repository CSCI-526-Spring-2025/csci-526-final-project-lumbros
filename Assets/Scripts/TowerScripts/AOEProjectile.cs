using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEProjectile : Projectile
{
    public float explosionRadius = 2f;    // Area of effect radius
    public int splashDamage = 1;          // Splash damage amount

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit an enemy
        if (other.CompareTag("Enemy"))
        {
            // Find all colliders within explosion radius
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

            // Apply damage to all enemies in range
            foreach (Collider2D collider in nearbyColliders)
            {
                // Using tag system makes our code work with any type of enemy
                if (collider.CompareTag("Enemy"))
                {
                    // If the enemy has IDamageable interface, apply damage
                    var damageable = collider.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(damage, transform);
                    }
                }
            }
        }
    }
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
   
   
}