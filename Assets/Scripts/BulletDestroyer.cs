using UnityEngine;

public class BulletDestroyer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("bullet"))  
        {
            Destroy(other.gameObject);  
        }
    }
}