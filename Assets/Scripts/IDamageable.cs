using UnityEngine;

// This interface defines what any damageable object must be able to do
public interface IDamageable
{
    void TakeDamage(int damage, Transform attacker);
}