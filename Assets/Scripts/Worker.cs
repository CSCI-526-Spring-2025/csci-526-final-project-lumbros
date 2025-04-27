using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Worker : MonoBehaviour, IDamageable
{
    public float speed = 0.5f;
    public int health = 5;
    public float acceleration = 5f;
    public float maxSpeed = 1f;

    public Transform target;
    NavMeshAgent agent;
    private static GameObject manager;
    private Rigidbody2D rb;
    private int goldAmount = 0;

    public Animator animator;

    void Start()
    {
        SetTargetMine();
        manager = GameObject.FindGameObjectWithTag("Manager");

        rb = GetComponent<Rigidbody2D>();
        rb.drag = 2f;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        animator = GetComponent<Animator>();
    }

    public int GetGoldAmount()
    {
        return goldAmount;
    }
    public void TakeExpectedDamage(int dmg){
        return;
    }
    public int getHealthExpected(){
        return 0;
    }
    public void SetTargetMine()
    {
        goldAmount = 0;
        GameObject[] mines = GameObject.FindGameObjectsWithTag("Mine");
        if(mines.Length > 0)
        {
            // Choose a random mine from mines
            target = mines[Random.Range(0, mines.Length)].transform;
            Debug.Log("Setting target to mine " + target.name);
        }
    }

    public void SetTargetCore(int gold)
    {
        if (GameObject.FindGameObjectWithTag("Core") != null)
        {
            target = GameObject.FindGameObjectWithTag("Core").transform;
        }
        goldAmount = gold;
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            // get X and Y value
            float x = target.position.x - transform.position.x;
            float y = target.position.y - transform.position.y;

            // set moveX to 1 if x is greater than y
            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                x = x > 0 ? 1 : -1;
                y = 0;
            }
            else
            {
                y = y > 0 ? 1 : -1;
                x = 0;
            }

            animator.SetFloat("moveX", x);
            animator.SetFloat("moveY", y);

            // agent.SetDestination(target.position);
        }
    }

    //**Take damage**
    public void TakeDamage(int damage, Transform attacker)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
