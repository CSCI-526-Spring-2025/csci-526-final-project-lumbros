using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceProjectile : Projectile
{
    public float slowAmount = 0.5f;    
    public float slowDuration = 3f;   


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyAbstract normalEnemy = other.GetComponent<EnemyAbstract>();
            StartCoroutine(ApplySlowEffect(normalEnemy));
            // Enemy normalEnemy = other.GetComponent<Enemy>();
            // RangedEnemy rangedEnemy = other.GetComponent<RangedEnemy>();

            // if (normalEnemy != null)
            // {
            //     StartCoroutine(ApplySlowEffect(normalEnemy));
            // }
            // else if (rangedEnemy != null)
            // {
            //     StartCoroutine(ApplySlowEffect(rangedEnemy));
            // }
        }
    }

    IEnumerator ApplySlowEffect(EnemyAbstract enemy)
    {
        var speedField = enemy.GetType().GetField("speed");
        float originalSpeed = (float)speedField.GetValue(enemy);
        speedField.SetValue(enemy, originalSpeed * slowAmount);
        enemy.SetSlowDebuff(true);

        yield return new WaitForSeconds(slowDuration);

        if (enemy != null)
        {
            speedField.SetValue(enemy, originalSpeed);
            enemy.SetSlowDebuff(false);
        }
    }
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
