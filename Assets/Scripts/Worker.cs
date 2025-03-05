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
    private int goldAmount = 0;
    private bool isMoving = true;

    void Start()
    {
        SetTargetMine();
        manager = GameObject.FindGameObjectWithTag("Manager");

        rb = GetComponent<Rigidbody2D>();
        rb.drag = 2f;
    }

    public int GetGoldAmount()
    {
        return goldAmount;
    }

    public void SetTargetMine()
    {
        goldAmount = 0;
        GameObject[] mines = GameObject.FindGameObjectsWithTag("Mine");
        if(mines.Length > 0)
        {
            // Choose a random mine from mines
            target = mines[Random.Range(0, mines.Length)].transform;
        }
    }

    public void SetTargetCore(int gold)
    {
        target = GameObject.FindGameObjectWithTag("Core").transform;
        goldAmount = gold;
    }

    public void StopMovement()
    {
        // TODO: call this when game is paused / on upgrades scene
        isMoving = false;
    }

    public void ResumeMovement()
    {
        isMoving = true;
    }

    void Update()
    {
        if (target != null && isMoving)
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
