using UnityEngine;
using System.Collections;

public class Worker : MonoBehaviour, IDamageable
{
    public float speed = 0.5f;
    public int health = 5;
    public float acceleration = 5f;
    public float maxSpeed = 1f;

    private Transform target; // Current target
    private static GameObject manager;
    private Rigidbody2D rb;
    private bool hasGold = false;

    void Start()
    {
        SetTargetMine();
        manager = GameObject.FindGameObjectWithTag("Manager");

        rb = GetComponent<Rigidbody2D>();
        rb.drag = 2f;
    }

    public bool HasGold()
    {
        return hasGold;
    }

    public void SetTargetMine()
    {
        hasGold = false;
        GameObject[] mines = GameObject.FindGameObjectsWithTag("Mine");

        // Choose a random mine from mines
        target = mines[Random.Range(0, mines.Length)].transform;
    }

    public void SetTargetCore()
    {
        target = GameObject.FindGameObjectWithTag("Core").transform;
        hasGold = true;
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    // **Take damage**
    public void TakeDamage(int damage, Transform attacker)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
