using UnityEngine;
using System.Collections;
using TMPro;
public class Mine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Worker"))
        {
            Worker worker = collision.GetComponent<Worker>();

            if (worker.target == transform)
            {
                // Return worker to core with 1 gold
                worker.SetTargetCore(10);
            }
        }
    }
}