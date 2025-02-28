using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTower : MonoBehaviour
{
    public float boostRange = 3f;           
    public float speedBoostAmount = 1.5f;  
    private HashSet<GameObject> boostedUnits = new HashSet<GameObject>();

    void Update()
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, boostRange);


        HashSet<GameObject> unitsToRemove = new HashSet<GameObject>(boostedUnits);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player") || collider.CompareTag("Tower"))
            {
                GameObject unit = collider.gameObject;

                unitsToRemove.Remove(unit);


                if (!boostedUnits.Contains(unit))
                {
                    ApplySpeedBoost(unit);
                    boostedUnits.Add(unit);
                }
            }
        }

        foreach (GameObject unit in unitsToRemove)
        {
            RemoveSpeedBoost(unit);
            boostedUnits.Remove(unit);
        }
    }

    void ApplySpeedBoost(GameObject unit)
    {
        HeroMovement movement = unit.GetComponent<HeroMovement>();
        if (movement != null)
        {
            movement.moveSpeed *= speedBoostAmount;
        }
        AutoAttack attack = unit.GetComponent<AutoAttack>();
        if (attack != null)
        {
            attack.attackCooldown /= speedBoostAmount;
        }
    }

    void RemoveSpeedBoost(GameObject unit)
    {
        if (unit == null) return;

        HeroMovement movement = unit.GetComponent<HeroMovement>();
        if (movement != null)
        {
            movement.moveSpeed /= speedBoostAmount;
        }

        AutoAttack attack = unit.GetComponent<AutoAttack>();
        if (attack != null)
        {
            attack.attackCooldown *= speedBoostAmount;
        }
    }

    void OnDestroy()
    {
        foreach (GameObject unit in boostedUnits)
        {
            RemoveSpeedBoost(unit);
        }
    }
}