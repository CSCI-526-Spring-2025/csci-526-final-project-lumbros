using UnityEngine;
using System.Collections;
public class Mine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("mine triggered");
        if (collision.CompareTag("Worker"))
        {
            Debug.Log("worker entered mine");
            Worker worker = collision.GetComponent<Worker>();
            worker.SetTargetCore();
        }
    }
}